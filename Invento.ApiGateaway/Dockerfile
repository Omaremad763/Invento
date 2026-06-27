FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
# عدلنا البورت هنا لـ 80 ليتوافق مع الـ docker-compose (5000:80)
EXPOSE 80 

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# التعديل الأول: تحديد المسار الصحيح للمشروع لأن البناء بيتم من الـ Root بره
COPY ["Invento.Gateway/Invento.Gateway.csproj", "Invento.Gateway/"]
RUN dotnet restore "./Invento.Gateway/Invento.Gateway.csproj"

COPY . .
# التعديل الثاني: التحرك جوه فولدر المشروع قبل عمل build
WORKDIR "/src/Invento.Gateway"
RUN dotnet build "./Invento.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Invento.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Invento.Gateway.dll"]