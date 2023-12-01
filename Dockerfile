FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Classes.Api/Classes.Api.csproj", "Classes.Api/"]
COPY ["Classes.Domain/Classes.Domain.csproj", "Classes.Domain/"]
COPY ["Classes.Data/Classes.Data.csproj", "Classes.Data/"]

RUN dotnet restore "Classes.Api/Classes.Api.csproj" --verbosity detailed

COPY . .
WORKDIR /src/Classes.Api

RUN dotnet build "Classes.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Classes.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Classes.Api.dll"]