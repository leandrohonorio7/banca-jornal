
# Dockerfile para aplicação Web (Blazor WASM + ASP.NET Core API)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BancaJornal.sln", "."]
COPY ["BancaJornal.Model/BancaJornal.Model.csproj", "BancaJornal.Model/"]
COPY ["BancaJornal.Repository/BancaJornal.Repository.csproj", "BancaJornal.Repository/"]
COPY ["BancaJornal.Application/BancaJornal.Application.csproj", "BancaJornal.Application/"]
COPY ["BancaJornal.Api/BancaJornal.Api.csproj", "BancaJornal.Api/"]
COPY ["BancaJornal.Web/BancaJornal.Web.csproj", "BancaJornal.Web/"]
COPY . .
RUN dotnet restore "BancaJornal.sln"
RUN dotnet build "BancaJornal.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BancaJornal.Api/BancaJornal.Api.csproj" -c Release -o /app/publish
RUN dotnet publish "BancaJornal.Web/BancaJornal.Web.csproj" -c Release -o /app/publish/web

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/publish/web ./web
ENTRYPOINT ["dotnet", "BancaJornal.Api.dll"]
