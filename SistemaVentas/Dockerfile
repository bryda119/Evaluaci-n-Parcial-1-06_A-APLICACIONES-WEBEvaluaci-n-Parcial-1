FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SistemaVentas.csproj", "."]
RUN dotnet restore "SistemaVentas.csproj"
COPY . .
RUN dotnet publish "SistemaVentas.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN mkdir -p /app/data
ENV ASPNETCORE_URLS=http://+:8080
ENV ConnectionStrings__DefaultConnection="Data Source=/app/data/ventas.db"
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SistemaVentas.dll"]
