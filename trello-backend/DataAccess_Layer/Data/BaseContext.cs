using DataAccess_Layer.Helpers;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;

namespace DataAccess_Layer.Data
{
    public class BaseContext : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BaseContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }

        private void OnBeforeSaving()
        {
            var userId = _httpContextAccessor.HttpContext?.GetCurrentUserId();

            IEnumerable<EntityEntry<ITrack>> entities = this.ChangeTracker.Entries<ITrack>();
            foreach (var entry in entities)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property(a => a.CreatedDate).CurrentValue = DateTime.Now;
                        entry.Property(a => a.CreatedBy).CurrentValue = userId != Guid.Empty ? userId : (Guid?)null;
                        entry.Property(a => a.IsDeleted).CurrentValue = false;
                        break;
                    case EntityState.Modified:
                        entry.Property(a => a.CreatedDate).IsModified = false;
                        entry.Property(a => a.CreatedBy).IsModified = false;
                        entry.Property(a => a.ModifiedDate).CurrentValue = DateTime.Now;
                        entry.Property(a => a.ModifiedBy).CurrentValue = userId != Guid.Empty ? userId : (Guid?)null;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
