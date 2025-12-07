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

Notes / troubleshooting

- CORS: during development the API allows requests from `http://localhost:3000`. If you call the API over HTTP and the server is configured to redirect HTTP→HTTPS, browsers may block the OPTIONS preflight (307 redirect). Recommended approaches:
  - Call the HTTPS URL (e.g. `https://localhost:5000`) and trust the dev certificate once: `dotnet dev-certs https --trust`.
  - Or run the API in Development without `UseHttpsRedirection()` (currently handled in `Program.cs`).
- Tests: unit tests are in `api.Tests/` and test the `CommisionController` logic (calculations, validation, rounding, edge cases).
- Currency and rounding: monetary amounts use `decimal` and are rounded to 2 decimal places in responses.

Business rates used

- Avalpha local: 20%
- Avalpha foreign: 35%
- Competitor local: 2%
- Competitor foreign: 7.55%

If you want any follow-ups (add integration tests, add tests to solution, refine UI tests), tell me which one and I will implement it.

## Decisions, trade-offs and unfinished work

- Decisions
  - Placed the backend in `api/` as a minimal ASP.NET Web API project using controller-based endpoints for clarity and easy unit testing.
  - Kept the frontend simple (React) and used a simple fetch-based POST for the `/Commision` endpoint to keep the UI dependency-free and easy to reason about.

- Trade-offs
  - HTTPS vs HTTP in development: to avoid the browser preflight redirect problem we disabled `UseHttpsRedirection()` in Development mode. This makes the local dev experience easier (no need to trust the dev cert) but is less production-like. The alternative is to call the HTTPS endpoint from the frontend and run `dotnet dev-certs https --trust` locally.
  - Early work created nested test files which caused duplicate assembly attribute and build-copy issues; the pragmatic fix was to move tests to a top-level folder and restore the API csproj defaults. Tests were later consolidated into `api.Tests/` to keep them alongside the code they test.

- Unfinished / future improvements
  - Add integration tests (TestServer or WebApplicationFactory) to exercise the controller over HTTP and validate CORS/end-to-end behaviour automatically.
  - Add both API and test projects to the main solution file (`.sln`) so `dotnet test` at the solution level runs all tests by default.
  - Improve UI tests to mock API responses and validate error paths (network failures, server errors, validation feedback).
  - Consider adding OpenAPI/Swagger examples for the `/Commision` endpoint and including sample requests in the repo.

If you'd like me to implement any of these follow-ups, tell me which and I'll proceed.