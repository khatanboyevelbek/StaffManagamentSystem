using System.Linq.Expressions;
using StaffManagementSystem.Api.Domain.Entities;

namespace StaffManagementSystem.Api.Infrastructure.IRepositories
{
    public interface IAdminRepository
    {
        Task<Admin> InsertAsync(Admin admin);
        Task<Admin> UpdateAsync(Admin admin);
        Task DeleteAsync(Admin admin);
        IQueryable<Admin> GetAll();
        IQueryable<Admin> GetSelected(Expression<Func<Admin, bool>> expression);
        Task<Admin> GetByIdAsync(Guid id);
        Task<Admin> GetAdminByEmailAsync(string email);
    }
}
