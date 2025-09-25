using LadeviVentasApi.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LadeviVentasApi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", isEnabled: true);
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TaxType> TaxType { get; set; }
        public DbSet<Country> Country { get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<District> District { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<AdvertisingSpaceLocationType> AdvertisingSpaceLocationTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAdvertisingSpace> ProductAdvertisingSpaces { get; set; }
        public DbSet<ProductEdition> ProductEditions { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<BillingCondition> BillingConditions { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<SoldSpace> SoldSpaces { get; set; }
        public DbSet<PublishingOrder> PublishingOrders { get; set; }
        public DbSet<CheckPayment> CheckPayments { get; set; }
        public DbSet<ContractHistorical> ContractHistoricals { get; set; }
        public DbSet<ReportOPForProductionExports> ReportOPForProductionExports { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<Auditory> Auditory { get; set; }
        public DbSet<ProductCurrencyParity> ProductCurrencyParity { get; set; }
        public DbSet<ProductLocationDiscount> ProductLocationDiscount { get; set; }
        public DbSet<ProductVolumeDiscount> ProductVolumeDiscount { get; set; }
        public DbSet<ProductAdvertisingSpaceVolumeDiscount> ProductAdvertisingSpaceVolumeDiscount { get; set; }
        public DbSet<ProductAdvertisingSpaceLocationDiscount> ProductAdvertisingSpaceLocationDiscount { get; set; }
        public DbSet<CurrencyParity> CurrencyParities { get; set; }
        public DbSet<EuroParity> EuroParities { get; set; }
        public DbSet<TaxCategory> TaxCategories { get; set; }
        public DbSet<InventoryAdvertisingSpace> InventoryAdvertisingSpaces { get; set; }
        public DbSet<ProductionTemplate> ProductionTemplates { get; set; }
        public DbSet<ProductionSlot> ProductionSlots { get; set; }

        public void ContextSaveChanges()
        {
            this.SaveChanges();
        }
    }
}
