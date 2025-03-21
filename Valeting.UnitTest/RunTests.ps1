dotnet test --collect:"XPlat Code Coverage"

$latestDir = Get-ChildItem -Path .\TestResults\ -Directory | Sort-Object LastWriteTime -Descending | Select-Object -First 1

reportgenerator -reports:"($latestDir.FullName)\coverage.cobertura.xml" -targetdir:.\TestResults\coveragereport