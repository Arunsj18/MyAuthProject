using MyAuth.Models;
using System.Data;
using Microsoft.Data.SqlClient;

namespace MyAuth.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly IConfiguration _configuration;

        public AuthRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SqlConnection GetConnection()
        {
            return new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand(
                "SELECT * FROM Users WHERE Email = @Email",
                connection);

            command.Parameters.AddWithValue("@Email", email);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = Convert.ToInt32(reader["Id"]),
                    UserName = reader["UserName"].ToString(),
                    Email = reader["Email"].ToString(),
                    PasswordHash = reader["PasswordHash"].ToString(),
                    PasswordSalt = reader["PasswordSalt"]?.ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    IsEmailConfirmed = Convert.ToBoolean(reader["IsEmailConfirmed"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                };
            }

            return null;
        }

        public async Task<int> AddUserAsync(User user)
        {
            using var connection = GetConnection();
            using var command = new SqlCommand(@"
                INSERT INTO Users
                (UserName, Email, PasswordHash, PasswordSalt, IsActive, IsEmailConfirmed, CreatedAt)
                VALUES
                (@UserName, @Email, @PasswordHash, @PasswordSalt, 1, 0, SYSDATETIME());
                SELECT SCOPE_IDENTITY();",
                connection);

            command.Parameters.AddWithValue("@UserName", user.UserName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            command.Parameters.AddWithValue("@PasswordSalt", (object?)user.PasswordSalt ?? DBNull.Value);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();

            return Convert.ToInt32(result);
        }

    }
}