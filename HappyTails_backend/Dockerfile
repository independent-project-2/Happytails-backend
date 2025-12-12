# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy built files from build stage
COPY --from=build /app/out .

# Expose port
EXPOSE 8080

# Start the app
ENTRYPOINT ["dotnet", "HappyTails_backend.dll"]  
