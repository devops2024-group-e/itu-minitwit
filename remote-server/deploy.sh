#!/bin/bash

echo $DOCKER_PASSWORD | docker login ghcr.io -u $DOCKER_USERNAME --password-stdin
docker run --platform linux/amd64 ghcr.io/devops2024-group-e/frontend.minitwit
docker run --platform linux/amd64 ghcr.io/devops2024-group-e/backend.minitwit
