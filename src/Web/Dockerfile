FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Web/Web.csproj", "Web/"]
COPY ["Common/Common.csproj", "Common/"]
COPY ["Operations/Operations.csproj", "Operations/"]
COPY ["ShoppingLists/ShoppingLists.csproj", "ShoppingLists/"]
COPY ["Identity/Identity.csproj", "Identity/"]
COPY ["ApplicationServices/ApplicationServices.csproj", "ApplicationServices/"]
RUN dotnet restore "Web/Web.csproj"
COPY . .
WORKDIR /src/Web
RUN dotnet build "Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.dll"]