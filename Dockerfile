FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["Listrr/Listrr.csproj", "Listrr/"]
RUN dotnet restore "Listrr/Listrr.csproj"
COPY . .
WORKDIR "/src/Listrr"
RUN dotnet build "Listrr.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Listrr.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Listrr.dll"]