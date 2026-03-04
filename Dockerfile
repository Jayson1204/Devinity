FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LearningAppAPI/LearningApp.Api.csproj", "LearningAppAPI/"]
RUN dotnet restore "LearningAppAPI/LearningApp.Api.csproj"
COPY . .
WORKDIR "/src/LearningAppAPI"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LearningApp.Api.dll"]