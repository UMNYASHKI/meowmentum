ARG GRAFANA_PROMTAIL_VERSION=latest

FROM grafana/promtail:${GRAFANA_PROMTAIL_VERSION}

COPY ./promtail-config.yaml /etc/promtail/config.yml

ENTRYPOINT ["/usr/bin/promtail"]
CMD ["-config.file=/etc/promtail/config.yml", "-config.expand-env=true"]
