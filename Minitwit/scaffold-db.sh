#!/bin/bash

dotnet ef dbcontext scaffold "Host=127.0.0.1;Port=5432;Username=minitwit-sa;Password=123;Database=minitwit" Npgsql.EntityFrameworkCore.PostgreSQL -o Models -c MinitwitContext --context-dir ./ -f
