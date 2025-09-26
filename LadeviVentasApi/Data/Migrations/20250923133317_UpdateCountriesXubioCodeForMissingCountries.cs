using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCountriesXubioCodeForMissingCountries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Actualizar códigos de Xubio para países que necesitan mapeo específico
            // Basado en análisis de compatibilidad entre la DB local y Xubio

            migrationBuilder.Sql(@"
                UPDATE ""Country"" SET ""XubioCode"" = 'ALEMANIAREPFED' WHERE ""XubioCode"" = 'ALEMANIA';
                UPDATE ""Country"" SET ""XubioCode"" = 'ANGUILA_TERRITORIO_NO_AUTONOMO_DEL_R_UNIDO' WHERE ""XubioCode"" = 'ANGUILLA';
                UPDATE ""Country"" SET ""XubioCode"" = 'ANTIGUA_Y_BARBUDA' WHERE ""XubioCode"" = 'ANTIGUA';
                UPDATE ""Country"" SET ""XubioCode"" = 'BELICE' WHERE ""XubioCode"" = 'BELIZE';
                UPDATE ""Country"" SET ""XubioCode"" = 'BERMUDAS_TERRITORIO_NO_AUTONOMO_DEL_R_UNIDO' WHERE ""XubioCode"" = 'BERMUDA';
                UPDATE ""Country"" SET ""XubioCode"" = 'DOMINICANAREP' WHERE ""XubioCode"" = 'DOMINICANA';
                UPDATE ""Country"" SET ""XubioCode"" = 'EMIRATOS_ARABESUNID' WHERE ""XubioCode"" = 'EMIRATOS_ARABE';
                UPDATE ""Country"" SET ""XubioCode"" = 'ESPANA' WHERE ""XubioCode"" = 'ESPAÑA';
                UPDATE ""Country"" SET ""XubioCode"" = 'REINO_UNIDO' WHERE ""XubioCode"" = 'INGLATERRA';
                UPDATE ""Country"" SET ""XubioCode"" = 'COREA_REPUBLICANA' WHERE ""XubioCode"" = 'KOREA';
                UPDATE ""Country"" SET ""XubioCode"" = 'NUEVA_ZELANDIA' WHERE ""XubioCode"" = 'NUEVA_ZELANDA';
                UPDATE ""Country"" SET ""XubioCode"" = 'REP_CHECA' WHERE ""XubioCode"" = 'REPUBLICA_CHECA';
                UPDATE ""Country"" SET ""XubioCode"" = 'ESLOVAQUIA' WHERE ""XubioCode"" = 'SLOVAKIA';
                UPDATE ""Country"" SET ""XubioCode"" = 'REPUBLICA_DE_SUDAFRICA' WHERE ""XubioCode"" = 'SOUTH_AFRICA';
                UPDATE ""Country"" SET ""XubioCode"" = 'SAN_VICENTE_Y_LAS_GRANADINS' WHERE ""XubioCode"" = 'ST._VINCENT-TH';
                UPDATE ""Country"" SET ""XubioCode"" = 'THAILANDIA' WHERE ""XubioCode"" = 'TAILANDIA';
                UPDATE ""Country"" SET ""XubioCode"" = 'TRINIDAD_Y_TOBAGO' WHERE ""XubioCode"" = 'TRINIDAD_Y_TOB';
                UPDATE ""Country"" SET ""XubioCode"" = 'ESTADOS_UNIDOS' WHERE ""XubioCode"" = 'USA';
                UPDATE ""Country"" SET ""XubioCode"" = 'VIETNAM' WHERE ""XubioCode"" = 'VIETMAN';
                UPDATE ""Country"" SET ""XubioCode"" = 'MALTA' WHERE ""XubioCode"" = 'REPUBLICA_DE_MALTA';
                UPDATE ""Country"" SET ""XubioCode"" = NULL WHERE ""XubioCode"" = 'CURAZAO'; -- No existe en Xubio
                UPDATE ""Country"" SET ""XubioCode"" = NULL WHERE ""XubioCode"" = 'SAINT_MARTIN'; -- No existe en Xubio
                UPDATE ""Country"" SET ""XubioCode"" = NULL WHERE ""XubioCode"" = 'ST._MAARTEN'; -- No existe en Xubio
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback: revertir los códigos de Xubio a los valores originales
            migrationBuilder.Sql(@"
                UPDATE ""Country"" SET ""XubioCode"" = 'ALEMANIA' WHERE ""XubioCode"" = 'ALEMANIAREPFED';
                UPDATE ""Country"" SET ""XubioCode"" = 'ANGUILLA' WHERE ""XubioCode"" = 'ANGUILA_TERRITORIO_NO_AUTONOMO_DEL_R_UNIDO';
                UPDATE ""Country"" SET ""XubioCode"" = 'ANTIGUA' WHERE ""XubioCode"" = 'ANTIGUA_Y_BARBUDA';
                UPDATE ""Country"" SET ""XubioCode"" = 'BELIZE' WHERE ""XubioCode"" = 'BELICE';
                UPDATE ""Country"" SET ""XubioCode"" = 'BERMUDA' WHERE ""XubioCode"" = 'BERMUDAS_TERRITORIO_NO_AUTONOMO_DEL_R_UNIDO';
                UPDATE ""Country"" SET ""XubioCode"" = 'DOMINICANA' WHERE ""XubioCode"" = 'DOMINICANAREP';
                UPDATE ""Country"" SET ""XubioCode"" = 'EMIRATOS_ARABE' WHERE ""XubioCode"" = 'EMIRATOS_ARABESUNID';
                UPDATE ""Country"" SET ""XubioCode"" = 'ESPAÑA' WHERE ""XubioCode"" = 'ESPANA';
                UPDATE ""Country"" SET ""XubioCode"" = 'INGLATERRA' WHERE ""XubioCode"" = 'REINO_UNIDO';
                UPDATE ""Country"" SET ""XubioCode"" = 'KOREA' WHERE ""XubioCode"" = 'COREA_REPUBLICANA';
                UPDATE ""Country"" SET ""XubioCode"" = 'NUEVA_ZELANDA' WHERE ""XubioCode"" = 'NUEVA_ZELANDIA';
                UPDATE ""Country"" SET ""XubioCode"" = 'REPUBLICA_CHECA' WHERE ""XubioCode"" = 'REP_CHECA';
                UPDATE ""Country"" SET ""XubioCode"" = 'SLOVAKIA' WHERE ""XubioCode"" = 'ESLOVAQUIA';
                UPDATE ""Country"" SET ""XubioCode"" = 'SOUTH_AFRICA' WHERE ""XubioCode"" = 'REPUBLICA_DE_SUDAFRICA';
                UPDATE ""Country"" SET ""XubioCode"" = 'ST._VINCENT-TH' WHERE ""XubioCode"" = 'SAN_VICENTE_Y_LAS_GRANADINS';
                UPDATE ""Country"" SET ""XubioCode"" = 'TAILANDIA' WHERE ""XubioCode"" = 'THAILANDIA';
                UPDATE ""Country"" SET ""XubioCode"" = 'TRINIDAD_Y_TOB' WHERE ""XubioCode"" = 'TRINIDAD_Y_TOBAGO';
                UPDATE ""Country"" SET ""XubioCode"" = 'USA' WHERE ""XubioCode"" = 'ESTADOS_UNIDOS';
                UPDATE ""Country"" SET ""XubioCode"" = 'VIETMAN' WHERE ""XubioCode"" = 'VIETNAM';
                UPDATE ""Country"" SET ""XubioCode"" = 'REPUBLICA_DE_MALTA' WHERE ""XubioCode"" = 'MALTA';
                UPDATE ""Country"" SET ""XubioCode"" = 'CURAZAO' WHERE ""XubioCode"" IS NULL AND EXISTS (SELECT 1 FROM ""Country"" c WHERE c.""Codigo"" = 'CURAZAO');
                UPDATE ""Country"" SET ""XubioCode"" = 'SAINT_MARTIN' WHERE ""XubioCode"" IS NULL AND EXISTS (SELECT 1 FROM ""Country"" c WHERE c.""Codigo"" = 'SAINT_MARTIN');
                UPDATE ""Country"" SET ""XubioCode"" = 'ST._MAARTEN' WHERE ""XubioCode"" IS NULL AND EXISTS (SELECT 1 FROM ""Country"" c WHERE c.""Codigo"" = 'ST._MAARTEN');
            ");
        }
    }
}
