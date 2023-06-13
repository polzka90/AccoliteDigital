FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Core.Domain/Core.Domain.csproj", "src/Core.Domain/"]
RUN dotnet restore "src/Core.Domain/Core.Domain.csproj"
COPY . .
WORKDIR "/src/src/Core.Domain"
RUN dotnet build "Core.Domain.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Core.Domain.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Core.Domain.dll"]
