# Get the directory of the script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Get the current PATH environment variable
$currentPath = [System.Environment]::GetEnvironmentVariable('PATH', [System.EnvironmentVariableTarget]::User)

# Check if the script directory is already in the PATH
if ($currentPath -notlike "*$scriptDir*") {
    # If not, add it to the PATH
    $newPath = $currentPath + ";" + $scriptDir
    [System.Environment]::SetEnvironmentVariable('PATH', $newPath, [System.EnvironmentVariableTarget]::User)
    Write-Host "Added $scriptDir to PATH"
}

# Get the current user path after potential modification
$userPath = [System.Environment]::GetEnvironmentVariable('PATH', [System.EnvironmentVariableTarget]::User)

# Create env variable for the path so that dotnet run works from anywhere
if (-not [System.Environment]::GetEnvironmentVariable('rarbg_cli_path', [System.EnvironmentVariableTarget]::User)) {
    # Set the environment variable 'rarbg_cli_path' to the current script directory
    setx rarbg_cli_path $scriptDir
    Write-Host "Created 'rarbg_cli_path' environment variable."
}