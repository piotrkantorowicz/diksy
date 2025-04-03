FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/Diksy.WebApi/Diksy.WebApi.csproj", "src/Diksy.WebApi/"]
COPY ["src/Diksy.Translation.OpenAI/Diksy.Translation.OpenAI.csproj", "src/Diksy.Translation.OpenAI/"]
COPY ["src/Diksy.Translation/Diksy.Translation.csproj", "src/Diksy.Translation/"]

RUN dotnet restore "src/Diksy.WebApi/Diksy.WebApi.csproj" 

COPY . .

RUN dotnet publish "src/Diksy.WebApi/Diksy.WebApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

EXPOSE 80

ENTRYPOINT ["dotnet", "Diksy.WebApi.dll"] 