dotnet-coverage collect "dotnet test" -f xml -o "coverage.xml"
reportgenerator -reports:coverage.xml -targetdir:./TestResults