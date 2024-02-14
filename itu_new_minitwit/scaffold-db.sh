#!/bin/bash

dotnet ef dbcontext scaffold "Data Source=../tmp/minitwit.db" Microsoft.EntityFrameworkCore.Sqlite -o Models -c MinitwitContext
