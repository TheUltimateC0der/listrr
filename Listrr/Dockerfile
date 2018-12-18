FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Listrr.csproj", "Listrr/"]
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