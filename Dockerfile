FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App
COPY . ./
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /App
COPY --from=build-env /App/out .
RUN mkdir -p/App/logs
EXPOSE 2323
ENTRYPOINT ["dotnet", "labaDocker.dll"]
