#!/bin/bash

IMAGE_NAME="thanh1692003/document-service-highschool"
TAG="latest"

docker build -t $IMAGE_NAME:$TAG --build-arg BUILD_CONFIGURATION=Release .
docker push $IMAGE_NAME:$TAG