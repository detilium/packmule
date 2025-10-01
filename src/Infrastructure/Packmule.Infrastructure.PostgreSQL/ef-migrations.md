# EF Migrations

## Create migration
```bash
dotnet ef migrations add [migration-name] --project Infrastructure/Packmule.Infrastructure.PostgreSQL/Packmule.Infrastructure.PostgreSQL.csproj --startup-project Packmule.Api/Packmule.Api.csproj
```

## Generate script
```bash
dotnet ef migrations script --idempotent --startup-project ../../Packmule.Api/Packmule.Api.csproj -o Scripts
```