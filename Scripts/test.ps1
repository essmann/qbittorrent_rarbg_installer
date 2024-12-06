$currentDirectory = Split-Path $MyInvocation.MyCommand.Path

echo $currentDirectory

$parentDirectory = Split-Path (Split-Path $MyInvocation.MyCommand.Path) -Parent

[System.Console]::WriteLine("Hello from C# Console.WriteLine!")


echo $parentDirectory


# Add the directory to the PATH for the current user
[System.Environment]::SetEnvironmentVariable(
    "PATH",
    [System.Environment]::GetEnvironmentVariable("PATH", [System.EnvironmentVariableTarget]::User) + ";$parentDirectory",
    [System.EnvironmentVariableTarget]::User
)
	
# Add the directory to the PATH for the current user
[System.Environment]::SetEnvironmentVariable(
    "PATH",
    [System.Environment]::GetEnvironmentVariable("PATH", [System.EnvironmentVariableTarget]::User) + ";$currentDirectory",
    [System.EnvironmentVariableTarget]::User
)
	