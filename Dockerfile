# ===== Base image for runtime (.NET 9) =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# ===== Build image (.NET 9 SDK) =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj เพื่อ cache restore
COPY src/Domain/*.csproj src/Domain/
COPY src/Application/*.csproj src/Application/
COPY src/Infrastructure/*.csproj src/Infrastructure/
COPY src/Api/*.csproj src/Api/

RUN dotnet restore src/Api/Api.csproj

# copy source ทั้งหมดแล้ว build/publish
COPY . .
RUN dotnet build src/Api/Api.csproj -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish src/Api/Api.csproj -c Release -o /app/publish --no-restore

# ===== Final =====
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./
ENTRYPOINT ["dotnet", "Api.dll"]
