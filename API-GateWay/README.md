docker pull prom/prometheus

docker run -d -p 9090:9090 -v $(pwd)/prometheus.yml:/etc/prometheus/prometheus.yml prom/prometheus

docker pull hashicorp/consul

docker run --name=consul -d -p 8500:8500 hashicorp/consul

docker run --rm -p 8500:8500 \
-v $(pwd)/config.yaml:/otel-local-config.yaml \
-e OTEL_CONFIG_FILE=/otel-local-config.yaml \
otel/opentelemetry-collector-contrib:latest
