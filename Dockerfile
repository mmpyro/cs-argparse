# Use the official .NET SDK image for building and testing
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the entire source code
COPY . .

# Restore dependencies for the test project (which will also restore ArgParse)
RUN dotnet restore ArgParseSpec/ArgParseSpec.csproj

# Build the projects
RUN dotnet build ArgParse/ArgParse.csproj -c Release --no-restore
RUN dotnet build ArgParseSpec/ArgParseSpec.csproj -c Release --no-restore

# Run tests
FROM build AS test
WORKDIR /app/ArgParseSpec/bin/Release/net8.0
ENTRYPOINT ["dotnet", "test", "ArgParseSpec.dll", "--verbosity", "normal"]