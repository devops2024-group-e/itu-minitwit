# Testing the API

## Starting the API

In a terminal start up the API by running the following:
```sh
cd MinitwitSimulatorAPI/
dotnet run
```
This should result in something like this:
```sh
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5121
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: [path]/itu-minitwit/MinitwitSimulatorAPI
info: Microsoft.Hosting.Lifetime[0]
      Application is shutting down...
```
This means that the program has been built and testing can start.

## Running the tests

In a different terminal from the one that is running the API, start the tests:
```sh
python minitwit_simulator.py "http://localhost:5121"
```

Note that the localhost address has to match the one given when running the server(see step 1).