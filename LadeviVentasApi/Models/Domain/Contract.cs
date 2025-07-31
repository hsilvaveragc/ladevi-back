using System.ComponentModel.DataAnnotations;
using LadeviVentasApi.Data;
using Microsoft.EntityFrameworkCore;

namespace LadeviVentasApi.Models.Domain
{
    public class Contract : BaseEntity
    {
        [Required] public long Number { get; set; }
        [Required] public string Name { get; set; }
        public BillingCondition? BillingCondition { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(BillingCondition))] public long BillingConditionId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(PaymentMethod))] public long PaymentMethodId { get; set; }

        public Currency? Currency { get; set; }
        [FkCheck(TypeToCheck = typeof(Currency))] public long? CurrencyId { get; set; }
        public bool UseEuro { get; set; }
        public Country? BillingCountry { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Country))] public long BillingCountryId { get; set; }
        public string InvoiceNumber { get; set; }
        public bool? PaidOut { get; set; }
        public Product? Product { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Product))] public long ProductId { get; set; }

        public Client? Client { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(Client))] public long ClientId { get; set; }

        public ApplicationUser? Seller { get; set; }
        [Required, FkCheck(TypeToCheck = typeof(ApplicationUser))] public long SellerId { get; set; }

        [Required] public DateTime Start { get; set; }
        [Required] public DateTime End { get; set; }
        [Required] public DateTime ContractDate { get; set; }

        public bool ApplyDiscountForCheck { get; set; }
        public bool ApplyDiscountForLoyalty { get; set; }
        public bool ApplyDiscountForSameCountry { get; set; }
        public bool ApplyDiscountForOtherCountry { get; set; }
        public bool AppyDiscountForAgency { get; set; }
        public bool ApplyDiscountForVolume { get; set; }
        public double? DiscountForCheck { get; set; }
        public double? DiscountForLoyalty { get; set; }
        public double? DiscountForAgency { get; set; }
        public double? DiscountForSameCountry { get; set; }
        public double? DiscountForOtherCountry { get; set; }
        public double? DiscountForVolume { get; set; }
        public int? CheckQuantity { get; set; }
        public int? DaysToFirstPayment { get; set; }
        public int? DaysBetweenChecks { get; set; }
        public double TotalDiscounts { get; set; }
        public double TotalTaxes { get; set; }
        public double Total { get; set; }
        public string Observations { get; set; }
        public double? CurrencyParity { get; set; }
        public double? IVA { get; set; }

        [MustHaveOne] public ICollection<SoldSpace> SoldSpaces { get; set; }
        public ICollection<PublishingOrder>? PublishingOrders { get; set; }
        public ICollection<CheckPayment>? CheckPayments { get; set; }
        public ICollection<ContractHistorical>? ContractHistoricals { get; set; }

        protected override IList<ValidationResult> PerformValidate(ValidationContext validationContext, ApplicationDbContext context, Lazy<ApplicationUser> applicationUser)
        {
            var currentClient = context.Clients.Include(c => c.City.District.State).Single(x => x.Id == ClientId);
            var currentProduct = context.Products.Include(p => p.ProductLocationDiscounts).Single(x => x.Id == ProductId);
            var productAdvertisingSpaces = context.ProductAdvertisingSpaces.Where(x => x.ProductId == currentProduct.Id).ToList();

            return base.PerformValidate(validationContext, context, applicationUser)
                .Check(() => SellerValidations(applicationUser.Value, currentClient, currentProduct))
                .Check(() => SoldSpacesValidations(applicationUser.Value, currentProduct, productAdvertisingSpaces))
                .Check(() => GeneralValidations(context, applicationUser.Value, currentClient, currentProduct))
                .Check(() => EditValidations(context, applicationUser.Value))
                ;
        }

        private ValidationResult IsValidSeller(ApplicationDbContext context)
        {
            var seller = context.ApplicationUsers.Include(u => u.ApplicationRole).FirstOrDefault(u => u.Id == SellerId);
            return seller != null && seller.ApplicationRole.IsSeller()
                ? ValidationResult.Success
                : new ValidationResult("El usuario asignado como vendedor no es Vendedor!", new[] { nameof(SellerId) });
        }

        private ValidationResult SellerValidations(ApplicationUser user, Client currentClient, Product currentProduct)
        {
            if (user.ApplicationRole.IsSeller())
            {
                if (currentClient.ApplicationUserSellerId != user.Id)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor el vendedor asignado tiene que ser usted mismo", new[] { nameof(currentClient.ApplicationUserSellerId) });
                }

                if (Math.Truncate(End.Subtract(Start).TotalDays) > 365)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor la duración del contrato no puede ser mayor a un año", new[] { nameof(End) });
                }

                if (!string.IsNullOrEmpty(InvoiceNumber))
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede cargar el número de factura", new[] { nameof(BillingConditionId) });
                }
            }

            if (user.ApplicationRole.IsNationalSeller())
            {
                if (currentProduct.CountryId != user.CountryId)
                {
                    throw new ValidationExtensions.ValidationException("No coincide el pais autorizado para el usuario con el pais del producto", new[] { nameof(currentProduct.CountryId) });
                }
            }

            return ValidationResult.Success;
        }

        private ValidationResult EditValidations(ApplicationDbContext context, ApplicationUser user)
        {
            if (Id == 0)
                return ValidationResult.Success;

            var currentContract = context.Contracts.Include(c => c.PublishingOrders).FirstOrDefault(x => x.Id == Id);

            if (user.ApplicationRole.IsSeller())
            {
                if (currentContract.ClientId != ClientId)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar el cliente", new[] { nameof(ClientId) });
                }

                if ((!string.IsNullOrWhiteSpace(currentContract.InvoiceNumber) || currentContract.PublishingOrders.Where(po => po.Deleted.HasValue && !po.Deleted.Value).Count() > 0) && currentContract.Name != Name)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar el nombre", new[] { nameof(Name) });
                }

                if (currentContract.ProductId != ProductId)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar el producto", new[] { nameof(ProductId) });
                }

                if (currentContract.Start != Start)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar la fecha de inicio", new[] { nameof(Start) });
                }

                if (currentContract.End != End)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar la fecha de fin", new[] { nameof(End) });
                }

                if (currentContract.BillingConditionId != BillingConditionId)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar la condición de facturación", new[] { nameof(BillingConditionId) });
                }

                if (currentContract.PaymentMethodId != PaymentMethodId)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar el metodo de pago", new[] { nameof(PaymentMethodId) });
                }

                if (currentContract.CurrencyId != CurrencyId)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar la moneda", new[] { nameof(CurrencyId) });
                }

                if (currentContract.CheckQuantity != CheckQuantity)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar la cantidad de cheques", new[] { nameof(CheckQuantity) });
                }

                if (currentContract.DaysToFirstPayment != DaysToFirstPayment)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar los dias al primer pago", new[] { nameof(DaysToFirstPayment) });
                }

                if (currentContract.DaysBetweenChecks != DaysBetweenChecks)
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar la cantidad de dias entre cheques", new[] { nameof(DaysToFirstPayment) });
                }
            }
            return ValidationResult.Success;
        }

        private ValidationResult SoldSpacesValidations(ApplicationUser user, Product currentProduct, List<ProductAdvertisingSpace> productAdvertisingSpaces)
        {
            for (int i = 0; i < SoldSpaces.Count; i++)
            {
                SoldSpace sp = SoldSpaces.ToList()[i];
                var productAdvertisingSpace = productAdvertisingSpaces.SingleOrDefault(x => x.Id == sp.ProductAdvertisingSpaceId);

                if (user.ApplicationRole.IsSeller() && sp.Id != 0)
                {
                    if (sp.ApplyDiscountForCheck && productAdvertisingSpace.DiscountForCheck != sp.DiscountForCheck)
                    {
                        throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar el descuento por cheques", new[] { nameof(DiscountForCheck) });
                    }

                    if (sp.ApplyDiscountForLoyalty && productAdvertisingSpace.DiscountForLoyalty != sp.DiscountForLoyalty)
                    {
                        throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar el descuento por lealtad", new[] { nameof(DiscountForCheck) });
                    }

                    if (sp.AppyDiscountForAgency && productAdvertisingSpace.DiscountForAgency != sp.DiscountForAgency)
                    {
                        throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar el descuento por agencia", new[] { nameof(DiscountForAgency) });
                    }

                    if (sp.ApplyDiscountForSameCountry && productAdvertisingSpace.DiscountForSameCountry != sp.DiscountForSameCountry)
                    {
                        throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar el descuento por cliente nacional", new[] { nameof(DiscountForSameCountry) });
                    }

                    if (sp.ApplyDiscountForOtherCountry && productAdvertisingSpace.DiscountForOtherCountry != sp.DiscountForOtherCountry)
                    {
                        throw new ValidationExtensions.ValidationException("Siendo vendedor no puede modificar el descuento por cliente internacional", new[] { nameof(DiscountForOtherCountry) });
                    }
                }

                if (sp.Id != 0)
                {
                    if (sp.ApplyDiscountForCheck && productAdvertisingSpace.DiscountForCheck != sp.DiscountForCheck)
                    {
                        throw new ValidationExtensions.ValidationException("El descuento de pago por cheques solo se puede modificar en la creación del contrato", new[] { nameof(DiscountForCheck) });
                    }

                    if (sp.ApplyDiscountForSameCountry && productAdvertisingSpace.DiscountForSameCountry != sp.DiscountForSameCountry)
                    {
                        throw new ValidationExtensions.ValidationException("El descuento por cliente nacional solo se puede modificar en la creación del contrato", new[] { nameof(DiscountForSameCountry) });
                    }

                    if (sp.ApplyDiscountForLoyalty && productAdvertisingSpace.DiscountForLoyalty != sp.DiscountForLoyalty)
                    {
                        throw new ValidationExtensions.ValidationException("El descuento de pago por fidelización solo se puede modificar en la creación del contrato", new[] { nameof(DiscountForLoyalty) });
                    }

                    if (sp.AppyDiscountForAgency && productAdvertisingSpace.DiscountForAgency != sp.DiscountForAgency)
                    {
                        throw new ValidationExtensions.ValidationException("El descuento de pago por agencia solo se puede modificar en la creación del contrato", new[] { nameof(DiscountForAgency) });
                    }

                    if (sp.ApplyDiscountForOtherCountry && productAdvertisingSpace.DiscountForOtherCountry != sp.DiscountForOtherCountry)
                    {
                        throw new ValidationExtensions.ValidationException("El descuento por cliente internacional solo se puede modificar en la creación del contrato", new[] { nameof(DiscountForOtherCountry) });
                    }
                }

                if (sp.TypeSpecialDiscount == 1 && sp.SpecialDiscount > currentProduct.MaxAplicableDiscount)
                {
                    throw new ValidationExtensions.ValidationException("La cantidad máxima de descuentos es de " + currentProduct.MaxAplicableDiscount.ToString() + "%", new[] { nameof(TotalDiscounts) });
                }

                if (SoldSpaces.ToList()[i].GerentialDiscount > 0 && user.ApplicationRole.IsSeller())
                {
                    throw new ValidationExtensions.ValidationException("Siendo vendedor no puede cargar descuentos gerenciales", new[] { nameof(sp.GerentialDiscount) });
                }

                if (SoldSpaces.ToList()[i].Balance < 0)
                {
                    throw new ValidationExtensions.ValidationException("La cantidad no puede ser menor a la cantidad de órdenes publicadas", new[] { "soldSpaces." + i.ToString() + "." + nameof(sp.Quantity).ToLower() });
                }

                //TODO: Agregar validaciones de totales
            }

            return ValidationResult.Success;
        }

        private ValidationResult GeneralValidations(ApplicationDbContext context, ApplicationUser user, Client currentClient, Product currentProduct)
        {
            /*double alicuota = TotalDiscounts * 100 / (Total - TotalTaxes);

            if (alicuota > currentProduct.MaxAplicableDiscount)
            {
                throw new ValidationExtensions.ValidationException("La cantidad máxima de descuentos es de " + currentProduct.MaxAplicableDiscount.ToString() + "%", new[] { nameof(TotalDiscounts) });
            }*/

            if (Start > End)
            {
                throw new ValidationExtensions.ValidationException("La fecha de inicio no puede ser mayor a la fecha de fin", new[] { nameof(Start), nameof(End) });
            }

            var currentBillingCondition = context.BillingConditions.Single(x => x.Id == BillingConditionId);
            if (!currentBillingCondition.IsAnticipated() && !string.IsNullOrEmpty(InvoiceNumber))
            {
                throw new ValidationExtensions.ValidationException("Si la condición de facturación es Canje o Sin Cargo no puede tener factura", new[] { nameof(InvoiceNumber) });
            }

            var currentPaymentMethod = context.PaymentMethods.Single(x => x.Id == PaymentMethodId);
            if (!currentPaymentMethod.IsDocumented() && (CheckQuantity > 0 || DaysToFirstPayment > 0 || DaysBetweenChecks > 0))
            {
                throw new ValidationExtensions.ValidationException("Si el metodo de pago no es documentado no se pueden cargar cheques", new[] { nameof(PaymentMethodId) });
            }


            if (!currentClient.IsComtur && currentClient.CountryId != BillingCountryId)
            {
                throw new ValidationExtensions.ValidationException("Si el cliente no es Comtur solo puede facturarse en su pais", new[] { nameof(BillingCountryId) });
            }

            if (PaidOut.HasValue && PaidOut.Value && string.IsNullOrEmpty(InvoiceNumber))
            {
                throw new ValidationExtensions.ValidationException("Si no se ingresó el número de factura no se puede configurar como Pagada", new[] { nameof(InvoiceNumber) });
            }

            if (CheckQuantity < 0)
                throw new ValidationExtensions.ValidationException("La cantidad de cheques no puede ser menor a 0", new[] { nameof(CheckQuantity) });

            if (DaysToFirstPayment < 0)
                throw new ValidationExtensions.ValidationException("La cantidad de dias al primero no puede ser menor a 0", new[] { nameof(CheckQuantity) });

            if (DaysBetweenChecks < 0)
                throw new ValidationExtensions.ValidationException("La cantidad de dias entre cheques no puede ser menor a 0", new[] { nameof(CheckQuantity) });

            /*if(CurrencyId != 2)
            {
                ProductCurrencyParity pcp = context.ProductCurrencyParities.Where(x => x.Start <= ContractDate && x.End >= ContractDate).FirstOrDefault();
                if(pcp == null)
                {
                    throw new ValidationExtensions.ValidationException("No hay paridad disponible para el producto seleccionado", new[] { nameof(CurrencyId) });
                }
            }*/

            return ValidationResult.Success;
        }
    }
}