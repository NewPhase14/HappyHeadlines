FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SubscriberDatabase/SubscriberDatabase.csproj", "SubscriberDatabase/"]
RUN dotnet restore "SubscriberDatabase/SubscriberDatabase.csproj"
COPY . .
WORKDIR "/src/SubscriberDatabase"
RUN dotnet build "./SubscriberDatabase.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SubscriberDatabase.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubscriberDatabase.dll"]
