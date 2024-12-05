# Get the directory of the script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Optionally: Get the parent directory of the script directory, which should be the main project directory
$parentDir = Split-Path -Parent $scriptDir

# Check if the script directory exists as expected
Write-Host "Script directory is: $scriptDir"
Write-Host "Parent directory is: $parentDir"

# Get the current PATH environment variable
$currentPath = [System.Environment]::GetEnvironmentVariable('PATH', [System.EnvironmentVariableTarget]::User)

# Check if the script directory is already in the PATH
if ($currentPath -notlike "*$scriptDir*") {
    # If not, add it to the PATH
    $newPath = $currentPath + ";" + $scriptDir
    [System.Environment]::SetEnvironmentVariable('PATH', $newPath, [System.EnvironmentVariableTarget]::User)
    Write-Host "Added $scriptDir to PATH"
}

# Check if the parent directory is already in the PATH
if ($currentPath -notlike "*$parentDir*") {
    # If not, add it to the PATH
    $newPath = $currentPath + ";" + $parentDir
    [System.Environment]::SetEnvironmentVariable('PATH', $newPath, [System.EnvironmentVariableTarget]::User)
    Write-Host "Added $parentDir to PATH"
}

# Create env variable for the path so that dotnet run works from anywhere
if (-not [System.Environment]::GetEnvironmentVariable('rarbg_cli_path', [System.EnvironmentVariableTarget]::User)) {
    # Set the environment variable 'rarbg_cli_path' to the parent directory
    setx rarbg_cli_path $parentDir
    Write-Host "Created 'rarbg_cli_path' environment variable."
}
