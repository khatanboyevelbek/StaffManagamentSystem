using StaffManagementSystem.Api.Domain.Entities;
using System.Linq.Expressions;

namespace StaffManagementSystem.Api.Infrastructure.IRepositories
{
    public interface IUserRepository
    {
        Task<User> InsertAsync(User user);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(User user);
        IQueryable<User> GetAll();
        IQueryable<User> GetSelected(Expression<Func<User, bool>> expression);
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetUserByEmailAsync(string email);
    }
}
