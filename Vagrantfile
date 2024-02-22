# -*- mode: ruby -*-
# vi: set ft=ruby :

$ip_file = "db_ip.txt"

Vagrant.configure("2") do |config|
    config.vm.box = 'digital_ocean'
    config.vm.box_url = "https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box"
    config.ssh.private_key_path = '~/.ssh/id_rsa'
    config.vm.synced_folder ".", "/vagrant", type: "rsync"

    config.vm.define "dbserver-new", primary: true do |server|
        server.vm.provider :digital_ocean do |provider|
            provider.ssh_key_name = ENV["SSH_KEY_NAME"]
            provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
            provider.image = 'ubuntu-22-04-x64'
            provider.region = 'fra1'
            provider.size = 's-1vcpu-1gb'
            provider.privatenetworking = true
            /provider.ipv4_private_network = "192.168.50.5"/
        end

        server.vm.hostname = "dbserver-new"

        server.trigger.after :up do |trigger|
            trigger.info =  "Writing dbserver's IP to file..."
            trigger.ruby do |env,machine|
                remote_ip = machine.instance_variable_get(:@communicator).instance_variable_get(:@connection_ssh_info)[:host]
                File.write($ip_file, remote_ip)
            end
        end

        server.vm.provision "shell", inline: <<-SHELL
            echo "Installing prerequisites"
            sudo apt-get install gnupg curl
            sudo apt-get update

            echo "Installing dotnet"
            sudo apt-get install -y dotnet-sdk-7.0

            echo "Adding .NET to bash_profile"
            tee -a ~/.bash_profile << END
                # Add .NET Core SDK tools
                export PATH="$PATH:/root/.dotnet/tools"
END

            #echo "# Add .NET Core SDK tools" >> ~/.bash_profile
            #echo 'export PATH="$PATH:/root/.dotnet/tools"' >> ~/.bash_profile

            echo "Installing EF"
            dotnet tool install --global dotnet-ef --version 7.0.1

            echo "Installing Sqlite"
            sudo apt-get install -y libsqlite3-dev sqlitebrowser

            #might need to start the DB
        SHELL
    end

    config.vm.define "webserver-new", primary: false do |server|

        server.vm.provider :digital_ocean do |provider|
            provider.ssh_key_name = ENV["SSH_KEY_NAME"]
            provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
            provider.image = 'ubuntu-22-04-x64'
            provider.region = 'fra1'
            provider.size = 's-1vcpu-1gb'
            provider.privatenetworking = true
        end

        server.vm.hostname = "webserver-new"

        server.trigger.before :up do |trigger|
            trigger.info =  "Waiting to create server until dbserver's IP is available."
            trigger.ruby do |env,machine|
                ip_file = "db_ip.txt"
                while !File.file?($ip_file) do
                    sleep(1)
                end
                db_ip = File.read($ip_file).strip()
                puts "Now, I have it..."
                puts db_ip
            end
        end

        server.trigger.after :provision do |trigger|
            trigger.ruby do |env,machine|
            File.delete($ip_file) if File.exists? $ip_file
            end
        end

        server.vm.provision "shell", inline: <<-SHELL
            export DB_IP=`cat /vagrant/db_ip.txt`
            echo $DB_IP

            echo $DB_IP

            echo "Installing dotnet"
            sudo apt-get install -y dotnet-sdk-7.0

            echo "Adding .NET to bash_profile"
            tee -a ~/.bash_profile << END
                # Add .NET Core SDK tools
                export PATH="/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin:/snap/bin:/root/.dotnet/tools"
END

            #echo "# Add .NET Core SDK tools" >> ~/.bash_profile
            #echo 'export PATH="$PATH:/root/.dotnet/tools"' >> ~/.bash_profile

            echo "Installing EF"
            dotnet tool install --global dotnet-ef --version 7.0.1
    
    
            cp -r /vagrant/* $HOME
            echo "GOING INTO MINITWIT"
            cd Minitwit/
            pwd | echo
            echo "RUNNING THE PROGRAM"
            nohup dotnet run #> out.log &
            echo "================================================================="
            echo "=                            DONE                               ="
            echo "================================================================="
            echo "Navigate in your browser to:"
            THIS_IP=`hostname -I | cut -d" " -f1`
            echo "http://${THIS_IP}:5000"
        SHELL
    end

    config.vm.provision "shell", privileged: false, inline: <<-SHELL
        sudo apt-get update
    SHELL
end