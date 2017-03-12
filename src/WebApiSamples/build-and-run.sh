#!/usr/bin/env sh
set -e
dotnet restore
dotnet publish -r debian.8-x64 -c Release -o out
docker build -t api-demo .
docker run -p 8000:80 -d api-demo