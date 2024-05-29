#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TicketServices.Api/TicketServices.Api.csproj", "TicketServices.Api/"]
COPY ["IoCmanage/IoCmanage.csproj", "IoCmanage/"]
COPY ["MovieServices.Application/MovieServices.Application.csproj", "MovieServices.Application/"]
COPY ["MovieServices.Domain/MovieServices.Domain.csproj", "MovieServices.Domain/"]
COPY ["ApecMovieCore/ApecMovieCore.csproj", "ApecMovieCore/"]
COPY ["RabbitMQ/RabbitMQ.csproj", "RabbitMQ/"]
COPY ["MovieServices.Infrastructure/MovieServices.Infrastructure.csproj", "MovieServices.Infrastructure/"]
COPY ["TicketServices.Application/TicketServices.Application.csproj", "TicketServices.Application/"]
COPY ["TicketServices.Domain/TicketServices.Domain.csproj", "TicketServices.Domain/"]
COPY ["TicketServices.Infrastructure/TicketServices.Infrastructure.csproj", "TicketServices.Infrastructure/"]
COPY ["UserServices.Application/UserServices.Application.csproj", "UserServices.Application/"]
COPY ["ApecCoreIdentity/ApecCoreIdentity.csproj", "ApecCoreIdentity/"]
COPY ["UserServices.Domain/UserServices.Domain.csproj", "UserServices.Domain/"]
COPY ["UserServices.Infrastructure/UserServices.Infrastructure.csproj", "UserServices.Infrastructure/"]
COPY ["SwaggerDoc/SwaggerDoc.csproj", "SwaggerDoc/"]
RUN dotnet restore "./TicketServices.Api/TicketServices.Api.csproj"
COPY . .
WORKDIR "/src/TicketServices.Api"
RUN dotnet build "./TicketServices.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TicketServices.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TicketServices.Api.dll"]