using System.Data;
using System.Data.SqlClient;

namespace Levali.Infra;

public sealed class DbSession : IDisposable
{
    public IDbConnection Connection { get; }

    public DbSession(string connectionString)
    {
        var builder = new SqlConnectionStringBuilder(connectionString)
        {
            Pooling = true,
            MinPoolSize = 0,
            MaxPoolSize = 20
        };

        Connection = new SqlConnection(builder.ConnectionString);
        Connection.Open();
    }

    public void Dispose() => Connection?.Dispose();
}
