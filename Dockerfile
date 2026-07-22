# ETAPA 1: Construção (Build)
# Pega o ambiente do .NET 8 com as ferramentas de compilação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de projeto e restaura as dependências (Pacotes NuGet)
COPY ["GestaoManutencao/GestaoManutencao.csproj", "GestaoManutencao/"]
RUN dotnet restore "GestaoManutencao/GestaoManutencao.csproj"

# Copia o resto do código da API e compila a versão final
COPY . .
WORKDIR "/src/GestaoManutencao"
RUN dotnet build "GestaoManutencao.csproj" -c Release -o /app/build
RUN dotnet publish "GestaoManutencao.csproj" -c Release -o /app/publish

# ETAPA 2: Execução (Run)
# Pega uma versão super leve do .NET 8 apenas para rodar a API (economiza memória)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Avisa o Render que a API vai responder na porta 8080 (padrão de contêineres web)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# O comando que "dá o play" no seu sistema
ENTRYPOINT ["dotnet", "GestaoManutencao.dll"]