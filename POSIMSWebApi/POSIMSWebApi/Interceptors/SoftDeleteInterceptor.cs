﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using POSIMSWebApi.Interfaces;

namespace POSIMSWebApi.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result
        )
        {
            if (eventData.Context == null)
                return result;

            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete delete)
                {
                    entry.State = EntityState.Modified;
                    delete.IsDeleted = true;
                    delete.DeletedAt = DateTimeOffset.UtcNow;
                }
            }

            return base.SavingChanges(eventData, result);
        }
    }
}
