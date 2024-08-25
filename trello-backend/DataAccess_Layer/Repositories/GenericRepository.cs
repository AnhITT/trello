using DataAccess_Layer.DTOs;
using DataAccess_Layer.Helpers;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace DataAccess_Layer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly DbContext _context;
        private IDbContextTransaction _transaction;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GenericRepository(DbContext dbContext, IHttpContextAccessor httpContextAccessor) {
            _context = dbContext;
            _httpContextAccessor = httpContextAccessor;
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
        public IEnumerable<T> GetAll(bool includeDeleted = false)
        {
            var property = typeof(T).GetProperty("IsDeleted");

            if (property != null && !includeDeleted)
            {
                return _context.Set<T>().Where(e => EF.Property<bool>(e, "IsDeleted") == false).ToList();
            }

            return _context.Set<T>().ToList();
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression, bool includeDeleted = false)
        {
            var parameter = expression.Parameters.First();
            var property = typeof(T).GetProperty("IsDeleted");

            if (property != null && !includeDeleted)
            {
                var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
                var isDeletedFalse = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                var combinedExpression = Expression.AndAlso(expression.Body, isDeletedFalse);
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

                return _context.Set<T>().Where(lambda);
            }

            return _context.Set<T>().Where(expression);
        }
        public IEnumerable<TResult> Select<TResult>(Expression<Func<T, TResult>> selector, bool includeDeleted = false)
        {
            var property = typeof(T).GetProperty("IsDeleted");

            if (property != null && !includeDeleted)
            {
                return _context.Set<T>()
                               .Where(e => EF.Property<bool>(e, "IsDeleted") == false)
                               .Select(selector)
                               .ToList();
            }

            return _context.Set<T>().Select(selector).ToList();
        }
        public T FirstOrDefault(Expression<Func<T, bool>> predicate, bool includeDeleted = false)
        {
            var property = typeof(T).GetProperty("IsDeleted");

            if (property != null && !includeDeleted)
            {
                var parameter = predicate.Parameters[0];
                var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
                var isDeletedFalse = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                var combinedExpression = Expression.AndAlso(predicate.Body, isDeletedFalse);
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

                return _context.Set<T>().FirstOrDefault(lambda);
            }

            return _context.Set<T>().FirstOrDefault(predicate);
        }
        public T GetById(object id, bool includeDeleted = false)
        {
            if (includeDeleted)
            {
                return _context.Set<T>().Find(id);
            }
            else
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var idProperty = Expression.Property(parameter, "Id");
                var idValue = Expression.Constant(id);
                var idEqual = Expression.Equal(idProperty, idValue);

                var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
                var isDeletedFalse = Expression.Equal(isDeletedProperty, Expression.Constant(false));

                var combinedExpression = Expression.AndAlso(idEqual, isDeletedFalse);
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);

                return _context.Set<T>().FirstOrDefault(lambda);
            }
        }

        public GenericPageModel<T> GetPage(int skip, int take,
                                        Expression<Func<T, bool>>? filter = null,
                                        Expression<Func<T, string>>? orderBy = null,
                                        bool includeDeleted = false)
        {
            var property = typeof(T).GetProperty("IsDeleted");
            Expression<Func<T, bool>> defaultFilter = c => true;
            Expression<Func<T, string>> defaultOrderBy = c => "";

            filter = filter ?? defaultFilter;
            orderBy = orderBy ?? defaultOrderBy;

            var query = _context.Set<T>().AsQueryable();
            if (property != null && !includeDeleted)
            {
                var parameter = filter.Parameters[0];
                var isDeletedProperty = Expression.Property(parameter, "IsDeleted");
                var isDeletedFalse = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                var combinedFilter = Expression.AndAlso(filter.Body, isDeletedFalse);
                var lambdaFilter = Expression.Lambda<Func<T, bool>>(combinedFilter, parameter);

                query = query.Where(lambdaFilter);
            }
            else
            {
                query = query.Where(filter);
            }

            var orderedQuery = query.OrderBy(orderBy.Compile());
            var pagedQuery = orderedQuery.Skip(skip).Take(take);

            GenericPageModel<T> result = new GenericPageModel<T>
            {
                Items = pagedQuery.ToList(),
                TotalCount = query.Count()
            };

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
        public void Remove(T entity, bool hardDelete = false)
        {
            var property = typeof(T).GetProperty("IsDeleted");

            if (hardDelete || property == null)
            {
                _context.Set<T>().Remove(entity);
            }
            else
            {
                var trackableEntity = entity as TrackableEntry;
                if (trackableEntity != null)
                {
                    trackableEntity.IsDeleted = true;
                    trackableEntity.DeletedBy = _httpContextAccessor.HttpContext?.GetCurrentUserId();
                    trackableEntity.DeletedDate = DateTime.UtcNow;
                    _context.Set<T>().Update(entity);
                }
            }
        }
        public void RemoveRange(IEnumerable<T> entities, bool hardDelete = false)
        {
            var property = typeof(T).GetProperty("IsDeleted");

            if (hardDelete || property == null)
            {
                _context.Set<T>().RemoveRange(entities);
            }
            else
            {
                foreach (var entity in entities)
                {
                    var trackableEntity = entity as TrackableEntry;
                    if (trackableEntity != null)
                    {
                        trackableEntity.IsDeleted = true;
                        trackableEntity.DeletedBy = _httpContextAccessor.HttpContext?.GetCurrentUserId();
                        trackableEntity.DeletedDate = DateTime.UtcNow;
                        _context.Set<T>().Update(entity);
                    }
                }
            }
        }
        public void Restore(T entity)
        {
            var property = typeof(T).GetProperty("IsDeleted");

            if (property != null)
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
            catch (Exception ex)
            {
                return false;
            }
        }
        #region transaction
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
        #endregion
    }
}
