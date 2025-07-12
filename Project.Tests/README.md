# Test Suite Guide for Project.Tests

## 1. Running Unit Test & Integration Test

- **Requirements:** .NET 7 SDK or higher, all dependencies installed.
- **Run all tests:**  
  ```bash
  dotnet test
  ```
- **Run tests by category (if any):**  
  ```bash
  dotnet test --filter Category=Unit
  dotnet test --filter Category=Integration
  ```
- **Run tests in VSCode:**  
  - Install C# and .NET Test Explorer extensions.
  - Click the test icon in the sidebar to view and run individual tests.

## 2. Test Environment Configuration

- **Database:**  
  - Integration tests use SQLite in-memory or custom configuration in `appsettings.Development.json`.
  - Ensure environment variables (connection string, API key, etc.) are set correctly when running tests in CI/CD.
- **Seed Data:**  
  - Sample data is managed at `Project.Tests/TestData/SeedData/` and accessed via `Helpers/TestDataFixture.cs`.

## 3. CI/CD Notes

- **Pipeline:**  
  - Integrate the `dotnet test` step in your CI pipeline (GitHub Actions, GitLab CI, Azure DevOps, etc.).
  - You can add coverage check steps:
    ```bash
    dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
    ```
  - Coverage reports should reach at least 80% for core modules.
- **Fail Fast:**  
  - If a test fails, the pipeline should stop immediately to save resources.

## 4. Test Suite Maintenance & Extension Rules

- **No code duplication:**  
  - Use shared helpers and fixtures (`Helpers/TestMockHelper.cs`, `Helpers/TestDataFixture.cs`).
- **Documentation:**  
  - Each test class and group must have clear comments/summary about its purpose and coverage.
- **Test naming:**  
  - Follow the syntax: `MethodName_Scenario_ExpectedResult`.
- **Adding new tests:**  
  - Ensure tests cover both success and failure flows.
  - Prioritize writing tests for bug fixes and new features before merging code.

---