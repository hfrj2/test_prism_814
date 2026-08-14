using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using test_prism_814.Models;
using test_prism_814.Services;

namespace test_prism_814.ViewModels
{
    public class NoteManageUserControlViewModel : BindableBase
    {
        private readonly NoteRepository _repository;

        // ========== 便签列表 ==========
        private ObservableCollection<Note> _notes;
        public ObservableCollection<Note> Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }

        // ========== 当前选中的便签 ==========
        private Note _selectedNote;
        public Note SelectedNote
        {
            get => _selectedNote;
            set
            {
                // 如果点击的是同一个便签，先置空再重新赋值，强制触发所有更新
                if (_selectedNote == value && value != null)
                {
                    _selectedNote = null;
                    RaisePropertyChanged(nameof(SelectedNote));
                    Title = string.Empty;
                    Content = string.Empty;
                }

                // 正式赋值
                _selectedNote = value;
                RaisePropertyChanged(nameof(SelectedNote));

                // 更新编辑框
                if (value != null)
                {
                    Title = value.Title;
                    Content = value.Content;
                }
                else
                {
                    Title = string.Empty;
                    Content = string.Empty;
                }

                // ⭐ 关键：手动触发 DeleteCommand 的 CanExecute 重新评估
                (DeleteCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            }
        }

        // ========== 标题（编辑框绑定） ==========
        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        // ========== 内容（编辑框绑定） ==========
        private string _content;
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        // ========== 🆕 搜索关键词 ==========
        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                SetProperty(ref _searchKeyword, value);
                // 当用户输入时，自动触发搜索（如果不想自动，可以删掉这行）
                // 这里使用“输入即搜索”，体验更好
                if (string.IsNullOrWhiteSpace(value))
                {
                    // 如果清空搜索框，自动加载全部
                    _ = LoadAllNotesAsync();
                }
                else
                {
                    _ = SearchAsync();
                }
            }
        }

        // ========== 命令 ==========
        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand SearchCommand { get; }  // 🆕 搜索命令（也可手动触发）

        public NoteManageUserControlViewModel(NoteRepository repository)
        {
            _repository = repository;

            Notes = new ObservableCollection<Note>();

            // 初始化命令
            LoadCommand = new DelegateCommand(async () => await LoadAllNotesAsync());
            SaveCommand = new DelegateCommand(async () => await SaveNoteAsync());
            DeleteCommand = new DelegateCommand(async () => await DeleteNoteAsync(), () => SelectedNote != null);
            NewCommand = new DelegateCommand(ClearForm);
            SearchCommand = new DelegateCommand(async () => await SearchAsync()); // 🆕

            // 页面加载时自动读取数据
            _ = LoadAllNotesAsync();
        }

        // ========== 读取所有便签（全量加载） ==========
        private async Task LoadAllNotesAsync()
        {
            var list = await _repository.GetAllAsync();
            UpdateNotesList(list);
        }

        // ========== 🆕 搜索便签（按标题和内容模糊匹配） ==========
        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                // 如果关键词为空，加载全部
                await LoadAllNotesAsync();
                return;
            }

            // 调用 Repository 的搜索方法（见后面的 NoteRepository 更新）
            var list = await _repository.SearchAsync(SearchKeyword);
            UpdateNotesList(list);
        }

        // ========== 辅助方法：更新列表（避免重复代码） ==========
        private void UpdateNotesList(IEnumerable<Note> list)
        {
            Notes.Clear();
            foreach (var item in list)
            {
                Notes.Add(item);
            }
        }

        // ========== 保存（新增或更新） ==========
        private async Task SaveNoteAsync()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                // 简单提示：标题不能为空（你可以改成弹窗）
                return;
            }

            if (SelectedNote == null)
            {
                var newNote = new Note
                {
                    Title = Title,
                    Content = Content,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                await _repository.InsertAsync(newNote);
            }
            else
            {
                SelectedNote.Title = Title;
                SelectedNote.Content = Content;
                SelectedNote.UpdatedAt = DateTime.Now;
                await _repository.UpdateAsync(SelectedNote);
            }

            // 刷新列表（保留当前搜索状态）
            if (string.IsNullOrWhiteSpace(SearchKeyword))
                await LoadAllNotesAsync();
            else
                await SearchAsync();

            ClearForm();
        }

        // ========== 删除 ==========
        private async Task DeleteNoteAsync()
        {
            if (SelectedNote != null)
            {
                await _repository.DeleteAsync(SelectedNote.Id);
                if (string.IsNullOrWhiteSpace(SearchKeyword))
                    await LoadAllNotesAsync();
                else
                    await SearchAsync();
                ClearForm();
            }
        }

        // ========== 清空表单 ==========
        private void ClearForm()
        {
            SelectedNote = null;
            Title = string.Empty;
            Content = string.Empty;
        }
    }
}