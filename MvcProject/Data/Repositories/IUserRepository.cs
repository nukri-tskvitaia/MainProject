namespace MvcProject.Data.Repositories;

public interface IUserRepository
{
    public Task<string> GetUsernameAsync(string userId);
}
