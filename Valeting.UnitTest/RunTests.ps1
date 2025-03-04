dotnet test --collect:"XPlat Code Coverage"

$coverageFiles = Get-ChildItem -Path "./TestResults" -Recurse -Filter "coverage.cobertura.xml"
$latestCoverageFile = $coverageFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if ($null -eq $latestCoverageFile) {
    exit 1
}

reportgenerator -reports:$latestCoverageFile.FullName -targetdir:./TestResults -reporttypes:Html