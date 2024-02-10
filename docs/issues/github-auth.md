# How to make Github collaborate with git
We had an issue in the group where we got a 403 error when we wanted to push to Github. So far we have experienced it for the following operating systems:

- M1 Mac with MacOS
- Windows running WSL

# What is the issue?

The issue is that Github has changed the way they want the users to handle their credentials and the way they want us to authenticate. If we use the `HTTPS` url for pull, push, fetch, and etc. we will now have to generate a token as a new form of password or let the newly created `git-credentials-manager` handle the authentication for you.

## Solution for MacOS

I (Andreas) solved it by installing the `git-credential-manager` through the Homebrew package manager. So if you do not have Homebrew, then please install it from [here](https://brew.sh/).

1. Then in order to install the git credential manager you need to type in the following from your terminal:

    ```bash
    brew install --cask git-credential-manager
    ```

2. Now you need to navigate to the repo that you want to push to and type in:

    ```bash
    git push -u origin <name of the branch you want to push>
    ```

3. Then you should see the git credential manager kick in. It will prompt you for different options on how to login. Choose the “Sign in with your browser”
4. Now after you have authenticated, you just have to wait a little bit and then it should be it

## Solution for WSL

1. In windows, install the newest version of `git for windows` from [here](https://gitforwindows.org/). This will install everything that we need.
2. Now we go back to your WSL terminal and set the global git config like:

    ```bash
    git config --global credential.helper "/mnt/c/Program\ Files/Git/mingw64/bin/git-credential-manager.exe"
    ```

3. In WSL  you need to navigate to the repo that you want to push to and type in:

    ```bash
    git push -u origin <name of the branch you want to push>
    ```

4. Then you should see the git credential manager kick in. It will prompt you for different options on how to login. Choose the “Sign in with your browser”
5. Now after you have authenticated, you just have to wait a little bit and then it should have pushed your changes
