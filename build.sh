#!/bin/bash
# https://andrewlock.net/optimising-asp-net-core-apps-in-docker-avoiding-manually-copying-csproj-files/

set -eux

# tarball csproj files, sln files, and NuGet.config
find . \( -name "*.csproj" -o -name "*.sln" -o -name "nuget.config" \) -print0 | tar -cvf app.tar --null -T -

# TODO: depois mudar para usar a url do ECR
docker build . -f  ./Dockerfile -t stark-challenge:$1

rm app.tar