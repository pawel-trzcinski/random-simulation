FROM mcr.microsoft.com/dotnet/core/sdk:2.1 as build-stage

WORKDIR /app
COPY src .

RUN dotnet build ClassNamer.sln -c Release

RUN dotnet vstest --logger:"console;verbosity=detailed" \
    /app/UnitTests/ClassNamer.Tests/bin/Release/netcoreapp2.1/ClassNamer.Tests.dll \
    /app/UnitTests/ClassNamerEngine.Tests/bin/Release/netcoreapp2.1/ClassNamerEngine.Tests.dll

RUN dotnet publish /app/ClassNamer/ClassNamer.csproj -c Release -o /out -f netcoreapp2.1

#FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim
# 270

#FROM mcr.microsoft.com/dotnet/core/runtime:2.1
# 197

FROM mcr.microsoft.com/dotnet/core/runtime:2.1-alpine
# 103MB

COPY --from=build-stage /out /out

LABEL MAINTAINER="Pawel.Trzcinski@onet.eu"
LABEL SUMMARY="ClassNamer RestService"

# serviceui 
EXPOSE 15400 

WORKDIR /out
ENTRYPOINT [ "dotnet", "ClassNamer.dll" ]