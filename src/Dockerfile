# 1. Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto e restaura as dependências
COPY ["FCG.UsersAPI/FCG.UsersAPI.csproj", "FCG.UsersAPI/"]
COPY ["FCG.Contracts/FCG.Contracts.csproj", "FCG.Contracts/"]
RUN dotnet restore "FCG.UsersAPI/FCG.UsersAPI.csproj"

# Copia o restante dos arquivos e compila
COPY . .
WORKDIR "/src/FCG.UsersAPI"
RUN dotnet publish "FCG.UsersAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 2. Estágio de Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FCG.UsersAPI.dll"]