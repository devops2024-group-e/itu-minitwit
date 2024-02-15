#!/bin/bash

if [ "$1" = "init" ]; then

    if [ -f "./test-tmp/minitwit.db" ]; then
        echo "Database already exists."
        exit 1
    fi
    echo "Putting a database to ./test-tmp/minitwit.db..."
    python3 -c "from minitwit import init_db;init_db()"
elif [ "$1" = "start" ]; then
    echo "Starting minitwit..."
    nohup "$(which python3)" minitwit.py > /tmp/out.log 2>&1 &
elif [ "$1" = "stop" ]; then
    echo "Stopping minitwit..."
    pkill -f minitwit
elif [ "$1" = "inspectdb" ]; then
    ./flag_tool -i | less
elif [ "$1" = "flag" ]; then
    ./flag_tool "$@"
else
  echo "I do not know this command..."
fi
