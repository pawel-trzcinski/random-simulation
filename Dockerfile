FROM mcr.microsoft.com/dotnet/core/sdk:2.1 as build-stage

WORKDIR /app
COPY src .

RUN dotnet build RandomSimulation.sln -c Release

RUN dotnet vstest --logger:"console;verbosity=detailed" \
    /app/RandomSimulation.Tests/bin/Release/netcoreapp2.1/RandomSimulation.Tests.dll

RUN dotnet publish /app/RandomSimulation/RandomSimulation.csproj -c Release -o /out -f netcoreapp2.1

#####################################################
FROM mcr.microsoft.com/dotnet/core/runtime:2.1-alpine

COPY --from=build-stage /out /out

LABEL MAINTAINER="Pawel.Trzcinski@onet.eu"
LABEL SUMMARY="RandomSimulation RestService"

EXPOSE 15500 

WORKDIR /out
ENTRYPOINT [ "dotnet", "RandomSimulation.dll" ]