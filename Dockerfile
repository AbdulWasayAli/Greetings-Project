# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy project files and restore
COPY E-Project/*.csproj ./E-Project/
WORKDIR /source/E-Project
RUN dotnet restore

# Copy all files and publish release
WORKDIR /source
COPY E-Project/ ./E-Project/
WORKDIR /source/E-Project
RUN dotnet publish -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose HTTP port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "E-Project.dll"]

