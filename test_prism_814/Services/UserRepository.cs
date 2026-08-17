using Dapper;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using test_prism_814.Models;

namespace test_prism_814.Services
{
    public class UserRepository
    {
        private readonly string _connectionString = "Data Source=notes.db";

        public UserRepository()
        {
            CreateTableIfNotExists();
            SeedDefaultUsers();
        }

        private void CreateTableIfNotExists()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Execute(@"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Account TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    Phone TEXT,
                    Address TEXT,
                    Role TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL
                )
            ");
        }

        private void SeedDefaultUsers()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            // 检查是否已有管理员
            var adminExists = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Users WHERE Role = 'Admin'");
            if (adminExists == 0)
            {
                var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                // 插入默认管理员
                connection.Execute(@"
                    INSERT INTO Users (Account, Password, Phone, Address, Role, CreatedAt)
                    VALUES ('admin', '123456', '13800000000', '北京市海淀区', 'Admin', @CreatedAt)
                ", new { CreatedAt = now });

                // 插入两个测试普通用户
                connection.Execute(@"
                    INSERT INTO Users (Account, Password, Phone, Address, Role, CreatedAt)
                    VALUES 
                    ('user1', '123456', '13800001111', '上海市浦东新区', 'User', @CreatedAt),
                    ('user2', '123456', '13800002222', '广州市天河区', 'User', @CreatedAt)
                ", new { CreatedAt = now });
            }
        }

        // 根据账号查询用户
        public async Task<User> GetByAccountAsync(string account)
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Account = @Account", new { Account = account }
            );
        }

        // 获取所有普通用户（管理员用，不包含管理员自己）
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            using var connection = new SqliteConnection(_connectionString);
            return await connection.QueryAsync<User>(
                "SELECT * FROM Users WHERE Role = 'User' ORDER BY Id"
            );
        }

        // 检查账号是否已存在
        public async Task<bool> AccountExistsAsync(string account)
        {
            using var connection = new SqliteConnection(_connectionString);
            var count = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE Account = @Account", new { Account = account }
            );
            return count > 0;
        }

        // 新增用户
        public async Task<int> InsertAsync(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"
                INSERT INTO Users (Account, Password, Phone, Address, Role, CreatedAt)
                VALUES (@Account, @Password, @Phone, @Address, @Role, @CreatedAt);
                SELECT last_insert_rowid();
            ";
            return await connection.ExecuteScalarAsync<int>(sql, user);
        }

        // 更新用户
        public async Task UpdateAsync(User user)
        {
            using var connection = new SqliteConnection(_connectionString);
            var sql = @"
                UPDATE Users 
                SET Account = @Account, Password = @Password, Phone = @Phone, Address = @Address
                WHERE Id = @Id
            ";
            await connection.ExecuteAsync(sql, user);
        }

        // 删除用户
        public async Task DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { Id = id });
        }
    }
}