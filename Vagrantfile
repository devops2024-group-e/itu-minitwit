# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure("2") do |config|
  config.vm.box = 'digital_ocean'
  config.vm.box_url = "https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box"
  config.ssh.private_key_path = '~/.ssh/do_ssh_key'

  config.vm.synced_folder '.', '/vagrant', disabled: true

  config.vm.define "minitwit-conf", primary: true do |server|

    server.vm.provider :digital_ocean do |provider|
      provider.ssh_key_name = ENV["DIGITAL_OCEAN_SSH_KEY_NAME"]
      provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
      provider.image = 'ubuntu-22-04-x64'
      provider.region = 'fra1'
      provider.size = 's-1vcpu-1gb'
    end

    server.vm.hostname = "minitwit-conf-01"
    server.vm.synced_folder "./.infrastructure", "/config-management", type: "rsync"

    server.vm.provision "shell", inline: 'echo "export DOCKER_USERNAME=' + "'" + ENV["DOCKER_USERNAME"] + "'" + '" >> ~/.bash_profile'
    server.vm.provision "shell", inline: 'echo "export DOCKER_PASSWORD=' + "'" + ENV["DOCKER_PASSWORD"] + "'" + '" >> ~/.bash_profile'

    server.vm.provision "shell", inline: 'echo "export DIGITAL_OCEAN_TOKEN=' + "'" + ENV["DIGITAL_OCEAN_TOKEN"] + "'" + '" >> ~/.bash_profile'

    server.vm.provision "shell", inline: <<-SHELL

    sudo apt-get update
    #sudo killall apt apt-get
    sudo rm /var/lib/dpkg/lock-frontend
    sudo apt-get update

    sudo apt-get install -y software-properties-common
    sudo apt-add-repository ppa:ansible/ansible
    sudo apt-get install -y ansible
    sudo apt-get install -y vagrant
    sudo apt-get install -y vagrant-scp
    sudo apt-get install -y vagrant-digital-ocean
    sudo apt-get install -y vagrant-vbguest
    sudo apt-get install -y vagrant-reload


    echo -e "\nVerifying correct download" 
    ansible --version

    echo ". $HOME/.bashrc" >> $HOME/.bash_profile

    echo -e "\nConfiguring credentials as environment variables...\n"
    source $HOME/.bash_profile

    echo -e "\nSelecting Minitwit Folder as default folder when you ssh into the server...\n"
    echo "cd /config-management" >> ~/.bash_profile

    echo -e "\nGenerating sshkey"
    ssh-keygen -f ~/.ssh/do_ssh_key -N ""

    echo -e "\nSetting up "

    echo -e "\nVagrant setup done ..."
    SHELL
  end
end
