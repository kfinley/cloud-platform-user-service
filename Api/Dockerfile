FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine AS builder

WORKDIR /source

# Copy source code and compile
COPY ./ ./

RUN dotnet publish --configuration ${BUILD_CONFIG} -o build

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS runner

WORKDIR /app

COPY --from=build /source/build .

ENTRYPOINT ["dotnet", "User.Api.dll"]
