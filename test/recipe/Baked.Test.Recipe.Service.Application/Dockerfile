# syntax=docker/dockerfile:1.7-labs

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

ARG ENVIRONMENT=Production

EXPOSE 80
ENV ASPNETCORE_ENVIRONMENT=$ENVIRONMENT
ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish

COPY --exclude=**/*.cs --exclude=**/*.json . .
RUN dotnet restore

COPY . .
RUN dotnet publish ./test/recipe/Baked.Test.Recipe.Service.Application -c Release -o /app/publish

FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
CMD dotnet Baked.Test.Recipe.Service.Application.dll
