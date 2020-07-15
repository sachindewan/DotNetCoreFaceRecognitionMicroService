#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:5.0-buster AS build
WORKDIR /src
COPY ["CustomerNotification/notificationService/notificationService.csproj", "CustomerNotification/notificationService/"]
COPY ["CustomerNotification/EmailService/EmailService.csproj", "CustomerNotification/EmailService/"]
COPY ["Messaging.InterfacesConstants/Messaging.InterfacesConstants.csproj", "Messaging.InterfacesConstants/"]
RUN dotnet restore "CustomerNotification/notificationService/notificationService.csproj"
COPY . .
WORKDIR "/src/CustomerNotification/notificationService"
RUN dotnet build "notificationService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "notificationService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "notificationService.dll"]
