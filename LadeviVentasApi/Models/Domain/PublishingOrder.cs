using LadeviVentasApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using System.ComponentModel.DataAnnotations;

namespace LadeviVentasApi.Models.Domain
{
    public class PublishingOrder : BaseEntity
    {
        public ProductEdition? ProductEdition { get; set; }

        [Required, FkCheck(TypeToCheck = typeof(ProductEdition))]
        public long ProductEditionId { get; set; }

        public bool Latent { get; set; }

        public Client? Client { get; set; }

        [Required, FkCheck(TypeToCheck = typeof(Client))]
        public long ClientId { get; set; }

        public ApplicationUser? Seller { get; set; }

        [FkCheck(TypeToCheck = typeof(ApplicationUser))]
        public long SellerId { get; set; }

        public Contract? Contract { get; set; }
        [FkCheck(TypeToCheck = typeof(Contract))]
        public long? ContractId { get; set; }

        //Ubicacion
        public AdvertisingSpaceLocationType? AdvertisingSpaceLocationType { get; set; }

        [Required, FkCheck(TypeToCheck = typeof(AdvertisingSpaceLocationType))]
        public long AdvertisingSpaceLocationTypeId { get; set; }

        //Tipo de Espacio
        public ProductAdvertisingSpace? ProductAdvertisingSpace { get; set; }

        [Required, FkCheck(TypeToCheck = typeof(ProductAdvertisingSpace))]
        public long ProductAdvertisingSpaceId { get; set; }

        public string PageNumber { get; set; }

        public string InvoiceNumber { get; set; }

        public bool? PaidOut { get; set; }

        public double Quantity { get; set; }

        public string Observations { get; set; }

        public SoldSpace? SoldSpace { get; set; }

        [FkCheck(TypeToCheck = typeof(SoldSpace))]
        public long? SoldSpaceId { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? LastUpdate { get; set; }

        public bool CanDelete { get { return ProductEdition != null && !ProductEdition.Closed; } }

        public string? XubioDocumentNumber { get; set; }

        public long? XubioTransactionId { get; set; }

        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            return base.PerformValidate(validationContext, context, applicationUser)
                .Check(() => ValidateBusinessLogic(context, applicationUser.Value));
        }

        private ValidationResult ValidateBusinessLogic(ApplicationDbContext context, ApplicationUser user)
        {
            // ===== CARGA DE ENTIDADES =====
            var oldOP = context.PublishingOrders
                .Include(op => op.ProductEdition)
                .AsNoTracking()
                .SingleOrDefault(op => op.Id == Id);

            var currentProductEdition = context.ProductEditions
                .AsNoTracking()
                .Single(x => x.Id == ProductEditionId);

            var currentProduct = context.Products
                .Single(p => p.Id == currentProductEdition.ProductId);

            var currentClient = context.Clients
                .Single(c => c.Id == ClientId);

            var currentContract = context.Contracts
                .Include(c => c.BillingCondition)
                .Include(c => c.SoldSpaces)
                .ThenInclude(sp => sp.AdvertisingSpaceLocationType)
                .AsNoTracking()
                .SingleOrDefault(c => c.Id == ContractId);

            // ===== 1. VALIDACIONES DE PERMISOS DE USUARIO =====

            // BR: No se puede modificar  si la edición está abierta
            if (Id != 0 && currentProductEdition.Closed)
            {
                throw new ValidationExtensions.ValidationException(
                    "No se puede modificar cuando la edición está cerrada",
                    new[] { nameof(ProductEditionId) });
            }

            if (Id != 0 && (user.ApplicationRole.IsSeller() || user.ApplicationRole.IsSupervisor()))
            {
                if (!string.IsNullOrWhiteSpace(oldOP.PageNumber))
                {
                    throw new ValidationExtensions.ValidationException("No tiene permisos para modificar la OP ya que tiene numero de página", new[] { nameof(this.PageNumber) });
                }

                // BR: Vendedores y supervisores NO pueden modificar nro de página.
                if (oldOP.PageNumber != this.PageNumber)
                {
                    throw new ValidationExtensions.ValidationException("No tiene permisos para modificar el número de página", new[] { nameof(this.PageNumber) });
                }

                // BR: Vendedores y supervisores NO pueden modificar PaidOut ni InvoiceNumber en contratos Contra Publicación
                if (currentContract.BillingCondition.IsAgainstPublication())
                {
                    if (oldOP.PaidOut != this.PaidOut)
                    {
                        throw new ValidationExtensions.ValidationException("No tiene permisos para modificar si la OP fue pagada o no", new[] { nameof(this.PaidOut) });
                    }

                    if (oldOP.InvoiceNumber != this.InvoiceNumber)
                    {
                        throw new ValidationExtensions.ValidationException("No tiene permisos para modificar el número de factura", new[] { nameof(this.InvoiceNumber) });
                    }
                }
            }

            // BR: Vendedor nacional solo puede trabajar con productos de su país
            if (user.ApplicationRole.IsNationalSeller() && currentProduct.CountryId != user.CountryId)
            {
                throw new ValidationExtensions.ValidationException(
                    "Siendo vendedor nacional el producto tiene que ser de su mismo país",
                    new[] { nameof(currentProductEdition.ProductId) });
            }

            // BR: Vendedor solo puede trabajar con sus propios clientes
            if (user.ApplicationRole.IsSeller() && currentClient.ApplicationUserSellerId != user.Id)
            {
                throw new ValidationExtensions.ValidationException(
                    "El cliente que quiere asignar a la OP no es suyo",
                    new[] { nameof(ClientId) });
            }

            // ===== 2. VALIDACIONES DE REGLAS DE NEGOCIO GENERALES =====

            // BR: Relación entre estado Latente y Contrato
            if (Latent && ContractId > 0)
            {
                throw new ValidationExtensions.ValidationException(
                    "Si la orden de publicación se encuentra latente no puede estar asociada a un contrato",
                    new[] { nameof(ContractId) });
            }

            if (!Latent && ContractId == 0)
            {
                throw new ValidationExtensions.ValidationException(
                    "Si la orden de publicación no es latente debe estar asociada a un contrato",
                    new[] { nameof(ContractId) });
            }

            // BR: Creación solo permitida bajo condiciones específicas
            if (Id == 0 && currentProductEdition.Closed)
            {
                bool canCreateWhenClosed = currentContract != null &&
                    (currentContract.BillingCondition.IsAgainstPublication() ||
                     (currentContract.BillingCondition.IsAnticipated() &&
                      !string.IsNullOrWhiteSpace(currentContract.InvoiceNumber)));

                if (!canCreateWhenClosed)
                {
                    throw new ValidationExtensions.ValidationException(
                        "No se permite crear una OP luego de que la edición esté cerrada.",
                        new[] { nameof(ProductEditionId) });
                }
            }

            // ===== 3. VALIDACIONES ESPECÍFICAS DEL CONTRATO =====
            if (currentContract != null)
            {
                // Validar consistencia: Cliente del contrato
                if (ClientId != currentContract.ClientId)
                {
                    throw new ValidationExtensions.ValidationException(
                        "El cliente asignado tiene que ser el mismo cliente del contrato",
                        new[] { nameof(ClientId) });
                }

                // Validar consistencia: Producto del contrato
                if (currentProduct.Id != currentContract.ProductId)
                {
                    throw new ValidationExtensions.ValidationException(
                        "El producto asignado tiene que ser el mismo producto del contrato",
                        new[] { nameof(currentProductEdition.ProductId) });
                }

                // BR: Contrato no puede estar vencido
                if ((Id == 0 || (oldOP?.ContractId != ContractId && !user.ApplicationRole.IsSuperuser())) &&
                    currentContract.End < DateTime.Now)
                {
                    throw new ValidationExtensions.ValidationException(
                        "El contrato seleccionado está vencido",
                        new[] { nameof(ContractId) });
                }

                // BR: PaidOut solo aplicable para contratos Contra Publicación
                if (PaidOut.HasValue && PaidOut.Value && !currentContract.BillingCondition.IsAgainstPublication())
                {
                    throw new ValidationExtensions.ValidationException(
                        "Si la condición de facturación del contrato no es Contra Publicación no se puede especificar si fue pagada o no",
                        new[] { nameof(PaidOut) });
                }

                // ===== 4. VALIDACIONES DE CANTIDAD Y SALDO =====
                ValidateQuantityAndBalance(context, currentContract);
            }

            return ValidationResult.Success;
        }

        private void ValidateQuantityAndBalance(ApplicationDbContext context, Contract currentContract)
        {
            var currentSoldSpace = currentContract.SoldSpaces.Single(x => x.Id == SoldSpaceId);

            if (currentSoldSpace.AdvertisingSpaceLocationType.Name.Equals(AdvertisingSpaceLocationType.RotaryLocation))
            {
                // BR: Ubicación rotativa no puede exceder 50%
                ValidateRotaryLocationRule(context, currentContract, currentSoldSpace);
            }
            else
            {
                // BR: No exceder saldo disponible
                ValidateAvailableBalanceRule(context);
            }
        }

        private void ValidateRotaryLocationRule(ApplicationDbContext context, Contract currentContract, SoldSpace currentSoldSpace)
        {
            var oldOps = context.PublishingOrders
                .Where(x => x.ContractId == currentContract.Id
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
                    throw new ValidationExtensions.ValidationException(
                        "Si la ubicación del contrato es rotativa, la ubicación de la orden de publicación no puede exceder el 50%",
                        new[] { nameof(Quantity) });
                }
            }
            else
            {
                if (cantidad > Math.Truncate(currentSoldSpace.Quantity / 2) + 1)
                {
                    throw new ValidationExtensions.ValidationException(
                        "Si la ubicación del contrato es rotativa, la ubicación de la orden de publicación no puede exceder el 50%",
                        new[] { nameof(Quantity) });
                }
            }
        }

        private void ValidateAvailableBalanceRule(ApplicationDbContext context)
        {
            // Obtengo la cantidad en el contrato para ese tipo de espacio - ubicación
            var soldSpace = context.SoldSpaces.SingleOrDefault(sp => sp.Id == SoldSpaceId);

            if (soldSpace == null)
            {
                throw new ValidationExtensions.ValidationException(
                    "No se encontró el espacio vendido especificado",
                    new[] { nameof(SoldSpaceId) });
            }

            // Obtengo las OP ya existentes para ese contrato - tipo de espacio - ubicación
            var ops = context.PublishingOrders
                .Where(x => x.ContractId == ContractId
                    && (!x.Deleted.HasValue || !x.Deleted.Value)
                    && x.ProductAdvertisingSpaceId == ProductAdvertisingSpaceId
                    && x.AdvertisingSpaceLocationTypeId == AdvertisingSpaceLocationTypeId
                    && x.Id != Id
                    && x.SoldSpaceId == SoldSpaceId)
                .ToList();

            // Cálculo del saldo
            double availableBalance = soldSpace.Quantity - ops.Sum(x => x.Quantity);

            if (availableBalance < Quantity)
            {
                throw new ValidationExtensions.ValidationException(
                    $"No hay saldo. Saldo disponible: {availableBalance}",
                    new[] { nameof(Quantity) });
            }
        }
    }
}
