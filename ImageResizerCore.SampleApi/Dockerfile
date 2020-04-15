#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ImageResizerCore.SampleApi/ImageResizerCore.SampleApi.csproj", "ImageResizerCore.SampleApi/"]
RUN dotnet restore "ImageResizerCore.SampleApi/ImageResizerCore.SampleApi.csproj"
COPY . .
WORKDIR "/src/ImageResizerCore.SampleApi"
RUN dotnet build "ImageResizerCore.SampleApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ImageResizerCore.SampleApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ImageResizerCore.SampleApi.dll"]