FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["raBudget.Api/raBudget.Api.csproj", "raBudget.Api/"]
RUN dotnet restore "raBudget.Api/raBudget.Api.csproj"
COPY . .
WORKDIR "/src/raBudget.Api"
RUN dotnet build "raBudget.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "raBudget.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "raBudget.Api.dll"]