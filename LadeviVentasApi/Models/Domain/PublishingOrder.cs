using LadeviVentasApi.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class PublishingOrder : BaseEntity
    {
        /* public Product Product { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Product))] public long ProductId { get; set; } */
        public ProductEdition? ProductEdition { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(ProductEdition))] public long ProductEditionId { get; set; }
        public bool Latent { get; set; }
        public Client? Client { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Client))] public long ClientId { get; set; }
        public ApplicationUser? Seller { get; set; }
        [FkCheck(TypeToCheck = typeof(ApplicationUser))] public long SellerId { get; set; }
        public Contract? Contract { get; set; }
        [FkCheck(TypeToCheck = typeof(Contract))] public long? ContractId { get; set; }
        //Ubicacion
        public AdvertisingSpaceLocationType? AdvertisingSpaceLocationType { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(AdvertisingSpaceLocationType))] public long AdvertisingSpaceLocationTypeId { get; set; }
        //Tipo de Espacio
        public ProductAdvertisingSpace? ProductAdvertisingSpace { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(ProductAdvertisingSpace))] public long ProductAdvertisingSpaceId { get; set; }
        public string PageNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public bool? PaidOut { get; set; }
        public double Quantity { get; set; }
        public string Observations { get; set; }
        public SoldSpace? SoldSpace { get; set; }
        [FkCheck(TypeToCheck = typeof(SoldSpace))] public long? SoldSpaceId { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? LastUpdate { get; set; }
        public bool CanDelete { get { return ProductEdition != null && !ProductEdition.Closed; } }
        public string? XubioDocumentNumber { get; set; }
        public long? XubioTransactionId { get; set; }

        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .Check(() => ValidOP(context, applicationUser.Value));
        }

        private ValidationResult ValidOP(ApplicationDbContext context, ApplicationUser user)
        {
            var oldOP = context.PublishingOrders.Include(op => op.ProductEdition).AsNoTracking().SingleOrDefault(op => op.Id == Id);
            var currentProductEdition = context.ProductEditions.Single(x => x.Id == ProductEditionId);
            var currentProduct = context.Products.Single(p => p.Id == currentProductEdition.ProductId);
            var currentClient = context.Clients.Single(c => c.Id == ClientId);
            var currentContract = context.Contracts
                .Include(c => c.BillingCondition)
                .Include(c => c.SoldSpaces)
                    .ThenInclude(sp => sp.AdvertisingSpaceLocationType)
                .SingleOrDefault(c => c.Id == ContractId);

            if (Id != 0 && oldOP.ProductEdition.Closed && !user.ApplicationRole.IsSuperuser() && !user.ApplicationRole.IsSupervisor())
            {
                throw new ValidationExtensions.ValidationException("El rol asignado no permite modificar la OP luego de que la edicion este cerrada.", new[] { nameof(ProductEdition.ProductId) });
            }

            if (Id != 0 && !user.ApplicationRole.IsSuperuser() && !user.ApplicationRole.IsSupervisor() && oldOP.ProductEdition.Closed
                && (ProductEditionId != oldOP.ProductEditionId || Latent != oldOP.Latent || Observations != oldOP.Observations
                    || AdvertisingSpaceLocationTypeId != oldOP.AdvertisingSpaceLocationTypeId || ClientId != oldOP.ClientId || Quantity != oldOP.Quantity
                    || ProductAdvertisingSpaceId != oldOP.ProductAdvertisingSpaceId || ContractId != oldOP.ContractId || SoldSpaceId != oldOP.SoldSpaceId
                    || SellerId != oldOP.SellerId))
            {
                throw new ValidationExtensions.ValidationException("El rol asignado solo permite modificar el numero de factura y / o pagina luego del cierre de la edicion.", new[] { nameof(ProductEdition.ProductId) });
            }

            if (user.ApplicationRole.IsNationalSeller() && currentProduct.CountryId != user.CountryId)
            {
                throw new ValidationExtensions.ValidationException("Siendo vendedor nacional el producto tiene que ser de su mismo pais", new[] { nameof(ProductEdition.ProductId) });
            }

            if (!user.ApplicationRole.IsSuperuser() && !user.ApplicationRole.IsSupervisor() && currentProductEdition.Closed)
            {
                throw new ValidationExtensions.ValidationException("La edición del producto se encuentra cerrada", new[] { nameof(ProductEditionId) });
            }

            if (Latent && ContractId > 0)
            {
                throw new ValidationExtensions.ValidationException("Si la orden de publicación se encuentra latente no puede estar asociada a un contrato", new[] { nameof(Latent), nameof(ContractId) });
            }

            if (!Latent && ContractId == 0)
            {
                throw new ValidationExtensions.ValidationException("Si la orden de publicación no es latente debe estar asociada a un contrato", new[] { nameof(Latent), nameof(ContractId) });
            }

            if (user.ApplicationRole.IsSeller() && currentClient.ApplicationUserSellerId != user.Id)
            {
                throw new ValidationExtensions.ValidationException("Si es vendedor el vendedor asignado del cliente tiene que ser usted mismo", new[] { nameof(ClientId) });
            }

            if (currentContract != null)
            {
                var currentSoldSpace = currentContract.SoldSpaces.Single(x => x.Id == SoldSpaceId);

                if (currentSoldSpace.AdvertisingSpaceLocationType.Name.Equals(AdvertisingSpaceLocationType.RotaryLocation))
                {
                    var oldOps = context.PublishingOrders.Where(x => x.ContractId == currentContract.Id
                                && x.AdvertisingSpaceLocationTypeId == AdvertisingSpaceLocationTypeId
                                && x.SoldSpaceId == currentSoldSpace.Id
                                && x.Id != Id
                                && (!x.Deleted.HasValue || !x.Deleted.Value))
                            .ToList();

                    double cantidad = oldOps.Sum(x => x.Quantity) + Quantity;
                    bool isPairQuantity = currentSoldSpace.Quantity % 2 == 0;

                    if (isPairQuantity)
                    {
                        if (cantidad / currentSoldSpace.Quantity > 0.5)
                        {
                            throw new ValidationExtensions.ValidationException("Si la ubicación del contrato es rotativa, la ubicación de la orden de publicación no puede exceder el 50%", new[] { nameof(Quantity) });
                        }
                    }
                    else
                    {
                        if (cantidad > Math.Truncate(currentSoldSpace.Quantity / 2) + 1)
                        {
                            throw new ValidationExtensions.ValidationException("Si la ubicación del contrato es rotativa, la ubicación de la orden de publicación no puede exceder el 50%", new[] { nameof(Quantity) });
                        }
                    }
                }
                else
                {
                    //Calculo del saldo

                    //obtengo la cantidad en el contrato para ese tipo de espacio - ubicacion

                    /*var soldSpace = context.SoldSpaces.SingleOrDefault(sp => sp.ProductAdvertisingSpaceId == ProductAdvertisingSpaceId
                                        && AdvertisingSpaceLocationTypeId == sp.AdvertisingSpaceLocationTypeId
                                        && sp.ContractId == ContractId); */
                    var soldSpace = context.SoldSpaces.SingleOrDefault(sp => sp.Id == SoldSpaceId);

                    //Obtengo las OP ya existentes para ese contrato - tipo de espacio - ubicacion
                    var ops = context.PublishingOrders.Where(x => x.ContractId == ContractId
                                                        && (!x.Deleted.HasValue && !x.Deleted.Value)
                                                        && x.ProductAdvertisingSpaceId == ProductAdvertisingSpaceId
                                                        && x.AdvertisingSpaceLocationTypeId == AdvertisingSpaceLocationTypeId
                                                        && x.Id != Id
                                                        && x.SoldSpaceId == SoldSpaceId
                                                        ).ToList();

                    if (soldSpace.Quantity < ops.Sum(x => x.Quantity) + Quantity)
                    {
                        throw new ValidationExtensions.ValidationException("No hay saldo. Saldo disponible: " + (soldSpace.Quantity - ops.Sum(x => x.Quantity)).ToString(), new[] { nameof(Quantity) });
                    }
                }

                if (ClientId != currentContract.ClientId)
                {
                    throw new ValidationExtensions.ValidationException("El cliente asignado tiene que ser el mismo cliente del contrato", new[] { nameof(ClientId) });
                }

                if (currentProduct.Id != currentContract.ProductId)
                {
                    throw new ValidationExtensions.ValidationException("El producto asignado tiene que ser el mismo producto del contrato", new[] { nameof(currentProduct.Id) });
                }

                if ((Id == 0 || (oldOP.ContractId != ContractId && !user.ApplicationRole.IsSuperuser())) && currentContract.End < DateTime.Now)
                {
                    throw new ValidationExtensions.ValidationException("El contrato seleccionado esta vencido", new[] { nameof(ContractId) });
                }

                /* if (currentContract.BillingCondition.IsAnticipated())
                {
                    if (!InvoiceNumber.Equals(currentContract.InvoiceNumber))
                    {
                        throw new ValidationExtensions.ValidationException("Si la condición de facturacion del contrato es anticipada, el número de factura debe ser igual a la del contrato", new[] { nameof(InvoiceNumber) });
                    }
                } */

                if (PaidOut.HasValue && PaidOut.Value && !currentContract.BillingCondition.IsAgainstPublication())
                {
                    throw new ValidationExtensions.ValidationException("Si la condición de facturación del contrato no es Contra Publicación no se puede especificar si fue pagada o no", new[] { nameof(PaidOut) });
                }
            }

            //if(currentProductEdition.End < DateTime.Now && !String.IsNullOrEmpty(PageNumber))
            //{
            //    throw new ValidationExtensions.ValidationException("El número de página solo se puede cargar una vez haya sido publicada la edicion", new[] { nameof(PageNumber) });
            //}

            return ValidationResult.Success;
        }
    }
}
