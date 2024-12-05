# Get the directory of the script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

# Get the parent directory of the script directory
$parentDir = Split-Path -Parent $scriptDir

# Get the current PATH environment variable
$currentPath = [System.Environment]::GetEnvironmentVariable('PATH', [System.EnvironmentVariableTarget]::User)

# Check if the parent directory is already in the PATH
if ($currentPath -notlike "*$parentDir*") {
    # If not, add it to the PATH
    $newPath = $currentPath + ";" + $parentDir
    [System.Environment]::SetEnvironmentVariable('PATH', $newPath, [System.EnvironmentVariableTarget]::User)
    Write-Host "Added $parentDir to PATH"
}

# Get the current user path after potential modification
$userPath = [System.Environment]::GetEnvironmentVariable('PATH', [System.EnvironmentVariableTarget]::User)

# Create env variable for the path so that dotnet run works from anywhere
if (-not [System.Environment]::GetEnvironmentVariable('rarbg_cli_path', [System.EnvironmentVariableTarget]::User)) {
    # Set the environment variable 'rarbg_cli_path' to the parent directory
    setx rarbg_cli_path $parentDir
    Write-Host "Created 'rarbg_cli_path' environment variable."
}