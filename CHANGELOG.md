# Changelog

Todos los cambios importantes del proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto adhiere a [Semantic Versioning](https://semver.org/lang/es/).

## [No Publicado]

## [1.2.1] - 2025-09-25

### Corregido

- Ordenes de publicación: Cambios en la validación de reglas de negocio para modificar una OP en caso de vendedores y supervisores. #T78

## [1.2.0] - 2025-09-24

### Añadido

- Clientes: Creacion de metodo para sincronizar clientes xubio comtur contra websami. #T76

## [1.1.3] - 2025-09-23

### Corregido

- Integración: Mapeo de codigo de países entre APIs de Xubio y Websami para evitar fallos al facturar. #T74

## [1.1.2] - 2025-09-22

### Corregido

- Facturacion: lista OPs que ya tienen nro de factura. #T72

## [1.1.1] - 2025-01-19

### Corregido

- **Clientes**: Corregido bug en sincronización con Xubio para clientes Comtur. #T71
  - La creación/modificación en Xubio ahora solo se ejecuta para clientes no-Comtur o Comtur con punto de venta. "99"
  - Actualizada validación en `ProcessXubioIntegration` para aplicar regla de negocio.
  - Corregida validación de punto de venta en modelo `Client` para evitar consultas innecesarias a Xubio.
  - Los clientes Comtur con punto de venta diferente a "99" ya no se sincronizan con Xubio.

## [1.1.0] - 2025-09-12

### Añadido

- APIs para gestión y procesamiento de facturas del nuevo modulo de facturación.
- Inicio de implementacion de reporte cobertura de tests. #T68
- Inicio de fixeo de test de integracion. #T68

### Cambiado

- Actualización de versión en proyectos LadeviVentasApi y Tests.

## [1.0.0] - [Fecha inicial]

### Añadido

- Versión inicial del sistema Ladevi.
