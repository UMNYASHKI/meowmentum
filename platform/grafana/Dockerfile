ARG GRAFANA_VERSION=latest

FROM grafana/grafana:${GRAFANA_VERSION}

RUN grafana-cli plugins install grafana-lokiexplore-app

COPY ./grafana-dashboards.yml /etc/grafana/provisioning/dashboards/grafana-dashboards.yml
COPY ./grafana-datasources.yml /etc/grafana/provisioning/datasources/grafana-datasources.yml
COPY ./grafana-dashboards /etc/grafana/dashboards
COPY ./grafana-config.ini /etc/grafana/grafana.ini
