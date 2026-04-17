FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Frontend.csproj ./
RUN dotnet restore Frontend.csproj

COPY . ./
RUN dotnet publish Frontend.csproj -c Release -o /app/publish

FROM nginx:alpine AS runtime
WORKDIR /usr/share/nginx/html

COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/publish/wwwroot ./
