# Log

## Week 1 - 02/02/24

**Task**

Our task was to make the `minitwit.py` run with python v3 instead of python v2.

**Steps taken**

1. We copied the source code from the server

    ```bash
    $ scp -r student@164.90.160.52:/home/student/itu-minitwit ~/Desktop/
    ```
2. We also copied the database which was saved as a `*.db` file with
    ```bash
    $ scp student@164.90.160.52:/tmp/minitwit.db ~/Desktop/itu-minitwit
    ```
3. Then we created a Github organization with a repository called itu-minitwit

4. We clone the minitwit repo

   ```bash
   $ git clone https://github.com/devops2024-group-e/itu-minitwit.git
   ```

5. We copied the files from `~/Desktop` to the location where we cloned the repo

6. We tried to run the application by using the `control.sh` script. But got an error message that it were missing the database. We saw that the database file actually should be located in the `/tmp` and not in the current working directory of the application. So we placed the database in `/tmp` and then it worked.

7. We then started doing the steps to convert the application from python 2 to python 3. First we used the `2to3` tool to convert to see what has to be changed.
   1. We changed the reading of the database file to read it as `UTF-8`
   2. Added parenthesis to the print statement
   3. In the tests we changed the assertions to use `rv.text` instead of `rv.data` as the `rv.data` is in a binary format and that we are trying to look for a particular string.

8. After that we ran the `shellcheck` on `control.sh`. Here we added the shebang (`#!/bin/bash`) to the top of the file.

9. After all of this we tried to look into the difference between `python` and `python3` command [(take a look here)](https://stackoverflow.com/questions/64801225/python-or-python3-what-is-the-difference). After looking into it we decided to just use the `python3` command in order to make sure that the tools we use actually runs python3.
