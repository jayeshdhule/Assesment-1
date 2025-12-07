# README-notes

Summary

- Commission calculator backend implemented in `api/` (controller, validation, DTO response).
- Frontend `ui/` wired to call the API and display formatted GBP results.
- Unit tests for the backend located in `api.Tests/` (xUnit) with 10 comprehensive tests for `CommisionController`.

Quick start — backend

1. From repository root run:

```powershell
cd api
dotnet run --project AvalphaTechnologies.CommissionCalculator.csproj
```

The API exposes POST `/Commision` and listens on the URLs in `Properties/launchSettings.json` (typically `https://localhost:5000` and `http://localhost:5111`).

Quick start — tests

1. From repository root run:

```powershell
dotnet test .\api.Tests\api.Tests.csproj -c Debug
```

All tests should pass (10 tests covering normal operation, validation, rounding, and edge cases).

Quick start — frontend

1. From repository root run:

```powershell
cd ui
npm install
npm start
```

2. Open http://localhost:3000 and use the UI to call the API.

Business rates used

- Avalpha local: 20%
- Avalpha foreign: 35%
- Competitor local: 2%
- Competitor foreign: 7.55%

## Decisions

- Decisions
  - Placed the backend in `api/` as a minimal ASP.NET Web API project using controller-based endpoints for clarity and easy unit testing.
