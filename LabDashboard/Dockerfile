FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files from the root context
COPY ["LabDashboard/LabDashboard.csproj", "LabDashboard/"]
COPY ["LabServer/LabServer.csproj", "LabServer/"]

# Restore using the dashboard project
RUN dotnet restore "LabDashboard/LabDashboard.csproj"

# Copy everything from the root
COPY . .

# Build and Publish
WORKDIR "/src/LabDashboard"
RUN dotnet publish -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "LabDashboard.dll"]