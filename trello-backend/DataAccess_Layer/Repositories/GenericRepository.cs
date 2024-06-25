using DataAccess_Layer.DTOs;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace DataAccess_Layer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _context;
        private IDbContextTransaction _transaction;
        public GenericRepository(DbContext dbContext) {
            _context = dbContext;
        }
        public DbSet<T> Context()
        {
            return _context.Set<T>();
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
        }

        public IEnumerable<T> FindIncludeDelete(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            var parameter = expression.Parameters.First();
            var property = typeof(T).GetProperty("IsDeleted");

            if (property != null)
            {
                var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
                var isDeletedFalse = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                var combinedExpression = Expression.AndAlso(expression.Body, isDeletedFalse);
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

                return _context.Set<T>().Where(lambda);
            }

            return _context.Set<T>().Where(expression);
        }



        public IEnumerable<TResult> SelectIncludeDelete<TResult>(Expression<Func<T, TResult>> selector)
        {
            return _context.Set<T>().Select(selector).ToList();
        }

        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector)
        {
            return _context.Set<T>()
                           .Where(e => EF.Property<bool>(e, "IsDeleted") == false)
                           .Select(selector)
                           .ToList();
        }

        public T FirstOrDefault(System.Linq.Expressions.Expression<Func<T, bool>> expression)
        {
            var parameter = expression.Parameters[0];
            var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
            var isDeletedFalse = Expression.Equal(isDeletedProperty, Expression.Constant(false));

            var combinedExpression = Expression.AndAlso(expression.Body, isDeletedFalse);
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

            return _context.Set<T>().FirstOrDefault(lambda);
        }

        public T FirstOrDefaultIncludeDelete(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().FirstOrDefault(expression);
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().Where(e => EF.Property<bool>(e, "IsDeleted") == false).ToList();
        }

        public IEnumerable<T> GetAllIncludeDelete()
        {
            return _context.Set<T>().ToList();
        }

        public GenericPageModel<T> GetPage(int skip,int take, Expression<Func<T, bool>>? fillter, Expression<Func<T, string>>?  orderBy)
        {
            Expression<Func<T, bool>> lambdaFillter = c => true;
            Expression<Func<T, string>> lambdaOrderBy = c => "";

            if (fillter == null)
                fillter = lambdaFillter;

            if (orderBy == null)
                orderBy = lambdaOrderBy;

            GenericPageModel<T> result = new GenericPageModel<T>();
            result.Items = _context.Set<T>().Where(fillter).OrderBy(orderBy.Compile()).Skip(skip).Take(take).ToList();
            result.TotalCount= result.Items.Count();
            return result;
        }

        public List<T> GetUserFromObject(Expression<Func<T, bool>>? fillter)
        {
            Expression<Func<T, bool>> lambdaFillter = c => true;

            if (fillter == null)
                fillter = lambdaFillter;

            List<T> result = new List<T>();
            result = _context.Set<T>().Where(fillter).ToList();
            return result;
        }

        public T GetById(Guid id)
        {
            return _context.Set<T>().Find(id);
        }

        public T GetByIdIncludeDelete(Guid id)
        {
            return _context.Set<T>()
                           .FirstOrDefault(e => EF.Property<Guid>(e, "Id") == id && EF.Property<bool>(e, "IsDeleted") == false);
        }
        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }
        public void Restore(T entity)
        {
            var trackableEntity = entity as TrackableEntry;
            if (trackableEntity != null)
            {
                trackableEntity.IsDeleted = false;
                trackableEntity.DeletedBy = null;
                trackableEntity.DeletedDate = null;
                _context.Set<T>().Update(entity);
            }
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public void RollBack()
        {
            _transaction.Rollback();
            Dispose();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public bool SaveChangesBool()
        {
            try
            {
                _context.SaveChanges();
                return true;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }
    }
}
