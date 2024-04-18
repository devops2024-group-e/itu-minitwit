# Infrastructure
This directory is where all the configuration about the infrastructure is located. We have the following three directories:

- **Servers**: This contains the directory of servers files that will be synced to the actual server
- **Minitwit.Provision**: Contains code that describes our current infrastructure
- **Configuration**: Configuration is where all the configuration scripts for the servers are. This means setting up servers with firewall settings, creating the docker swarm cluster, installing server packages, and etc. All of this is managed by Ansible.

The code and configuration files are being used by the workflow `provision-and-deploy.yaml`. It does the following:
- Builds docker images
- Creates the cloud infrastructure (Minitwit.Provision)
- Ensure that the servers state is correct and deploys (Uses playbooks in `./configuration` and syncs the files in `servers`)
