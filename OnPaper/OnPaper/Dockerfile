# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy and restore the server project
COPY ["OnPaper/OnPaper.csproj", "OnPaper/"]
COPY ["OnPaper.Client/OnPaper.Client.csproj", "OnPaper.Client/"]
RUN dotnet restore "OnPaper/OnPaper.csproj"

# Copy the entire solution and build
COPY . .
WORKDIR "/src/OnPaper"
RUN dotnet build "OnPaper.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "OnPaper.csproj" -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OnPaper.dll"]