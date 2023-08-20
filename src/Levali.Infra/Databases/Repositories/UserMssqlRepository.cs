using Dapper;
using Levali.Core;
using Serilog;

namespace Levali.Infra;

public class UserMssqlRepository : IUserRepository
{
    private readonly DbSession _session;
    private readonly ILogger _logger;

    public UserMssqlRepository(DbSession session, ILogger logger)
    {
        _session = session;
        _logger = logger;
    }

    public async Task Insert(UserEntity user)
    {
        try
        {
            var query = $"INSERT INTO master.dbo.Users (Name, Email, PasswordHash, CreatedAt) VALUES (@Name, @Email, @PasswordHash, @CreatedAt);";
            await _session.Connection.ExecuteAsync(query, new
            {
                user.Id,
                user.Name,
                Email = user.Email.ToString(),
                user.PasswordHash,
                user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger
                .ForContext(nameof(user), user, destructureObjects: true)
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Error insert user");

            throw;
        }
    }

    public async Task<UserEntity?> GetByEmail(Email email)
    {
        try
        {
            var query = $"SELECT Id, Name, Email, PasswordHash, CreatedAt FROM master.dbo.Users WHERE Email = @email;";
            return await _session.Connection.QuerySingleOrDefaultAsync<UserEntity>(query, new { email = email.ToString() });
        }
        catch (Exception ex)
        {
            _logger
                .ForContext(nameof(email), email)
                .ForContext(nameof(Exception), ex, destructureObjects: true)
                .Error("Error get by email");

            return default;
        }
    }
}
