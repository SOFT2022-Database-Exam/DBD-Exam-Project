docker compose -f infrastructure/docker-compose.applications.dev.yml down --remove-orphans
docker compose -f infrastructure/docker-compose.infrastructure.dev.yml down --remove-orphans
docker compose -f infrastructure/docker-compose.tools.yml down --remove-orphans

docker compose -f infrastructure/docker-compose.infrastructure.dev.yml up --build -d

docker compose -f infrastructure/docker-compose.infrastructure.dev.yml exec mongo-conf1 sleep 20
docker compose -f infrastructure/docker-compose.infrastructure.dev.yml exec mongo-conf1 mongosh --quiet --file /scripts/setup_conf.js

docker compose -f infrastructure/docker-compose.infrastructure.dev.yml exec mongo-shard1-rs1 mongosh --quiet --file /scripts/setup_shard1.js
docker compose -f infrastructure/docker-compose.infrastructure.dev.yml exec mongo-shard2-rs1 mongosh --quiet --file /scripts/setup_shard2.js
docker compose -f infrastructure/docker-compose.infrastructure.dev.yml exec mongo-conf1 sleep 10
docker compose -f infrastructure/docker-compose.infrastructure.dev.yml exec mongo-router mongosh --quiet --file /scripts/setup_mongorouter.js

docker compose -f infrastructure/docker-compose.applications.dev.yml up --build -d
docker compose -f infrastructure/docker-compose.tools.yml up -d