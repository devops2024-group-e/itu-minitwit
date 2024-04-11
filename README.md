# Minitwit
This is a ITU twitter example website(only for educational purposes).

# How to start contributing?
In order to start contributing to the project you need to have your development environment right.

## Prerequisites
- Docker (v24.0.7)
- Visual Studio Code
  - Dev Container (Latest version of the Visual Studio Code Extension)

### Develop with Dev Container
In order to get started with development you need to have the prerequisites installed. If that is installed you can go ahead and clone the git repo.
Then follow these steps:

1. Open the project in Visual Studio Code
2. When it is open(and you have the dev-container extension installed and enabled) a prompt should appear in the bottom right corner asking if you want to reopen the directory with the dev-container. You need to say yes to that.
If it does not appear, you can in the Visual Studio Command pallet type in `Dev Containers: Reopen in Container`
3. Running this might take a while (be patient). When it is ready you can start developing the application like if you were on linux.

> [!IMPORTANT]
> You might get issues in using Git if you have not installed `git-credentials-manager`. Without it, the dev-container cannot fetch your git credentials. Also it is important to provide your git username and email in the git config. Write `git config --global user.name <Your name>` and `git config --global user.email <Your email for your github user>` in the terminal for the dev-container.

### Develop without development container
> [!IMPORTANT]
> You need to have the dotnet cli installed with the 7.0 version of the sdk. Using `dotnet` in this project without having the 7.0 version installed should then guide you to how get the relevant SDK versions you need.

You can run your development environment without using development containers then you still need to have docker installed.
What we need to do is to start the database. We do so by typing in the following at the root of the project:
```bash
docker compose up minitwit-db
```

Now you can run whatever `dotnet` command in order to run the projects or tests. As an example you can navigate to the frontend project and run it:
```bash
cd Minitwit
dotnet run
```

Then application should start and display where you can access the webpage in the terminal.

### How to make pre-commit work
> [!NOTE]
> This guide assumes that you are using the devcontainer.

After opening the devcontainer, run the following commands to install pre-commit, dotnet-format and hadolint:
```bash
install pre-commit #In order to have hooks that run automatically when we commit

pre-commit
```
The last command takes a bit of time to run, as it traverses through the files and applyes the formatters and linters.
