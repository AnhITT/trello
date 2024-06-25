using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.IdentityModel.Tokens.Jwt;

namespace DataAccess_Layer.Data
{
    public class BaseContext : DbContext
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public BaseContext(DbContextOptions options) : base(options)
        {
            _contextAccessor = new HttpContextAccessor();
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public override int SaveChanges()
        {
            OnBeforeSaving();
            return base.SaveChanges();
        }
      
        private void OnBeforeSaving()
        {
            var userId = GetUserIdCurrent();
            IEnumerable<EntityEntry<ITrack>> entities = this.ChangeTracker.Entries<ITrack>();
            foreach (var entry in entities)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Property(a => a.CreatedDate).CurrentValue = DateTime.Now;
                        entry.Property(a => a.CreatedBy).CurrentValue = userId != null ? new Guid(userId) : (Guid?)null;
                        entry.Property(a => a.IsDeleted).CurrentValue = false;
                        break;
                    case EntityState.Modified:
                        entry.Property(a => a.CreatedDate).IsModified = false;
                        entry.Property(a => a.CreatedBy).IsModified = false;
                        entry.Property(a => a.ModifiedDate).CurrentValue = DateTime.Now;
                        entry.Property(a => a.ModifiedBy).CurrentValue = userId != null ? new Guid(userId) : (Guid?)null;
                        break;
                    case EntityState.Deleted:
                        if (entry.Entity is TrackableEntry trackableEntry)
                        {
                            entry.State = EntityState.Modified;
                            trackableEntry.IsDeleted = true;
                            trackableEntry.DeletedBy = userId != null ? new Guid(userId) : (Guid?)null;
                            trackableEntry.DeletedDate = DateTime.Now;
                        }
                        break;
                    default:
                        break;
                }
            }
        }
     
        #region Get UserCurrent
        public string GetAccessToken()
        {
            var context = _contextAccessor.HttpContext;

            if (context != null && context.Request.Headers.ContainsKey("Authorization"))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    return authHeader.Substring("Bearer ".Length).Trim();
                }
            }

            return null;
        }
        public string GetUserIdCurrent()
        {
            var jwt = GetAccessToken();
            if (jwt == null)
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            var userIdClaim = token.Claims.FirstOrDefault(claim => claim.Type == "Id");

            return userIdClaim?.Value;
        }
        #endregion
    }
}
