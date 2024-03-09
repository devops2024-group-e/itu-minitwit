#!/bin/bash

docker login ghcr.io -u "${DOCKER_USERNAME}" -p "${DOCKER_PASSWORD}"
docker compose pull
docker compose up minitwit-backend -d
docker compose up minitwit-frontend -d
