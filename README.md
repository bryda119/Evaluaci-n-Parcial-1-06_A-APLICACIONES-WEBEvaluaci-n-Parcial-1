# Sistema de Gestión de Ventas — UNIANDES

**Proyecto académico** · Universidad Regional Autónoma de los Andes (UNIANDES)  
**Stack:** ASP.NET Core 8 MVC · Entity Framework Core · SQLite · Docker · Tailwind CSS

---

## 1. Presentación del proyecto

Este sistema permite **gestionar productos, clientes y ventas** con control de stock, según el enunciado del examen:

> Diseñar un sistema usando .NET o PHP para gestionar productos y clientes, incluyendo ventas.

### Qué resuelve

| Módulo | Qué puedes hacer |
|--------|------------------|
| **Productos** | Crear, listar, editar, ver y eliminar productos (precio y stock) |
| **Clientes** | Crear, listar, editar, ver y eliminar clientes (email único) |
| **Ventas** | Registrar una venta con varios productos, ver historial y detalle; el stock se descuenta automáticamente |

### Análisis del enunciado

El enunciado define solo dos tablas (`Productos` y `Clientes`), pero para las ventas hace falta un modelo completo:

- **Tablas dadas:** `Productos`, `Clientes`
- **Tablas añadidas (diseño):** `Ventas` (cabecera) y `DetalleVentas` (líneas)

Relación clásica **cabecera / detalle**: una venta pertenece a un cliente y tiene N productos. El `precio_unitario` se guarda en el detalle para no alterar el histórico si luego cambia el precio del producto.

---

## 2. Stack y decisiones técnicas

| Decisión | Elección |
|----------|----------|
| Framework | ASP.NET Core 8 **MVC** |
| ORM / BD | Entity Framework Core + **SQLite** |
| Contenedores | **Docker** + `docker-compose` |
| UI | Tailwind CSS + identidad UNIANDES |
| Autenticación | No incluida (fuera del enunciado) |
| Pagos / facturación | No incluidos; solo registro de ventas y stock |

---

## 3. Modelo de datos

```
Clientes 1 ─── N Ventas 1 ─── N DetalleVentas N ─── 1 Productos
```

| Tabla | Campos |
|-------|--------|
| **Productos** | `producto_id`, `nombre`, `descripcion`, `precio`, `stock` |
| **Clientes** | `cliente_id`, `nombre`, `apellido`, `email`, `telefono` |
| **Ventas** | `venta_id`, `cliente_id`, `fecha`, `total` |
| **DetalleVentas** | `detalle_id`, `venta_id`, `producto_id`, `cantidad`, `precio_unitario`, `subtotal` |

### Flujo al registrar una venta

1. El usuario elige cliente y una o más líneas (producto + cantidad).
2. El sistema valida cliente, productos y **stock disponible**.
3. Si hay stock insuficiente → muestra error y no guarda.
4. Si todo está bien → en una **transacción**:
   - inserta la venta y sus detalles,
   - descuenta el stock de cada producto,
   - redirige al detalle de la venta.

---

## 4. Estructura del proyecto

```
examen/
├── README.md                 ← este documento
├── docker-compose.yml        ← orquestación Docker
├── data/                     ← SQLite persistente (se crea al ejecutar)
└── SistemaVentas/
    ├── SistemaVentas.csproj
    ├── Program.cs            ← arranque + migraciones automáticas
    ├── appsettings.json      ← connection string SQLite
    ├── Dockerfile
    ├── Controllers/
    │   ├── HomeController.cs
    │   ├── ProductosController.cs
    │   ├── ClientesController.cs
    │   └── VentasController.cs
    ├── Models/
    │   ├── Producto.cs
    │   ├── Cliente.cs
    │   ├── Venta.cs
    │   ├── DetalleVenta.cs
    │   └── VentaCreateViewModel.cs
    ├── Data/
    │   └── AppDbContext.cs
    ├── Migrations/           ← esquema SQLite
    ├── Views/                ← pantallas MVC
    └── wwwroot/              ← CSS, JS, logo UNIANDES
```

---

## 5. Validaciones implementadas

### Productos
- Nombre obligatorio
- **Precio:** solo números decimales (mín. 0.01)
- **Stock:** solo números enteros (≥ 0)

### Clientes
- Nombre y apellido: **solo letras**
- Email: formato válido y **único** en la base
- Teléfono: **solo números** (7 a 15 dígitos), opcional

### Ventas
- Cliente obligatorio
- Al menos un producto con cantidad entera ≥ 1
- Validación de stock antes de guardar

---

## 6. Requisitos previos

Instala **una** de estas opciones según cómo vayas a ejecutar:

### Opción A — Local (sin Docker)
1. [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. (Opcional) herramienta EF:  
   `dotnet tool install --global dotnet-ef --version 8.0.11`

### Opción B — Docker
1. [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y **encendido**

---

## 7. Cómo ejecutar el proyecto (paso a paso)

### Opción A — Ejecutar en local (recomendado para desarrollo)

**Paso 1.** Abre una terminal (PowerShell o CMD).

**Paso 2.** Entra a la carpeta del proyecto:

```powershell
cd C:\Users\Brayan-Cangas\Documents\examen\SistemaVentas
```

**Paso 3.** Restaura paquetes NuGet:

```powershell
dotnet restore
```

**Paso 4.** (Opcional) Aplica migraciones de base de datos:

```powershell
dotnet ef database update
```

> Si no tienes `dotnet-ef`, puedes saltar este paso: al iniciar la app, `Program.cs` ejecuta `Database.Migrate()` solo.

**Paso 5.** Arranca la aplicación:

```powershell
dotnet run --urls http://localhost:5080
```

**Paso 6.** Abre el navegador en:

👉 **http://localhost:5080**

**Paso 7.** Para detener la app, en la terminal pulsa `Ctrl + C`.

---

### Opción B — Ejecutar con Docker

**Paso 1.** Abre **Docker Desktop** y espera a que esté en estado *Running*.

**Paso 2.** Abre una terminal y ve a la carpeta raíz del examen:

```powershell
cd C:\Users\Brayan-Cangas\Documents\examen
```

**Paso 3.** Construye e inicia el contenedor:

```powershell
docker compose up --build
```

La primera vez puede tardar varios minutos (descarga imagen .NET y compila).

**Paso 4.** Cuando veas en la consola que la app está escuchando, abre:

👉 **http://localhost:8080**

**Paso 5.** La base de datos SQLite se guarda en:

```
C:\Users\Brayan-Cangas\Documents\examen\data\ventas.db
```

Así los datos **no se pierden** al reiniciar el contenedor.

**Paso 6.** Para detener:

```powershell
docker compose down
```

O en la misma terminal: `Ctrl + C` y luego `docker compose down`.

---

## 8. Cómo probar el sistema (guía de demostración)

Usa estos pasos en la defensa / presentación:

1. **Inicio**  
   Entra a http://localhost:5080 (o `:8080` con Docker). Verás el panel con identidad UNIANDES.

2. **Productos**  
   - Menú → **Productos**  
   - Verás 3 productos de ejemplo (Laptop, Mouse, Teclado).  
   - Prueba **Nuevo producto**: si pones letras en el precio, no debe aceptarlas.

3. **Clientes**  
   - Menú → **Clientes**  
   - Verás 2 clientes de ejemplo.  
   - Prueba crear uno: nombre solo letras, email válido, teléfono solo números.

4. **Ventas**  
   - Menú → **Ventas** → **Nueva venta**  
   - Elige un cliente.  
   - Elige producto(s) y cantidad.  
   - Guarda → debe mostrar el detalle y **bajar el stock** del producto.

5. **Stock insuficiente**  
   - Intenta vender más unidades de las que hay en stock.  
   - El sistema debe mostrar error y **no** registrar la venta.

---

## 9. Datos de prueba (seed)

Al crear la base se cargan automáticamente:

| Tipo | Datos |
|------|--------|
| Productos | Laptop ($899.99, stock 10), Mouse ($25.50, stock 50), Teclado ($75.00, stock 30) |
| Clientes | Ana García, Luis Pérez |

---

## 10. Resumen para la presentación

| Pregunta | Respuesta corta |
|----------|-----------------|
| ¿Qué tecnología usaste? | ASP.NET Core 8 MVC + SQLite + Docker |
| ¿Cómo se relacionan las tablas? | Cliente → Ventas → DetalleVentas ← Productos |
| ¿Qué pasa con el stock? | Se valida y se descuenta en una transacción al guardar la venta |
| ¿Cómo se ejecuta? | `dotnet run` en local o `docker compose up --build` |
| ¿Dónde está la BD? | Archivo SQLite `data/ventas.db` |

---

## 11. Autor / institución

**Universidad Regional Autónoma de los Andes — UNIANDES**  
Sitio: [https://www.uniandes.edu.ec/](https://www.uniandes.edu.ec/)  

Proyecto de examen: *Sistema de Gestión de Ventas*
