# Stage 1: Build Angular Client
FROM node:20-alpine AS client-build
WORKDIR /app/client

# Copy client package files and install dependencies
COPY client/package*.json ./
RUN npm ci --legacy-peer-deps

# Copy client source and build
COPY client/ ./
RUN npm run build -- --configuration production --optimization=false

# Stage 2: Build .NET Server
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS server-build
WORKDIR /app/server

# Copy solution and project files
COPY server/server.sln ./
COPY server/server/*.csproj ./server/

# Restore dependencies
RUN dotnet restore

# Copy server source and build
COPY server/ ./
RUN dotnet publish server/server.csproj -c Release -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published server
COPY --from=server-build /app/publish .

# Copy built Angular client to wwwroot
# העתקה של התוכן מתוך תיקיית ה-browser ישירות ל-wwwroot
COPY --from=client-build /app/client/dist/client/browser ./wwwroot

# Expose port
EXPOSE 80
EXPOSE 443

# Set environment variable
ENV ASPNETCORE_URLS=http://+:80

# Run the application
ENTRYPOINT ["dotnet", "server.dll"]
