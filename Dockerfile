FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Listrr/Listrr.csproj", "Listrr/"]
RUN dotnet restore "Listrr/Listrr.csproj"
COPY . .
WORKDIR "/src/Listrr"
RUN dotnet build "Listrr.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Listrr.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Listrr.dll"]