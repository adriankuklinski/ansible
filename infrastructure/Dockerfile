FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Ansible.API/Ansible.API.csproj", "src/Ansible.API/"]
COPY ["src/Ansible.Domain/Ansible.Domain.csproj", "src/Ansible.Domain/"]
COPY ["src/Ansible.Infrastructure/Ansible.Infrastructure.csproj", "src/Ansible.Infrastructure/"]
COPY ["src/Ansible.Host/Ansible.Host.csproj", "src/Ansible.Host/"]
RUN dotnet restore "src/Ansible.Host/Ansible.Host.csproj"
COPY . .
WORKDIR "/src/src/Ansible.Host"
RUN dotnet build "Ansible.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Ansible.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create non-root user for security
RUN addgroup --system --gid 1000 appgroup && \
    adduser --system --uid 1000 --ingroup appgroup --shell /bin/sh appuser && \
    chown -R appuser:appgroup /app
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=3s --retries=3 CMD wget -qO- http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "Ansible.Host.dll"]