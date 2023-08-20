namespace Levali.Core;

public interface IUserRepository
{
    public Task Insert(UserEntity user);
    public Task<UserEntity?> GetByEmail(Email email);
}
