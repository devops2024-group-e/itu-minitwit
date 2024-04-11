#!/bin/bash

ssh devops "/bin/bash -c 'ufw allow in on eth0 to any port 5432'"

# First we need to run pg_dump for the old database server and save the dump file
echo "Fetch Database dump from source database"
pg_dump -h <webserver host ip> -U minitwit-sa -p 5432 -Fc minitwit > ./minitwit-dump.pgsql

# Secondly we need to run pg_restore on the new database with the dump file that we have just saved
echo "Restoring database on target database"
pg_restore -d <DO CONNECTION URI string to doadmin> --no-owner ./minitwit-dump.pgsql

psql -h <DO hostname> -p 25060 -U doadmin -d minitwit -c 'GRANT SELECT, UPDATE, INSERT, DELETE ON ALL TABLES IN SCHEMA public TO "minitwit-sa";'
psql -h <DO hostname> -p 25060 -U doadmin -d minitwit -c 'GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO "minitwit-sa";'

ssh devops "/bin/bash -c 'ufw deny in on eth0 to any port 5432'"
