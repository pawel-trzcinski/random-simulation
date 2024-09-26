FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine as build-stage

WORKDIR /app
COPY src .

RUN dotnet build RandomSimulation.sln -c Release

RUN dotnet test RandomSimulation.sln \
	--logger:"console;verbosity=detailed" \
	--no-build


RUN dotnet publish /app/RandomSimulation/RandomSimulation.csproj -r linux-musl-x64 /p:ContainerFamily=alpine --self-contained -c Release -o /out

WORKDIR /out
RUN rm -f *.pdb

#####################################################
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine

COPY --from=build-stage /out /out

LABEL MAINTAINER="Pawel.Trzcinski@onet.eu"
LABEL SUMMARY="RandomSimulation RestService"

EXPOSE 15500 

WORKDIR /out
ENTRYPOINT [ "/out/RandomSimulation" ]