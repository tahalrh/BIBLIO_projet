# ==========================================
# Étape 1 : Build & Test
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copie des fichiers projet et solution pour mise en cache optimale
COPY ["Biblio_DOTNET.sln", "./"]
COPY ["Biblio_DOTNET.csproj", "./"]
COPY ["Biblio_DOTNET.Tests/Biblio_DOTNET.Tests.csproj", "Biblio_DOTNET.Tests/"]

# Restauration des dépendances NuGet
RUN dotnet restore "Biblio_DOTNET.sln"

# Copie de l'intégralité du code source
COPY . .

# Exécution des tests unitaires durant la phase de build
RUN dotnet test "Biblio_DOTNET.Tests/Biblio_DOTNET.Tests.csproj" --no-restore --configuration Release

# Publication de l'application
RUN dotnet publish "Biblio_DOTNET.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# Étape 2 : Runtime léger et sécurisé
# ==========================================
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

# Dossier pour la persistance des données
RUN mkdir -p /app/data && chown -R $APP_UID:$APP_UID /app/data

# Copie des binaires compilés
COPY --from=build /app/publish .

# Variables d'environnement de persistance
ENV DATA_FILE_PATH=/app/data/bibliotheque_data.txt
ENV DOTNET_ENVIRONMENT=Production

# Utilisateur non-root intégré à .NET 8
USER $APP_UID

# Point d'entrée
ENTRYPOINT ["dotnet", "Biblio_DOTNET.dll"]
