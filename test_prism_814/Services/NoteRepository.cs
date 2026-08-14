using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using test_prism_814.Models;

namespace test_prism_814.Services
{
    public class NoteRepository
    {
        private readonly string _connectionString = "Data Source=notes.db";

        public NoteRepository()
        {
            CreateTableIfNotExists();
        }

        private void CreateTableIfNotExists()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Notes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Content TEXT,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                )
            ");
        }

        // 获取所有便签
        public async Task<IEnumerable<Note>> GetAllAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<Note>("SELECT * FROM Notes ORDER BY UpdatedAt DESC");
        }

        // 🆕 搜索便签（按标题或内容模糊匹配）
        public async Task<IEnumerable<Note>> SearchAsync(string keyword)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"
                SELECT * FROM Notes 
                WHERE Title LIKE @Keyword OR Content LIKE @Keyword 
                ORDER BY UpdatedAt DESC
            ";
            // 使用 % 包裹关键词，实现“包含”匹配
            var param = new { Keyword = $"%{keyword}%" };
            return await connection.QueryAsync<Note>(sql, param);
        }

        // 新增便签
        public async Task<int> InsertAsync(Note note)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"
                INSERT INTO Notes (Title, Content, CreatedAt, UpdatedAt)
                VALUES (@Title, @Content, @CreatedAt, @UpdatedAt);
                SELECT last_insert_rowid();
            ";
            return await connection.ExecuteScalarAsync<int>(sql, note);
        }

        // 更新便签
        public async Task UpdateAsync(Note note)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = "UPDATE Notes SET Title = @Title, Content = @Content, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            await connection.ExecuteAsync(sql, note);
        }

        // 删除便签
        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.ExecuteAsync("DELETE FROM Notes WHERE Id = @Id", new { Id = id });
        }
    }
}