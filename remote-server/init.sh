#!/bin/bash

docker login ghcr.io -u "${DOCKER_USERNAME}" -p "${DOCKER_PASSWORD}"
docker run -p 5432:5432 --name database -d ghcr.io/devops2024-group-e/database.minitwit:latest
docker run --platform linux/amd64 -p 80:80 --name frontend -d ghcr.io/devops2024-group-e/frontend.minitwit:latest
docker run --platform linux/amd64 -p 8080:8080 --name backend -d ghcr.io/devops2024-group-e/backend.minitwit:latest
