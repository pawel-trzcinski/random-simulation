FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-stage

WORKDIR /app
COPY src .

RUN dotnet build RandomSimulation.sln -c Release

RUN dotnet test RandomSimulation.sln \
	--logger:"console;verbosity=detailed" \
	--no-build

RUN dotnet publish /app/RandomSimulation/RandomSimulation.csproj -c Release -o /out -f net6.0

#####################################################
FROM mcr.microsoft.com/dotnet/runtime:6.0-alpine

COPY --from=build-stage /out /out

LABEL MAINTAINER="Pawel.Trzcinski@onet.eu"
LABEL SUMMARY="RandomSimulation RestService"

EXPOSE 15500 

WORKDIR /out
ENTRYPOINT [ "dotnet", "RandomSimulation.dll" ]