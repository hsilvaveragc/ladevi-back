using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCurrencyParities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM ""CurrencyParities""");

            migrationBuilder.Sql(@"
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (1, '2020-01-01', '2020-08-01', 63);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (1, '2020-08-01', '2020-10-01', 86);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (1, '2020-10-01', '2022-01-01', 100);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (1, '2022-01-01', '2023-01-04', 150);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (1, '2023-01-04', '2023-01-10', 164);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (1, '2023-01-10', '2023-04-20', 186);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (1, '2023-04-20', '2023-08-01', 220);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (1, '2023-08-01', '2023-08-15', 300);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (1, '2023-08-15', '2023-12-21', 365);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (1, '2023-12-21', '2123-12-21', 800);

                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (3, '2020-01-01', '2020-09-14', 800);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (3, '2020-09-14', '2021-03-01', 780);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (3, '2021-03-01', '2021-04-01', 726.37);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (3, '2021-04-01', '2022-04-01', 750);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (3, '2022-04-01', '2022-04-23', 800);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (3, '2022-04-23', '2023-01-10', 850);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (3, '2023-01-10', '2023-04-20', 900);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (3, '2023-04-20', '2123-04-20', 850);

                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (4, '2020-01-01', '2020-04-01', 20);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (4, '2020-04-01', '2022-07-31', 24);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (4, '2022-07-31', '2122-03-03', 20);

                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (5, '2020-01-01', '2020-04-01', 3300);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (5, '2020-04-01', '2020-09-20', 3600);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (5, '2020-09-20', '2022-01-01', 3784);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (5, '2022-01-01', '2023-01-10', 4000);
                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (5, '2023-01-10', '2123-04-20', 4500);

                INSERT INTO public.""CurrencyParities""(""CurrencyId"", ""Start"", ""End"", ""LocalCurrencyToDollarExchangeRate"") VALUES (6, '2020-01-10', '2050-12-31', 4);
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
