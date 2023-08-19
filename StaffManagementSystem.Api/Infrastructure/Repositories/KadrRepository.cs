using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StaffManagementSystem.Api.Domain.Entities;
using StaffManagementSystem.Api.Infrastructure.IRepositories;

namespace StaffManagementSystem.Api.Infrastructure.Repositories
{
    public class KadrRepository : IKadrRepository
    {
        private readonly AppDbContext context;
        private readonly DbSet<Kadr> dbSet;

        public KadrRepository(AppDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<Kadr>();
        }

        public async Task DeleteAsync(Kadr kadr)
        {
            EntityEntry<Kadr> deletedEntity = this.dbSet.Remove(kadr);
            await this.context.SaveChangesAsync();
        }

        public IQueryable<Kadr> GetAll() => this.dbSet;

        public async Task<Kadr> GetByIdAsync(Guid id) => await this.dbSet.FindAsync(id);

        public IQueryable<Kadr> GetSelected(Expression<Func<Kadr, bool>> expression) =>
            this.dbSet.Where(expression);

        public async Task<Kadr> InsertAsync(Kadr kadr)
        {
            var insertedEntity = await this.dbSet.AddAsync(kadr);
            await this.context.SaveChangesAsync();

            return insertedEntity.Entity;
        }

        public async Task<Kadr> UpdateAsync(Kadr kadr)
        {
            var updatedEntity = this.dbSet.Update(kadr);
            await this.context.SaveChangesAsync();

            return updatedEntity.Entity;
        }
    }
}
