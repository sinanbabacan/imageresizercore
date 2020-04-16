FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["ImageResizerCore.SampleApi/ImageResizerCore.SampleApi.csproj", "ImageResizerCore.SampleApi/"]
RUN dotnet restore "ImageResizerCore.SampleApi/ImageResizerCore.SampleApi.csproj"
COPY . .
WORKDIR "/src/ImageResizerCore.SampleApi"
RUN dotnet build "ImageResizerCore.SampleApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ImageResizerCore.SampleApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

RUN apt-get update && apt-get install -y libfontconfig1

ENTRYPOINT ["dotnet", "ImageResizerCore.SampleApi.dll"]
