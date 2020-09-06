#!/bin/sh

# install / update dependencies
dotnet restore

# change into project dir
cd NetRPG/

# run tests
dotnet run --project NetRPG.csproj