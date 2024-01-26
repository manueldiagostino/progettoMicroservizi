FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5134

ENV ASPNETCORE_URLS=http://+:5134

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["MusicalScoresHandler/MusicalScoresHandler.Api/MusicalScoresHandler.Api.csproj", "MusicalScoresHandler/MusicalScoresHandler.Api/"]
RUN dotnet restore "MusicalScoresHandler/MusicalScoresHandler.Api/MusicalScoresHandler.Api.csproj"
COPY . .
WORKDIR "/src/MusicalScoresHandler/MusicalScoresHandler.Api"
RUN dotnet build "MusicalScoresHandler.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "MusicalScoresHandler.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MusicalScoresHandler.Api.dll"]
