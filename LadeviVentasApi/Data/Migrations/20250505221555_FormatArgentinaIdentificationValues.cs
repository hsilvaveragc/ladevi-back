using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LadeviVentasApi.Data.Migrations
{
    /// <inheritdoc />
    public partial class FormatArgentinaIdentificationValues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Crear tabla permanente para guardar un registro de los cambios (permite revertir y auditar)
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS ""MigrationLogs"" (
                    ""Id"" SERIAL PRIMARY KEY,
                    ""MigrationName"" VARCHAR(100) NOT NULL,
                    ""TableName"" VARCHAR(100) NOT NULL,
                    ""RecordId"" INT NOT NULL,
                    ""ColumnName"" VARCHAR(100) NOT NULL,
                    ""OldValue"" TEXT,
                    ""NewValue"" TEXT,
                    ""DateCreated"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                );
            ");

            // Creamos una función que reemplaza caracteres no numéricos y formatea el CUIT/CUIL
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION format_identification_value(value text) 
                RETURNS text AS $$
                DECLARE
                    numbers_only text;
                BEGIN
                    -- Si es nulo o vacío, devolverlo sin cambios
                    IF value IS NULL OR value = '' THEN
                        RETURN value;
                    END IF;
                    
                    -- Si ya tiene el formato correcto, devolverlo sin cambios
                    IF value ~ '^[0-9]{2}-[0-9]{8}-[0-9]$' THEN
                        RETURN value;
                    END IF;
                    
                    -- Eliminar cualquier carácter que no sea número
                    numbers_only := regexp_replace(value, '[^0-9]', '', 'g');
                    
                    -- Verificar si tiene exactamente 11 dígitos (formato CUIT/CUIL)
                    IF length(numbers_only) = 11 THEN
                        -- Formatear como xx-xxxxxxxx-x
                        RETURN substring(numbers_only from 1 for 2) || '-' || 
                               substring(numbers_only from 3 for 8) || '-' || 
                               substring(numbers_only from 11 for 1);
                    ELSE
                        -- Si no tiene 11 dígitos, devolver el valor original
                        RETURN value;
                    END IF;
                END;
                $$ LANGUAGE plpgsql;
            ");

            // Registrar los cambios en la tabla de logs permanente antes de realizarlos
            migrationBuilder.Sql(@"
                INSERT INTO ""MigrationLogs"" (""MigrationName"", ""TableName"", ""RecordId"", ""ColumnName"", ""OldValue"", ""NewValue"")
                SELECT 
                    'FormatArgentinaIdentificationValues',
                    'Clients',
                    ""Id"",
                    'IdentificationValue',
                    ""IdentificationValue"",
                    format_identification_value(""IdentificationValue"")
                FROM ""Clients""
                WHERE (""Deleted"" is false or ""Deleted"" is null) 
                    AND ""IsComtur"" is false 
                    AND ""CountryId"" = 4
                    AND ""IdentificationValue"" IS NOT NULL 
                    AND ""IdentificationValue"" != ''
                    AND ""IdentificationValue"" != format_identification_value(""IdentificationValue"");
            ");

            // Actualizar los valores solo para clientes de Argentina
            migrationBuilder.Sql(@"
                UPDATE ""Clients"" 
                SET ""IdentificationValue"" = format_identification_value(""IdentificationValue"")
                WHERE (""Deleted"" is false or ""Deleted"" is null) 
                    AND ""IsComtur"" is false 
                    AND ""CountryId"" = 4
                    AND ""IdentificationValue"" IS NOT NULL 
                    AND ""IdentificationValue"" != ''
                    AND ""IdentificationValue"" != format_identification_value(""IdentificationValue"");
            ");

            // Registrar la cantidad de registros actualizados en el log
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    update_count integer;
                BEGIN
                    SELECT COUNT(*) INTO update_count 
                    FROM ""MigrationLogs"" 
                    WHERE ""MigrationName"" = 'FormatArgentinaIdentificationValues';
                    
                    RAISE NOTICE 'Identification values updated: %', update_count;
                END $$;
            ");

            // Eliminar la función temporal
            migrationBuilder.Sql("DROP FUNCTION format_identification_value(text);");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir los cambios usando la información guardada en la tabla de logs
            migrationBuilder.Sql(@"
                UPDATE ""Clients"" c
                SET ""IdentificationValue"" = m.""OldValue""
                FROM ""MigrationLogs"" m
                WHERE m.""MigrationName"" = 'FormatArgentinaIdentificationValues'
                AND m.""TableName"" = 'Clients'
                AND m.""RecordId"" = c.""Id""
                AND m.""ColumnName"" = 'IdentificationValue';
            ");

            // Registrar la cantidad de registros revertidos
            migrationBuilder.Sql(@"
                DO $$
                DECLARE
                    revert_count integer;
                BEGIN
                    SELECT COUNT(*) INTO revert_count 
                    FROM ""MigrationLogs"" 
                    WHERE ""MigrationName"" = 'FormatArgentinaIdentificationValues';
                    
                    RAISE NOTICE 'Identification values reverted: %', revert_count;
                END $$;
            ");

            // Opcional: eliminar los registros de log de esta migración específica
            // Nota: esto se puede comentar si se desea mantener el historial completo
            migrationBuilder.Sql(@"
                DELETE FROM ""MigrationLogs""
                WHERE ""MigrationName"" = 'FormatArgentinaIdentificationValues';
            ");
        }
    }
}
