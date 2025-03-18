FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PasswordKeeperServer/PasswordKeeperServer.csproj", "PasswordKeeperServer/"]
COPY ["PasswordKeeper.BusinessLogic/PasswordKeeper.BusinessLogic.csproj", "PasswordKeeper.BusinessLogic/"]
COPY ["PasswordKeeper.DataAccess/PasswordKeeper.DataAccess.csproj", "PasswordKeeper.DataAccess/"]
COPY ["PasswordKeeper.DAO/PasswordKeeper.DAO.csproj", "PasswordKeeper.DAO/"]
COPY ["PasswordKeeper.DTO/PasswordKeeper.DTO.csproj", "PasswordKeeper.DTO/"]
COPY ["PasswordKeeper.Interfaces/PasswordKeeper.Interfaces.csproj", "PasswordKeeper.Interfaces/"]
COPY ["DbMigrate/DbMigrate.csproj", "DbMigrate/"]
RUN dotnet restore "PasswordKeeperServer/PasswordKeeperServer.csproj"
COPY . .
WORKDIR "/src/PasswordKeeperServer"
RUN dotnet build "PasswordKeeperServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PasswordKeeperServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PasswordKeeperServer.dll"]
