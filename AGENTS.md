# AGENTS.md

## Build & test

```bash
dotnet restore FreeKit.sln
dotnet build FreeKit.sln -c Release
dotnet test FreeKit.sln
```

Run a single test project:
```bash
dotnet test test/IGeekFan.FreeKit.xUnit/IGeekFan.FreeKit.xUnit.csproj
```

No custom lint, format, or typecheck config — standard `dotnet build` is the full verification step.

## Solution files — pick the right one

| File | Scope |
|---|---|
| `FreeKit.sln` | Everything: src + test + samples + nupkg scripts |
| `FreeKit-Core.sln` | Source libraries only (fastest build) |
| `FreeKit-OpenIddict.sln` | OpenIddict projects only |

Using the wrong `.sln` wastes build time or misses projects.

## Directory.Build.props hierarchy

- **Root** `Directory.Build.props`: defines MSBuild properties `DefaultNetCoreTargetFrameworkVersion8` (`net8.0`), `DefaultNetCoreTargetFrameworkVersion10` (`net10.0`), `DefaultFreeSqlVersion` (`3.5.310`).
- **`src/Directory.Build.props`**: imports root, adds NuGet packaging. Version is set here (`<Version>0.0.550</Version>`). `GeneratePackageOnBuild=True` — every build of a src project produces a `.nupkg`.

All csproj files reference these properties (e.g. `<TargetFrameworks>$(DefaultNetCoreTargetFrameworkVersion8);$(DefaultNetCoreTargetFrameworkVersion10)</TargetFrameworks>`). Change versions centrally, not per-project.

## Source layout (`src/`)

| Package | Key dependencies | Notes |
|---|---|---|
| `IGeekFan.FreeKit` | MediatR | Core: AuditEntity, Dependency interfaces |
| `IGeekFan.FreeKit.Extras` | FreeSql.DbContext, Autofac | Depends on FreeKit. T4-generated `CoreStrings.Designer.cs` |
| `IGeekFan.FreeKit.Modularity` | ASP.NET Core framework ref | Module system |
| `IGeekFan.FreeKit.Email` | MailKit | Email sender |
| `IGeekFan.Localization.FreeSql` | FreeSql | DB-backed localization |
| `IGeekFan.AspNetCore.Identity.FreeSql` | FreeSql, Identity.Stores | T4-generated `CoreStrings.Designer.cs` |
| `IGeekFan.AspNetCore.DataProtection.FreeSql` | FreeSql | DataProtection keys in DB |
| `IGeekFan.AspNetCore.DataProtection.FreeRedis` | FreeRedis | DataProtection keys in Redis |
| `IGeekFan.Extensions.Diagnostics.HealthChecks.FreeSql` | FreeSql | Health checks |
| `IGeekFan.AspNetCore.SignalR.FreeRedis` | FreeRedis | SignalR backplane |
| `IGeekFan.OpenIddict.FreeSql` | OpenIddict.Core, FreeSql | OpenIddict stores |
| `IGeekFan.R2.NET` | AWSSDK.S3 | Cloudflare R2 client. **net10.0 only** |

## Tests (`test/`)

- **Framework**: xUnit v3 + `Xunit.DependencyInjection` (DI via `Startup.cs`, not constructor injection).
- **Database**: SQLite in-memory via `FreeSql.Provider.Sqlite` or `FreeSql.Provider.SqliteCore`.
- **Shared helpers**: `test/Shared/` contains `ApiConsistencyTestBase.cs`, `MockHelpers.cs`, etc. Included via `<Compile Include="..\Shared\**\*.cs" />`.
- All test projects target `net10.0` only.

## Code generation (T4 templates)

`tools/Resources.tt` generates strongly-typed resource classes from `.resx` files. Used in:
- `src/IGeekFan.FreeKit.Extras/Properties/CoreStrings.Designer.cs`
- `src/IGeekFan.AspNetCore.Identity.FreeSql/Properties/CoreStrings.Designer.cs`

These `*.Designer.cs` files are auto-generated — do not edit by hand.

## NuGet packaging & release

CI (`.github/workflows/publish-nuget.yml`) publishes on `v*` tags. Uses `dotnet pack` per-project (not per-solution):
```bash
dotnet pack src/IGeekFan.FreeKit -c Release --no-build --output ./nupkg/packages
```

Local scripts in `nupkg/` (`build-all-release.ps1`, `pack.ps1`, `deploy.ps1`) automate version bump, build, pack, and push. Version is read from `src/Directory.Build.props`.

## Samples (`samples/`)

- `IGeekFan.FreeKit.Web` — main demo app (ASP.NET Core Web, SQLite + multiple DB providers, JWT auth, Swagger)
- `Module1`, `Module2` — modularity demo modules
- `Sample.Localization` — localization demo

## Key conventions

- All NuGet package IDs follow `IGeekFan.*` naming.
- FreeSql `DataType.Sqlite` is the standard test database.
- `AutofacServiceProviderFactory` is used for DI in tests and samples (not the default MS DI).
- Serilog is the logging provider in test infrastructure.
