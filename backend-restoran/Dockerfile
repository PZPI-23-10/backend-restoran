FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["backend-restoran/backend-restoran.csproj", "backend-restoran/"]
RUN dotnet restore "backend-restoran/backend-restoran.csproj"

COPY . .
WORKDIR "/src/backend-restoran"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "backend-restoran.dll"]
