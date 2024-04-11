#!/bin/bash

wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
chmod +x ./dotnet-install.sh
./dotnet-install.sh --version latest --install-dir "/usr/share/dotnet-new"

sudo cp -u -r /usr/share/dotnet-new/* /usr/share/dotnet/
