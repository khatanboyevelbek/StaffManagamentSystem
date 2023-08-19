using StaffManagementSystem.Api.Domain.Entities;
using System.Linq.Expressions;

namespace StaffManagementSystem.Api.Infrastructure.IRepositories
{
    public interface IKadrRepository
    {
        Task<Kadr> InsertAsync(Kadr kadr);
        Task<Kadr> UpdateAsync(Kadr kadr);
        Task DeleteAsync(Kadr kadr);
        IQueryable<Kadr> GetAll();
        IQueryable<Kadr> GetSelected(Expression<Func<Kadr, bool>> expression);
        Task<Kadr> GetByIdAsync(Guid id);
    }
}
