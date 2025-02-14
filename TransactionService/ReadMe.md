# Databases installation guide

1. Install Docker for desktop
2. Initialize docker swarm `docker swarm init` 
3. Deploy stack: `docker stack deploy dstack -c ./databases-stack.yml`
4. Uninstall stack with: `docker stack rm dstack`

