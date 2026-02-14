FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ShevaTahanotNotifier/ShevaTahanotNotifier.csproj", "ShevaTahanotNotifier/"]
RUN dotnet restore "ShevaTahanotNotifier/ShevaTahanotNotifier.csproj"
COPY . .
WORKDIR "/src/ShevaTahanotNotifier"
RUN dotnet build "./ShevaTahanotNotifier.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShevaTahanotNotifier.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN mkdir -p ./data
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShevaTahanotNotifier.dll"]
