using Microsoft.EntityFrameworkCore.Migrations;

namespace LadeviVentasApi.Data.Migrations
{
    public partial class FixDuplicateContractNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Obtener el número más alto actual y asignar números secuenciales
            // a los contratos duplicados que esta en la clausula WHERE

            migrationBuilder.Sql(@"
                WITH MaxNumber AS (
                    SELECT MAX(""Number"") as max_num FROM ""Contracts""
                ),
                DuplicatedContracts AS (
                    SELECT ctid, ""Number"", ""Id"",
                           ROW_NUMBER() OVER (ORDER BY ""Id"") as global_rn,
                           (SELECT max_num FROM MaxNumber) as base_num
                    FROM ""Contracts"" 
                    WHERE 
                        (""Number"" = 903 AND ""Id"" = 904) OR
                        (""Number"" = 954 AND ""Id"" = 955) OR
                        (""Number"" = 1468 AND ""Id"" = 1470) OR
                        (""Number"" = 1664 AND ""Id"" = 1667) OR
                        (""Number"" = 21429 AND ""Id"" = 21501)
                )
                UPDATE ""Contracts"" 
                SET ""Number"" = dc.base_num + dc.global_rn
                FROM DuplicatedContracts dc
                WHERE ""Contracts"".ctid = dc.ctid;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir los números específicos a sus valores originales duplicados
            migrationBuilder.Sql(@"
                UPDATE ""Contracts"" 
                SET ""Number"" = 903 
                WHERE ""Id"" = 904;
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Contracts"" 
                SET ""Number"" = 954 
                WHERE ""Id"" = 955;
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Contracts"" 
                SET ""Number"" = 1468 
                WHERE ""Id"" = 1470;
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Contracts"" 
                SET ""Number"" = 1664 
                WHERE ""Id"" = 1667;
            ");

            migrationBuilder.Sql(@"
                UPDATE ""Contracts"" 
                SET ""Number"" = 21429 
                WHERE ""Id"" = 21501;
            ");
        }
    }
}