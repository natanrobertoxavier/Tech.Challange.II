FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

COPY Tech.Challenge.I/appsettings.json /app/

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /main
COPY ["Tech.Challenge.I.Api/Tech.Challenge.I.Api.csproj", "Tech.Challenge.I.Api/"]
RUN dotnet restore "Tech.Challenge.I.Api/Tech.Challenge.I.Api.csproj"
COPY . .

RUN dotnet build "Tech.Challenge.I.Api/Tech.Challenge.I.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tech.Challenge.I.Api/Tech.Challenge.I.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tech.Challenge.I.Api.dll"]
