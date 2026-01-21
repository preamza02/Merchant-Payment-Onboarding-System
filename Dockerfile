FROM mcr.microsoft.com/dotnet/sdk:10.0.100 AS build

WORKDIR /src

COPY src/ src/

RUN dotnet restore src/MerchantPayment.API/MerchantPayment.API.csproj

RUN dotnet publish src/MerchantPayment.API/MerchantPayment.API.csproj \
    -c Release \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "MerchantPayment.API.dll"]
