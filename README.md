# README #

This README would normally document whatever steps are necessary to get your application up and running.

### What is this repository for? ###

* Quick summary
* Version
* [Learn Markdown](https://bitbucket.org/tutorials/markdowndemo)

### How do I get set up? ###

* Summary of set up
* Configuration
* Dependencies
* Database configuration
* How to run tests

  # Ejecutar tests con dotnet-coverage
  dotnet tool install -g dotnet-coverage
  dotnet-coverage collect "dotnet test TracyCommerceApi.Tests" -f cobertura -o TestResults/coverage-ms.cobertura.xml -s coverage.config.xml

  # Ejecutar armado de reportes
  dotnet tool install -g dotnet-reportgenerator-globaltool
  reportgenerator -reports:"TestResults/coverage-ms.cobertura.xml" -targetdir:"TestResults/CoverageReport-MS" -reporttypes:Html

  # Enviar a api custom la cobertura
  dotnet tool install -g dotnet-script
  dotnet script CoverageReporter.cs

* Deployment instructions

### Contribution guidelines ###

* Writing tests
* Code review
* Other guidelines

### Who do I talk to? ###

* Repo owner or admin
* Other community or team contact