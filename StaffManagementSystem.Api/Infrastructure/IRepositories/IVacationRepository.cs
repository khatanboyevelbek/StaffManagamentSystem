using StaffManagementSystem.Api.Domain.Entities;
using System.Linq.Expressions;

namespace StaffManagementSystem.Api.Infrastructure.IRepositories
{
    public interface IVacationRepository
    {
        Task<Vacation> InsertAsync(Vacation vacation);
        Task<Vacation> UpdateAsync(Vacation Vacation);
        Task DeleteAsync(Vacation Vacation);
        IQueryable<Vacation> GetAll();
        IQueryable<Vacation> GetSelected(Expression<Func<Vacation, bool>> expression);
        Task<Vacation> GetByIdAsync(Guid id);
    }
}
