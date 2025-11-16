# This file is used to run the SecretSync.Server project with a specified configuration.
# Usage: .\run-server.ps1 -Configuration Release
# Currently this file is untested
Param(
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

Write-Host "Running SecretSync.Server in configuration '$Configuration'..."

dotnet run --project "src/SecretSync.Server/SecretSync.Server.csproj" --configuration $Configuration
