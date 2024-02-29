#!/bin/bash

docker build  -f Dockerfile.database . -t ghcr.io/devops2024-group-e/database.minitwit:latest
docker build  -f Dockerfile.frontend . -t ghcr.io/devops2024-group-e/frontend.minitwit:latest
docker build  -f Dockerfile.backend . -t ghcr.io/devops2024-group-e/backend.minitwit:latest
