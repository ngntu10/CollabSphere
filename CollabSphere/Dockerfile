FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /CollabSphere
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CollabSphere.csproj", "."]
ENV HUSKY=0
RUN dotnet restore "CollabSphere.csproj"
COPY . .
RUN dotnet build "CollabSphere.csproj" -c Release -o /CollabSphere/build

FROM build AS publish
RUN dotnet publish "CollabSphere.csproj" -c Release -o /CollabSphere/publish

FROM base AS final
LABEL maintainer="ngntu10"
LABEL name="collabsphere"
LABEL version="1.0"
LABEL description="CollabSphere - A Collaboration Platform"
WORKDIR /CollabSphere

RUN apt-get update && apt-get install -y default-mysql-client

# Copy published files first
COPY --from=publish /CollabSphere/publish .

# Then explicitly copy the Shared directory to ensure templates are included
COPY Shared /CollabSphere/Shared

ENTRYPOINT ["dotnet", "CollabSphere.dll"]
