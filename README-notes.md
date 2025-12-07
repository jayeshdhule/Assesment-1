# README-notes

What I changed

- Implemented commission calculation logic and input validation in `api/Controllers/CommisionController.cs`.
- Removed duplicate controller/DTO code from `api/Program.cs` (keeps startup only).
- Enabled CORS to allow `http://localhost:3000` in development.
- Updated test project to target `net8.0` and added `Microsoft.NET.Test.Sdk` and a project reference to the API project.
- Added `CommissionCalculationTests.cs` with unit tests covering normal and invalid inputs.
-- Previously added temporary safeguards in `api/AvalphaTechnologies.CommissionCalculator.csproj` have been removed. The test project was moved to a top-level `tests/` folder and the API project now uses the default SDK behavior.

How to run (backend)

1. Open a PowerShell terminal.
2. Run the API from the `api` folder:

```powershell
cd api
dotnet run --project AvalphaTechnologies.CommissionCalculator.csproj
```

The API will listen on the URLs configured in `Properties/launchSettings.json` (e.g. `https://localhost:5000` and `http://localhost:5111`). The controller endpoint is POST `/Commision`.

Example request body (JSON):

```json
{
  "localSalesCount": 10,
  "foreignSalesCount": 10,
  "averageSaleAmount": 100.00
}
```

How to run tests (backend)

1. From the repository root run:

```powershell
cd tests\AvalphaTechnologies.CommissionCalculator.Tests
dotnet test -c Debug
```

All tests should pass (there are unit tests validating the commission logic).

How to run (frontend)

1. Open a terminal and run:

```powershell
cd ui
npm install
npm start
```

2. Open http://localhost:3000 in your browser and use the UI to call the API. The front-end posts to `https://localhost:5000/Commision` by default; you can set `REACT_APP_API_URL` to `http://localhost:5111` or `https://localhost:5000` in `.env` if needed.

Notes / troubleshooting

-- Duplicate assembly attribute errors were encountered earlier because the test project was nested under `api` and MSBuild globs picked up both application and test C# files. This has been resolved by moving the tests to a top-level `tests/` folder and restoring the API project to the default SDK behavior.

- The API uses decimal arithmetic and rounds monetary amounts to 2 decimal places before returning them.

- Business rates implemented:
  - Avalpha local: 20%
  - Avalpha foreign: 35%
  - Competitor local: 2%
  - Competitor foreign: 7.55%

Follow-ups I can do (pick one or more)

- Remove the temporary assembly info generation toggles and move the test project to `tests/` (cleaner project layout).
- Add integration tests that spin up the in-memory TestServer and call the controller endpoints via HTTP.
- Improve frontend tests (React Testing Library) to mock API responses and test UI behaviour.
- Add more unit tests for edge cases (very large values, rounding behavior).

If you want me to proceed with any of the follow-ups, tell me which one and I'll implement it next.