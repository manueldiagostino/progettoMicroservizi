FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5134

ENV ASPNETCORE_URLS=http://+:5134

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["UsersHandler/UsersHandler.Api/UsersHandler.Api.csproj", "UsersHandler/UsersHandler.Api/"]
RUN dotnet restore "UsersHandler/UsersHandler.Api/UsersHandler.Api.csproj"
COPY . .
WORKDIR "/src/UsersHandler/UsersHandler.Api"
RUN dotnet build "UsersHandler.Api.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "UsersHandler.Api.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersHandler.Api.dll"]
