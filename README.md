# Credit Simulator

Aplicación de simulación de créditos con React, tres APIs ASP.NET Core 10 y PostgreSQL separado por microservicio. Permite registrar usuarios, comparar amortización francesa y alemana, guardar simulaciones y exportar sus tablas a PDF, Excel o CSV.

## Arquitectura

```text
React/Vite :5173
	|-- Auth API :5001 -------- auth_db
	|-- Catalog API :5002 ----- credit_catalog_db
	`-- Simulation API :5003 -- simulation_db
											 |
											 `-- HTTP -> Catalog API
PostgreSQL :5432
```

## Requisitos

- .NET SDK 10
- Node.js 22 o superior y npm
- PostgreSQL 16, o Docker Desktop

## Base de datos con Docker

Desde la raíz del repositorio:

```powershell
docker compose -f infrastructure/docker-compose.yml up -d postgres
```

Los scripts de `infrastructure/postgres/init` se ejecutan automáticamente en un volumen nuevo. Para ejecuciones manuales, ejecuta `01-create-databases.sql` en la base `postgres`, y después los scripts 02, 03 y 04 respectivamente en `auth_db`, `credit_catalog_db` y `simulation_db`.

## Instalación y ejecución local

Restaurar y ejecutar las APIs en terminales separadas:

```powershell
dotnet restore services/auth-service/auth-service.csproj
dotnet restore services/credit-catalog-service/credit-catalog-service.csproj
dotnet restore services/simulation-service/simulation-service.csproj
dotnet run --project services/auth-service/auth-service.csproj --urls http://localhost:5001
dotnet run --project services/credit-catalog-service/credit-catalog-service.csproj --urls http://localhost:5002
dotnet run --project services/simulation-service/simulation-service.csproj --urls http://localhost:5003
```

Frontend:

```powershell
cd frontend/credit-web
Copy-Item .env.example .env
npm install
npm run dev
```

Abre `http://localhost:5173`. Las claves JWT y contraseñas de desarrollo están en `appsettings.Development.json`; usa variables de entorno o un gestor de secretos fuera de desarrollo.

## Docker completo

```powershell
docker compose -f infrastructure/docker-compose.yml up --build
```

## Endpoints principales

- `POST /api/auth/register` y `POST /api/auth/login`
- `GET /api/credittypes` y `GET /api/credittypes/{id}`
- `POST /api/simulations`
- `GET /api/simulations` y `GET /api/simulations/{id}`
- `GET /api/simulations/{id}/export?format=pdf|xlsx|csv`

Las rutas de simulación requieren `Authorization: Bearer <token>`. Swagger está disponible en `/swagger` cuando cada API se ejecuta en entorno Development.

## Verificación de cálculos

Con monto 10 000, 12 meses y 12% anual, el método francés produce una cuota aproximada de 888.49; el método alemán produce una primera cuota de 933.33 y una última aproximada de 841.67.
