# Create a stage for building the application.
# Use the official .NET 8 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj file and restore dependencies (via dotnet restore)
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the code and build the app
COPY . ./
RUN dotnet publish -c Release -o out

# Generate the final image using the .NET 8 runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
COPY --from=build /app/out ./

# Expose the port the app runs on
EXPOSE 8080

ENTRYPOINT ["dotnet", "API-GateWay.dll"]
