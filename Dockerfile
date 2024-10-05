FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY app.tar .

# descompacta os arquivos
RUN tar -xvf app.tar -C .

# [preciso entender a necessidade desse copy]
# Copy everything else and build
COPY . ./

RUN dotnet restore thornament.sln

# roda o publish
RUN dotnet publish ./src/Host/App.Host/App.Host.csproj -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

#ENV ASPNETCORE_URLS=http://+:80 

# Enable the tracer
# ENV CORECLR_ENABLE_PROFILING=1
# ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
# ENV CORECLR_PROFILER_PATH=/opt/datadog/Datadog.Trace.ClrProfiler.Native.so
# ENV DD_DOTNET_TRACER_HOME=/opt/datadog
# ENV DD_INTEGRATIONS=/opt/datadog/integrations.json

# ENV DD_VERSION=1.0.0
# ENV DD_APM_ENABLED=true
# ENV DD_SERVICE=thornament
# ENV DD_AGENT_HOST=datadog-agent
# ENV DD_APM_NON_LOCAL_TRAFFIC=true
# ENV DD_RUNTIME_METRICS_ENABLED=true
# ENV DD_DOGSTATSD_NON_LOCAL_TRAFFIC=true
# ENV DD_TRACE_ROUTE_TEMPLATE_RESOURCE_NAMES_ENABLED=true


# Install dependencies
# RUN apt-get update \
#     && apt-get install -y libldap-2.4-2 \
#     && apt-get install -y curl

# # Download and install the Tracer
# RUN mkdir -p /opt/datadog \
#     && mkdir -p /var/log/datadog \
#     && TRACER_VERSION=2.46.0 \
#     # TODO: Ajustar para sempre obter a ultima versao
#     #&& TRACER_VERSION=$(curl -s https://api.github.com/repos/DataDog/dd-trace-dotnet/releases/latest --insecure | grep tag_name | cut -d '"' -f 4 | cut -c2-) \
#     && curl -LO https://github.com/DataDog/dd-trace-dotnet/releases/download/v${TRACER_VERSION}/datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
#     && dpkg -i ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb \
#     && rm ./datadog-dotnet-apm_${TRACER_VERSION}_amd64.deb

# copia a pasta out da camada build-env para /app dessa camada 
COPY --from=build-env /app/out .

EXPOSE 80

# ENTRYPOINT ["dotnet", "App.Host.dll", "--urls \"http://*:80\""]
ENTRYPOINT ["dotnet", "App.Host.dll"]