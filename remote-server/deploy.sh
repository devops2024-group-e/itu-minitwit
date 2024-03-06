#!/bin/bash

docker login ghcr.io -u "${DOCKER_USERNAME}" -p "${DOCKER_PASSWORD}"
docker run --platform linux/amd64 -p 80:80 -d ghcr.io/devops2024-group-e/frontend.minitwit
docker run --platform linux/amd64 -p 8080:8080 -d ghcr.io/devops2024-group-e/backend.minitwit
