using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class MigrateTextToCitext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE EXTENSION IF NOT EXISTS citext;

/*
select
	'ALTER TABLE ""'||relname
	||'"" ALTER ""'||attname
	||'"" TYPE '||field_type
	||' COLLATE ""'||collate_name||'"";'
from (SELECT
	(
		SELECT relname
		FROM pg_class
		WHERE oid = a.attrelid
		) as relname,
	a.attname,
	'citext'
		/*format_type(a.atttypid,a.atttypmod)*/
		as field_type,
	(
		SELECT collname
		FROM pg_collation
		where collname = 'es-AR-x-icu'
		) as collate_name
FROM pg_attribute AS a
JOIN pg_class AS c ON a.attrelid = c.oid
WHERE a.atttypid IN (25, 1042, 1043)
	AND c.relnamespace::regnamespace::name
		NOT IN ('pg_catalog', 'information_schema', 'pg_toast')
	AND not (text(a.attrelid::regclass) ilike '%pk_%')
	AND not (text(a.attrelid::regclass) ilike '%IX_%')
) as x
*/

ALTER TABLE ""Clients"" ALTER ""Address"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""District"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""City"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ApplicationUsers"" ALTER ""FullName"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ApplicationUsers"" ALTER ""Initials"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""AlternativeEmail"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""MainEmail"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""BillingPointOfSale"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""TelephoneCountryCode"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""TelephoneAreaCode"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""TelephoneNumber"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ApplicationRole"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""RefreshTokens"" ALTER ""Username"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""TaxType"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AdvertisingSpaceLocationTypes"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ProductTypes"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Products"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ProductAdvertisingSpaces"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ProductEditions"" ALTER ""Code"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ProductEditions"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Contracts"" ALTER ""InvoiceNumber"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Contracts"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""BillingConditions"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Currency"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""PaymentMethods"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""PublishingOrders"" ALTER ""Observations"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""PublishingOrders"" ALTER ""InvoiceNumber"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""SoldSpaces"" ALTER ""DescriptionSpecialDiscount"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""SoldSpaces"" ALTER ""DescriptionGerentialDiscount"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""District"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Currency"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Country"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Contracts"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""City"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ContractHistoricals"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AspNetRoles"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AspNetRoles"" ALTER ""NormalizedName"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AspNetUsers"" ALTER ""UserName"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AspNetUsers"" ALTER ""NormalizedUserName"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AspNetUsers"" ALTER ""Email"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AspNetUsers"" ALTER ""NormalizedEmail"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AspNetUsers"" ALTER ""PhoneNumber"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AspNetUserTokens"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AspNetUserTokens"" ALTER ""Value"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""BrandName"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""LegalName"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""PostalCode"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""State"" ALTER ""Name"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""TaxType"" ALTER ""OptionsInternal"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""SoldSpaces"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Contracts"" ALTER ""Observations"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""RefreshTokens"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""PublishingOrders"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ProductVolumeDiscount"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""IdentificationValue"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Clients"" ALTER ""Contact"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Country"" ALTER ""CodigoTelefonico"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""City"" ALTER ""CodigoTelefonico"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""TaxType"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""State"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ProductTypes"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Products"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ProductLocationDiscount"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ProductEditions"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ProductCurrencyParity"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ProductAdvertisingSpaces"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""PaymentMethods"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""CheckPayments"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""BillingConditions"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ApplicationUsers"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ApplicationRole"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""AdvertisingSpaceLocationTypes"" ALTER ""DeletedUser"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ContractHistoricals"" ALTER ""User"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""ContractHistoricals"" ALTER ""Changes"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Auditory"" ALTER ""User"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Auditory"" ALTER ""Entity"" TYPE citext COLLATE ""es-AR-x-icu"";
ALTER TABLE ""Auditory"" ALTER ""AuditMessage"" TYPE citext COLLATE ""es-AR-x-icu"";
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
