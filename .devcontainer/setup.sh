#!/bin/bash

# Install dotnet tools
dotnet tool install --global dotnet-ef --version 7.0.1

# Update apt and install dependencies
sudo apt update && sudo apt update && sudo apt install -y python3-pip libsqlite3-dev sqlitebrowser python3.11-venv

# Setup Python environment and install dependencies
sudo python3 -m venv .venv
source .venv/bin/activate
python3 -m pip install flask
