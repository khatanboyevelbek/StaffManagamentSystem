using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StaffManagementSystem.Api.Domain.Entities;
using StaffManagementSystem.Api.Infrastructure.IRepositories;

namespace StaffManagementSystem.Api.Infrastructure.Repositories
{
    public class DirectorRepository : IDirectorRepository
    {
        private readonly AppDbContext context;
        private readonly DbSet<Director> dbSet;

        public DirectorRepository(AppDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<Director>();
        }

        public async Task DeleteAsync(Director director)
        {
            EntityEntry<Director> deletedEntity = this.dbSet.Remove(director);
            await this.context.SaveChangesAsync();
        }

        public IQueryable<Director> GetAll() => this.dbSet;

        public async Task<Director> GetByIdAsync(Guid id) => await this.dbSet.FindAsync(id);

        public async Task<Director> GetDirectorByEmailAsync(string email) =>
            await this.dbSet.FirstOrDefaultAsync(a => a.Email == email);

        public IQueryable<Director> GetSelected(Expression<Func<Director, bool>> expression) =>
            this.dbSet.Where(expression);

        public async Task<Director> InsertAsync(Director director)
        {
            var insertedEntity = await this.dbSet.AddAsync(director);
            await this.context.SaveChangesAsync();

            return insertedEntity.Entity;
        }

        public async Task<Director> UpdateAsync(Director director)
        {
            var updatedEntity = this.dbSet.Update(director);
            await this.context.SaveChangesAsync();

            return updatedEntity.Entity;
        }
    }
}
