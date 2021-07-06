#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build

ARG CONFIGURATION=Release

WORKDIR /src
COPY ["FileTranslationService/FileTranslationService.csproj", "FileTranslationService/"]
RUN dotnet restore "FileTranslationService/FileTranslationService.csproj"
COPY . .
WORKDIR "/src/FileTranslationService"
RUN dotnet build "FileTranslationService.csproj" -c $CONFIGURATION -o /app/build

FROM build AS publish

ARG CONFIGURATION=Release

RUN dotnet publish "FileTranslationService.csproj" -c $CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Tilde.MT.FileTranslationService.dll"]