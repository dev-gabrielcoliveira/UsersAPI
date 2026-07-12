# 1. Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copia o projeto
COPY ["FCG.UsersAPI.csproj", "./"]

# Restaura dependências
RUN dotnet restore "FCG.UsersAPI.csproj"

# Copia o restante da aplicação
COPY . .

# Publica
RUN dotnet publish "FCG.UsersAPI.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# 2. Estágio de Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "FCG.UsersAPI.dll"]