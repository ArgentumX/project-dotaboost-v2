# backend/Dockerfile  (вариант B — контекст: ./backend)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy only csproj files (relative to context=backend)
COPY src/WebApi/WebApi.csproj WebApi/
COPY src/Application/Application.csproj Application/
COPY src/Domain/Domain.csproj Domain/
COPY src/Infrastructure/Infrastructure.csproj Infrastructure/

# restore using relative path inside container
RUN dotnet restore WebApi/WebApi.csproj

# copy the rest of the project sources INTO /src (copy only src/ so не создаём лишнюю вложенность)
COPY src/ .

# build
WORKDIR /src/WebApi
RUN dotnet build WebApi.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish WebApi.csproj --no-restore -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
ENV ASPNETCORE_URLS=http://+:5001
EXPOSE 5001
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
