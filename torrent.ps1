# Ensure the environment variable is set
$projectPath = Join-Path $env:rarbg_cli_path "HttpRequests.csproj"

# Ensure the project file path exists
if (Test-Path $projectPath) {
    # Run the dotnet project with arguments passed
    dotnet run --project $projectPath -- @args
} else {
    Write-Host "Project file not found at $projectPath"
}