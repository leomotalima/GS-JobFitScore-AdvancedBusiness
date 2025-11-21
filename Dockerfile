FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 10000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos do projeto principal
COPY ["JobFitScoreAPI.csproj", "./"]

# Copiar projeto de testes (necessário para restore)
COPY ["JobFitScore.Tests/JobFitScore.Tests.csproj", "JobFitScore.Tests/"]

# Restore
RUN dotnet restore "JobFitScoreAPI.csproj"

# Copiar todo o código
COPY . .

# Build apenas do projeto principal
WORKDIR "/src"
RUN dotnet build "JobFitScoreAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobFitScoreAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copiar arquivos estáticos se necessário
COPY ["Static/", "Static/"]

ENTRYPOINT ["dotnet", "JobFitScoreAPI.dll"]