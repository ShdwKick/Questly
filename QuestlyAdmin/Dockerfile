﻿FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["QuestlyAdmin/QuestlyAdmin.csproj", "QuestlyAdmin/"]
COPY ["Questly.Domain/Questly.Domain.csproj", "Questly.Domain/"]
RUN dotnet restore "QuestlyAdmin/QuestlyAdmin.csproj"
COPY . .
WORKDIR "/src/QuestlyAdmin"
RUN dotnet build "QuestlyAdmin.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "QuestlyAdmin.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuestlyAdmin.dll"]
