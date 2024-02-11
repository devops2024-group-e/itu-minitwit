# Minitwit
This is a ITU twitter example website(only for educational purposes).

# How to start contributing?
In order to start contributing to the project you need to have your development environment right.

## Prerequisites
In order to get started it is important that you have either a linux/unix machine with python(v3.*) or if not then you have the following:
- Docker (v24.0.7)
- Visual Studio Code
  - Dev Container (Latest version of the Visual Studio Code Extension)

> [!TIP]
> It is also possible to run the dev-container on linux/unix machines

### Develop in MacOS or Windows
> [!TIP]
> If you develop in Windows Operating System you also have the option to use WSL which more or less works as if you have installed a linux/unix OS on bare metal

In order to develop in MacOS or Windows you need to have the prerequisites installed (Note in the end you do not really develop in MacOS or Windows but in a linux environment created by a docker container). If that is installed you can go ahead and clone the git repo.
Then follow these steps:

1. Open the project in Visual Studio Code
2. When it is open(and you have the dev-container extension installed and enabled) a prompt should appear in the bottom right corner asking if you want to reopen the directory with the dev-container. You need to say yes to that.
If it does not appear, you can in the Visual Studio Command pallet type in `Dev Containers: Reopen in Container`
3. Running this might take a while (be patient). When it is ready you can start developing the application like if you were on linux.

> [!IMPORTANT]
> You might get issues in using Git if you have not installed `git-credentials-manager`. Without it, the dev-container cannot fetch your git credentials. Also it is important to provide your git username and email in the git config. Write `git config --global user.name <Your name>` and `git config --global user.email <Your email for your github user>` in the terminal for the dev-container.

> [!NOTE]
> There might be a possibility that some of the python related commands did not execute. If that is the case then please run the following:
>```
>sudo python3 -m venv .venv
>source .venv/bin/activate
>python3 -m pip install flask
>```

### Develop in Linux
This should more or less work out of the box... as far as we know at the moment.

## Setting up the environment
When the above prerequisites are done, then you you need to the following:
```bash
sudo apt install libsqlite3-dev
sudo apt install sqlitebrowser

make build
```
> [!NOTE]
> The above commands has already been executed for you if you run with the dev-container

After this you need to create the development database. We can do this in two ways:

**With Make**

```bash
make init
```

**With the `control.sh`**
```bash
sh ./control.sh init
```

Now you can run the application by typing the following:
```bash
sh ./control.sh start
```

The website should be accessible from `http://localhost:8001`
