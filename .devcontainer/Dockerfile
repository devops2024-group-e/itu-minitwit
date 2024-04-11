FROM mcr.microsoft.com/devcontainers/dotnet:7.0

WORKDIR /setup

COPY dotnet-sdk-setup.sh /setup
RUN chmod +x dotnet-sdk-setup.sh && ./dotnet-sdk-setup.sh

RUN apt-get update \
    && apt-get install -y --no-install-recommends python3=3.11.2-1+b1 python3-pip=23.0.1+dfsg-1 \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

RUN wget -q -O /bin/hadolint "https://github.com/hadolint/hadolint/releases/download/v2.10.0/hadolint-Linux-$(dpkg --print-architecture)" \
    && chmod +x /bin/hadolint; \
    pip3 install --no-cache-dir pre-commit==3.7.0 --break-system-packages
