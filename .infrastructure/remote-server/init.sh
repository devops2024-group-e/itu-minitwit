#!/bin/bash

docker login ghcr.io -u "${DOCKER_USERNAME}" -p "${DOCKER_PASSWORD}"
docker compose -f /minitwit/docker-compose.yaml up -d
