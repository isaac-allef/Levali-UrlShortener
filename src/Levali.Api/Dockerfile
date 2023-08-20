FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
EXPOSE 80

RUN curl https://api.nuget.org/v3/index.json -v

COPY ["Levali.Api/*.csproj", "Levali.Api/"]
COPY ["Levali.Core/*.csproj", "Levali.Core/"]
COPY ["Levali.Infra/*.csproj", "Levali.Infra/"]

COPY . ./
RUN dotnet restore "Levali.Api/Levali.Api.csproj"
RUN dotnet publish "Levali.Api/Levali.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Levali.Api.dll"]
