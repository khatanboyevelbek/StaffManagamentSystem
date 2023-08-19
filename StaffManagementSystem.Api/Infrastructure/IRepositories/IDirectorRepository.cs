using StaffManagementSystem.Api.Domain.Entities;
using System.Linq.Expressions;

namespace StaffManagementSystem.Api.Infrastructure.IRepositories
{
    public interface IDirectorRepository
    {
        Task<Director> InsertAsync(Director director);
        Task<Director> UpdateAsync(Director director);
        Task DeleteAsync(Director director);
        IQueryable<Director> GetAll();
        IQueryable<Director> GetSelected(Expression<Func<Director, bool>> expression);
        Task<Director> GetByIdAsync(Guid id);
    }
}
