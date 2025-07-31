using LadeviVentasApi.Data;
using LadeviVentasApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LadeviVentasApi.UseCases
{
    public static class PublishingOrdersUC
    {
        public static void UpdateContractBalance(long soldSpaceId, ApplicationDbContext context)
        {
            var soldSpace = context.SoldSpaces.Single(x => x.Id == soldSpaceId);
            var ops = context.PublishingOrders.Where(x => x.SoldSpaceId == soldSpaceId && (!x.Deleted.HasValue || !x.Deleted.Value)).ToList();

            var balance = soldSpace.Quantity - ops.Sum(x => x.Quantity);
            if (balance >= 0)
            {
                soldSpace.Balance = soldSpace.Quantity - ops.Sum(x => x.Quantity);
                context.ChangeTracker.TrackGraph(soldSpace, e =>
                {
                    e.Entry.State = e.Entry.IsKeySet
                        ? e.Entry.Entity is BaseEntity o && (o.ShouldDelete ?? false)
                            ? EntityState.Deleted
                            : EntityState.Modified
                        : EntityState.Added;
                });
                context.SaveChanges();
            }
            else
            {
                throw new ValidationExtensions.ValidationException("No es posible generar la orden de publiación ya que el espacio no tiene saldo", new[] { "Quantity" });
            }
        }
    }
}
