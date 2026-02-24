# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files first (better layer caching)
COPY Cryptomind.sln ./
COPY Cryptomind/Cryptomind.csproj ./Cryptomind/
COPY Cryptomind.Common/Cryptomind.Common.csproj ./Cryptomind.Common/
COPY Cryptomind.Core/Cryptomind.Core.csproj ./Cryptomind.Core/
COPY Cryptomind.Data/Cryptomind.Data.csproj ./Cryptomind.Data/

# Restore dependencies
RUN dotnet restore Cryptomind/Cryptomind.csproj

# Copy the rest of the source code
COPY . ./

# Build and publish
RUN dotnet publish Cryptomind/Cryptomind.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "Cryptomind.dll"]
