# ---------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# ---------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

ARG CONFIGURATION=Release

WORKDIR /src
COPY ["FileTranslationService/FileTranslationService.csproj", "FileTranslationService/"]
COPY ["FileTranslationService.Tests/FileTranslationService.Tests.csproj", "FileTranslationService.Tests/"]
RUN dotnet restore "FileTranslationService/FileTranslationService.csproj"
RUN dotnet restore "FileTranslationService.Tests/FileTranslationService.Tests.csproj"
COPY . .
WORKDIR "/src/FileTranslationService"
RUN dotnet build "FileTranslationService.csproj" -c $CONFIGURATION -o /app/build

# ---------------------------------------------------
FROM build AS tested-build

ARG CONFIGURATION=Release

WORKDIR "/src/FileTranslationService.Tests"
RUN dotnet test -c $CONFIGURATION

# ---------------------------------------------------
FROM tested-build AS publish

ARG CONFIGURATION=Release

WORKDIR "/src/FileTranslationService"
RUN dotnet publish "FileTranslationService.csproj" -c $CONFIGURATION -o /app/publish

# ---------------------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Tilde.MT.FileTranslationService.dll"]