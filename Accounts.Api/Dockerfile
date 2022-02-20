#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Accounts.Api/Accounts.Api.csproj", "Accounts.Api/"]
COPY ["Accounts.ReadModel/Accounts.ReadModel.csproj", "Accounts.ReadModel/"]
COPY ["Accounts.Domain/Accounts.Domain.csproj", "Accounts.Domain/"]
COPY ["Accounts.Infrastructure/Accounts.Infrastructure.csproj", "Accounts.Infrastructure/"]
COPY ["Accounts.Application/Accounts.Application.csproj", "Accounts.Application/"]
RUN dotnet restore "Accounts.Api/Accounts.Api.csproj"
COPY . .
WORKDIR "/src/Accounts.Api"
RUN dotnet build "Accounts.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Accounts.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Accounts.Api.dll"]