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
        IEnumerable<T> FindIncludeDelete(System.Linq.Expressions.Expression<Func<T, bool>> expression);
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        IEnumerable<TResult> SelectIncludeDelete<TResult>(Expression<Func<T, TResult>> selector);
        IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector);
        T FirstOrDefault(System.Linq.Expressions.Expression<Func<T, bool>> expression);
        T FirstOrDefaultIncludeDelete(Expression<Func<T, bool>> expression);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllIncludeDelete();
        GenericPageModel<T> GetPage(int skip, int take, System.Linq.Expressions.Expression<Func<T, bool>>? fillter,
            System.Linq.Expressions.Expression<Func<T, string>>? orderby);
        List<T> GetUserFromObject(Expression<Func<T, bool>>? fillter);
        T GetById(Guid id);
        T GetByIdIncludeDelete(Guid id);
        void Restore(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Commit();
        void Dispose();
        void RollBack();
        void SaveChanges();
        bool SaveChangesBool();
    }
}
