# Stage 1: Base
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
# Render بيستخدم بورتات متغيرة، بس 8080 هو الافتراضي الجديد لـ .NET 8/9
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Stage 2: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# نسخ ملفات الـ csproj أولاً لعمل Restore (عشان الـ Caching يكون أسرع)
COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]

# عمل Restore للـ Presentation وهو هيسحب الباقي معاه تلقائياً
RUN dotnet restore "Presentation/Presentation.csproj"

# نسخ باقي الكود بالكامل
COPY . .

# الانتقال لفولدر المشروع الأساسي وعمل Build
WORKDIR "/src/Presentation"
RUN dotnet build "Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# تأكد إن اسم الـ DLL هو فعلاً Presentation.dll
ENTRYPOINT ["dotnet", "Presentation.dll"]