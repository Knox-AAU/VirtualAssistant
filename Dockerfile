FROM mcr.microsoft.com/dotnet/sdk:5.0  AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
# the csproj references requires this structure
COPY VirtualAssistantAPI/VirtualAssistantAPI/VirtualAssistantAPI.csproj ./VirtualAssistantAPI/VirtualAssistantAPI/
COPY VirtualAssistantBusinessLogic/VirtualAssistantBusinessLogic/VirtualAssistantBusinessLogic.csproj ./VirtualAssistantBusinessLogic/VirtualAssistantBusinessLogic/ 

# Restore metadata and dependencies
RUN dotnet restore ./VirtualAssistantBusinessLogic/VirtualAssistantBusinessLogic/VirtualAssistantBusinessLogic.csproj  &&\
 dotnet restore ./VirtualAssistantAPI/VirtualAssistantAPI/VirtualAssistantAPI.csproj

# Copy everything else and build
COPY . ./
RUN dotnet publish ./VirtualAssistantAPI/VirtualAssistantAPI/VirtualAssistantAPI.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "VirtualAssistantAPI.dll"]
