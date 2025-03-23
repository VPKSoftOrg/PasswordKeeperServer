FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8000
EXPOSE 8001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PasswordKeeper.BusinessLogic/PasswordKeeper.BusinessLogic.csproj", "PasswordKeeper.BusinessLogic/"]
COPY ["PasswordKeeper.Classes/PasswordKeeper.Classes.csproj", "PasswordKeeper.Classes/"]
COPY ["PasswordKeeper.DAO/PasswordKeeper.DAO.csproj", "PasswordKeeper.DAO/"]
COPY ["PasswordKeeper.DataAccess/PasswordKeeper.DataAccess.csproj", "PasswordKeeper.DataAccess/"]
COPY ["PasswordKeeper.DatabaseMigrations/PasswordKeeper.DatabaseMigrations.csproj", "PasswordKeeper.DatabaseMigrations/"]
COPY ["PasswordKeeper.DTO/PasswordKeeper.DTO.csproj", "PasswordKeeper.DTO/"]
COPY ["PasswordKeeper.Interfaces/PasswordKeeper.Interfaces.csproj", "PasswordKeeper.Interfaces/"]
COPY ["PasswordKeeper.Server/PasswordKeeper.Server.csproj", "PasswordKeeper.Server/"]
COPY ["./Directory.Build.props", "/"]
COPY ["./Directory.Build.targets", "/"]

RUN dotnet restore "PasswordKeeper.Server/PasswordKeeper.Server.csproj"
COPY . .
WORKDIR "/src/PasswordKeeper.Server"
RUN dotnet build "PasswordKeeper.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PasswordKeeper.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PasswordKeeper.Server.dll"]
