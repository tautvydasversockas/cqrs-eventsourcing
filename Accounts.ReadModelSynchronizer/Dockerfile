FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Accounts.ReadModelSynchronizer/Accounts.ReadModelSynchronizer.csproj", "Accounts.ReadModelSynchronizer/"]
COPY ["Accounting.Common/Accounting.Common.csproj", "Accounting.Common/"]
COPY ["Accounts.Domain/Accounts.Domain.csproj", "Accounts.Domain/"]
COPY ["Accounts.ReadModel/Accounts.ReadModel.csproj", "Accounts.ReadModel/"]
COPY ["Accounts.Infrastructure/Accounts.Infrastructure.csproj", "Accounts.Infrastructure/"]
COPY ["Accounts.Application/Accounts.Application.csproj", "Accounts.Application/"]
RUN dotnet restore "Accounts.ReadModelSynchronizer/Accounts.ReadModelSynchronizer.csproj"
COPY . .
WORKDIR "/src/Accounts.ReadModelSynchronizer"
RUN dotnet build "Accounts.ReadModelSynchronizer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Accounts.ReadModelSynchronizer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Accounts.ReadModelSynchronizer.dll"]