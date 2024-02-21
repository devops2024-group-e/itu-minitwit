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
    end

    config.vm.define "webserver", primary: false do |server|

        server.vm.provider :digital_ocean do |provider|
            provider.ssh_key_name = ENV["SSH_KEY_NAME"]
            provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
            provider.image = 'ubuntu-22-04-x64'
            provider.region = 'fra1'
            provider.size = 's-1vcpu-1gb'
            provider.privatenetworking = true
        end

        server.vm.hostname = "webserver"

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
    end
end