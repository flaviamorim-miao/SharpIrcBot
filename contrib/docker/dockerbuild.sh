#!/bin/sh
cd "$(dirname "$0")/../.."
dotnet restore
cd SharpIrcBotCLI
dotnet publish -f "netcoreapp1.0" -r "debian.8-x64" -o "../out"
cd ..
docker build -t sharpircbot .
