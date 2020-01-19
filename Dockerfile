FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
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