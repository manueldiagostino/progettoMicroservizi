FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5134

ENV ASPNETCORE_URLS=http://+:5134

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["AuthorsHandler/AuthorsHandler.Api/AuthorsHandler.Api.csproj", "AuthorsHandler/AuthorsHandler.Api/"]
RUN dotnet restore "AuthorsHandler/AuthorsHandler.Api/AuthorsHandler.Api.csproj"
COPY . .
WORKDIR "/src/AuthorsHandler/AuthorsHandler.Api"
RUN dotnet build "AuthorsHandler.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "AuthorsHandler.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthorsHandler.Api.dll"]
