# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY Cryptomind.sln ./
COPY Cryptomind/Cryptomind.csproj ./Cryptomind/
COPY Cryptomind.Common/Cryptomind.Common.csproj ./Cryptomind.Common/
COPY Cryptomind.Core/Cryptomind.Core.csproj ./Cryptomind.Core/
COPY Cryptomind.Data/Cryptomind.Data.csproj ./Cryptomind.Data/
RUN dotnet restore Cryptomind/Cryptomind.csproj
COPY . ./
RUN dotnet publish Cryptomind/Cryptomind.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
COPY --from=build /app/Images ./Images
EXPOSE 8080
ENTRYPOINT ["dotnet", "Cryptomind.dll"]