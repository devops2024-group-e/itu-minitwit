# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure("2") do |config|
  config.vm.box = 'digital_ocean'
  config.vm.box_url = "https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box"
  config.ssh.private_key_path = '~/.ssh/do_ssh_key'

  config.vm.synced_folder "./remote-server", "/minitwit", type: "rsync"
  config.vm.synced_folder '.', '/vagrant', disabled: true

  config.vm.define "minitwit", primary: true do |server|

    server.vm.provider :digital_ocean do |provider|
      provider.ssh_key_name = ENV["DIGITAL_OCEAN_SSH_KEY_NAME"]
      provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
      provider.image = 'ubuntu-22-04-x64'
      provider.region = 'fra1'
      provider.size = 's-1vcpu-1gb'
    end

    server.vm.hostname = "minitwit-web-01"

    server.vm.provision "shell", inline: 'echo "export DOCKER_USERNAME=' + "'" + ENV["DOCKER_USERNAME"] + "'" + '" >> ~/.bash_profile'
    server.vm.provision "shell", inline: 'echo "export DOCKER_PASSWORD=' + "'" + ENV["DOCKER_PASSWORD"] + "'" + '" >> ~/.bash_profile'

    server.vm.provision "shell", inline: <<-SHELL

    sudo apt-get update

    # The following address an issue in DO's Ubuntu images, which still contain a lock file
    #sudo killall apt apt-get
    sudo rm /var/lib/dpkg/lock-frontend

    # Install docker and docker compose
    sudo apt-get install -y docker.io docker-compose-v2

    sudo systemctl status docker
    # sudo usermod -aG docker ${USER}

    echo -e "\nVerifying that docker works ...\n"
    docker run --rm hello-world
    docker rmi hello-world

    echo -e "\nOpening port for minitwit ...\n"
    ufw allow 80 && \
    ufw allow 22/tcp && \
    ufw allow 8080

    echo ". $HOME/.bashrc" >> $HOME/.bash_profile

    echo -e "\nConfiguring credentials as environment variables...\n"

    source $HOME/.bash_profile

    echo -e "\nSelecting Minitwit Folder as default folder when you ssh into the server...\n"
    echo "cd /minitwit" >> ~/.bash_profile

    chmod +x /minitwit/deploy.sh
    chmod +x /minitwit/init.sh

    echo -e "\nVagrant setup done ..."
    echo -e "minitwit will later be accessible at http://$(hostname -I | awk '{print $1}'):80"
    echo -e "The postgres sql database needs a minute to initialize, if the landing page shows an error stack-trace ..."
    SHELL
  end
end
