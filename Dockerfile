# Dockerfile para aplicação WPF .NET 8
# ATENÇÃO: WPF só roda em Windows Containers!
FROM mcr.microsoft.com/dotnet/runtime:8.0-windowsservercore-ltsc2022 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022 AS build
WORKDIR /src
COPY ["BancaJornal.sln", "."]
COPY ["BancaJornal.Model/BancaJornal.Model.csproj", "BancaJornal.Model/"]
COPY ["BancaJornal.Repository/BancaJornal.Repository.csproj", "BancaJornal.Repository/"]
COPY ["BancaJornal.Application/BancaJornal.Application.csproj", "BancaJornal.Application/"]
COPY ["BancaJornal.Desktop/BancaJornal.Desktop.csproj", "BancaJornal.Desktop/"]
COPY . .
RUN dotnet restore "BancaJornal.sln"
RUN dotnet build "BancaJornal.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BancaJornal.Desktop/BancaJornal.Desktop.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# SQLite: banco será criado no diretório de trabalho
# Executa o app desktop (WPF)
ENTRYPOINT ["BancaJornal.Desktop.exe"]
