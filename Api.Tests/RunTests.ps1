dotnet test --collect:"XPlat Code Coverage"

$lDir = Get-ChildItem -Path .\TestResults\ -Directory | Sort-Object LastWriteTime -Descending | Select-Object -First 1
echo $lDir

reportgenerator -reports:"$($lDir.FullName)\coverage.cobertura.xml" -targetdir:.\TestResults\coveragereport