namespace test_prism_814.Models
{
    public class MenuItemModel
    {
        public string Header { get; set; }    // 菜单显示文字（如 "便签管理"）
        public string Icon { get; set; }      // 图标对应的 Unicode 字符（如 "\uE8A5"）
        public string ViewName { get; set; }  // 导航目标名称（如 "ViewA"）
    }
}