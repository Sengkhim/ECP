# Rebuild the image
docker build --no-cache -t user-service .

# Run the container
docker run --name user-service --env-file .env -p 1270:1270 user-service
