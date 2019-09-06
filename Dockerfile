FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
COPY ./src/ApiGateway/bin/Release/netcoreapp2.2 /app
WORKDIR /app
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
EXPOSE 80