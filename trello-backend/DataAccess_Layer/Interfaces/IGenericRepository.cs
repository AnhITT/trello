using DataAccess_Layer.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataAccess_Layer.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        DbSet<T> Context();
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression, bool includeDeleted = false);
        IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector, bool includeDeleted = false);
        T FirstOrDefault(Expression<Func<T, bool>> expression, bool includeDeleted = false);
        IEnumerable<T> GetAll(bool includeDeleted = false);
        GenericPageModel<T> GetPage(int skip, int take, Expression<Func<T, bool>>? filter,
            Expression<Func<T, string>>? orderBy, bool includeDeleted = false);
        List<T> GetUserFromObject(Expression<Func<T, bool>>? filter);
        T GetById(object id, bool includeDeleted = false);
        void Restore(T entity);
        void Remove(T entity, bool hardDelete = false);
        void RemoveRange(IEnumerable<T> entities, bool hardDelete = false);
        void Commit();
        void Dispose();
        void RollBack();
        void SaveChanges();
        bool SaveChangesBool();
    }
}
