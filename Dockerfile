FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HairSalon.csproj", "./"]
RUN dotnet restore "HairSalon.csproj"

COPY . .

RUN dotnet publish "HairSalon.csproj" -c Release -o /app/publish \
    -p:CopyRazorGenerateFilesToPublishDirectory=true

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

RUN echo "Checking Pages directory:" && \
    ls -la Pages/ 2>/dev/null || echo "Pages not found!" && \
    find . -name "_Host*" 2>/dev/null || echo "_Host not found!"

ENTRYPOINT ["dotnet", "HairSalon.dll"]