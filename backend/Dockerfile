FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY .editorconfig ./
COPY backend/Directory.Build.props backend/Directory.Packages.props backend/
COPY backend/src/LoanFlow.Domain/LoanFlow.Domain.csproj backend/src/LoanFlow.Domain/
COPY backend/src/LoanFlow.Application/LoanFlow.Application.csproj backend/src/LoanFlow.Application/
COPY backend/src/LoanFlow.Infrastructure/LoanFlow.Infrastructure.csproj backend/src/LoanFlow.Infrastructure/
COPY backend/src/LoanFlow.Api/LoanFlow.Api.csproj backend/src/LoanFlow.Api/
RUN dotnet restore backend/src/LoanFlow.Api/LoanFlow.Api.csproj

COPY backend/src backend/src
RUN dotnet publish backend/src/LoanFlow.Api/LoanFlow.Api.csproj --configuration Release --no-restore --output /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app ./
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "LoanFlow.Api.dll"]
