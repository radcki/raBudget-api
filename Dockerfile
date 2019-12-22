FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

COPY *.sln .
COPY . .
RUN dotnet restore raBudget-Api.sln
RUN dotnet publish raBudget-Api.sln -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "raBudget.WebApi.dll"]