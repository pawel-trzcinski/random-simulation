FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-stage

WORKDIR /app
COPY src .

RUN dotnet build RandomSimulation.sln -c Release

RUN dotnet test RandomSimulation.sln \
	--logger:"console;verbosity=detailed" \
	--no-build

RUN dotnet publish /app/RandomSimulation/RandomSimulation.csproj -c Release -o /out

WORKDIR /out
RUN rm -f *.pdb

#####################################################
FROM mcr.microsoft.com/dotnet/aspnet:8.0

COPY --from=build-stage /out /out

LABEL MAINTAINER="Pawel.Trzcinski@onet.eu"
LABEL SUMMARY="RandomSimulation RestService"

EXPOSE 15500 

WORKDIR /out
ENTRYPOINT [ "dotnet", "RandomSimulation.dll" ]
