FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ENV TZ=America/Lima
COPY bin/Release/netcoreapp3.1/publish/ App/
WORKDIR /App

EXPOSE 80
ENTRYPOINT ["dotnet","MSRecursosHumanos.dll"]