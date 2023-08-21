using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StaffManagementSystem.Api.Domain.Entities;
using StaffManagementSystem.Api.Infrastructure.IRepositories;

namespace StaffManagementSystem.Api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext context;
        private readonly DbSet<User> dbSet;

        public UserRepository (AppDbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<User>();
        }

        public async Task DeleteAsync(User user)
        {
            EntityEntry<User> deletedEntity = this.dbSet.Remove(user);
            await this.context.SaveChangesAsync();
        }

        public IQueryable<User> GetAll() => this.dbSet;

        public async Task<User> GetByIdAsync(Guid id) => await this.dbSet.FindAsync(id);

        public IQueryable<User> GetSelected(Expression<Func<User, bool>> expression) =>
            this.dbSet.Where(expression);

        public async Task<User> GetUserByEmailAsync(string email) =>
            await this.dbSet.FirstOrDefaultAsync(a => a.Email == email);

        public async Task<User> InsertAsync(User user)
        {
            var insertedEntity = await this.dbSet.AddAsync(user);
            await this.context.SaveChangesAsync();

            return insertedEntity.Entity;
        }

        public async Task<User> UpdateAsync(User user)
        {
            var updatedEntity = this.dbSet.Update(user);
            await this.context.SaveChangesAsync();

            return updatedEntity.Entity;
        }
    }
}
