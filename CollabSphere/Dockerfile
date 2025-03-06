FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CollabSphere.csproj", "."]
ENV HUSKY=0
RUN dotnet restore "CollabSphere.csproj"
COPY . .
RUN dotnet build "CollabSphere.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CollabSphere.csproj" -c Release -o /app/publish

FROM base AS final
LABEL maintainer="ngntu10"
LABEL name="collabsphere"
LABEL version="1.0"
LABEL description="CollabSphere - A Collaboration Platform"
WORKDIR /app

# Cài đặt mysqladmin cho script wait-for-mysql
RUN apt-get update && apt-get install -y default-mysql-client

COPY wait-for-mysql.sh /wait-for-mysql.sh
RUN chmod +x /wait-for-mysql.sh

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CollabSphere.dll"]
