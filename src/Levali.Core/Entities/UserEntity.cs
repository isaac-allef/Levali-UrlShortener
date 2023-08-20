using System.Security.Cryptography;
using System.Text;

namespace Levali.Core;

public class UserEntity
{
    public int Id { get; private set; }
    public string Name { get; set; }
    public Email Email { get; set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public UserEntity(string name, Email email, Password password) : this(0, name, email, GenerateHash(password), DateTime.UtcNow)
    {
    }

    private UserEntity(int id, string name, string email, string passwordHash, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public void SetPassword(string password)
        => PasswordHash = GenerateHash(password);

    public static string GenerateHash(Password input)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] hash = sha256.ComputeHash(bytes);

        var hashString = Convert.ToBase64String(hash);
        return hashString;
    }
}
