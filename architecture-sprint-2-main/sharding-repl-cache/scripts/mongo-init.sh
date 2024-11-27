#!/bin/bash

###
# Инициализируем бд
###

docker compose exec -T configSrv mongosh --port 27021 --quiet <<EOF
rs.initiate({_id:"config_server", configsvr:true, members:[{_id:0,host: "configSrv:27021"}]});
EOF

docker compose exec -T shard1 mongosh --port 27022 --quiet <<EOF
rs.initiate({_id:"shard1", members:[{_id:0, host:"shard1:27022"}]});
EOF

docker compose exec -T shard2 mongosh --port 27023 --quiet <<EOF
rs.initiate({_id:"shard2", members:[{_id:1, host:"shard2:27023"}]});
EOF

docker compose exec -T mongos_router mongosh --port 27024 <<EOF
sh.addShard( "shard1/shard1:27022");
sh.addShard( "shard2/shard2:27023");
sh.enableSharding("somedb");
sh.shardCollection("somedb.helloDoc", { "name" : "hashed" } )
EOF

docker compose exec -T mongos_router mongosh --port 27024 --quiet <<EOF
use somedb;
for(var i = 0; i < 1000; i++) db.helloDoc.insert({age:i, name:"ly"+i})
db.helloDoc.countDocuments();
EOF

docker compose exec -T shard1 mongosh --port 27022 --quiet <<EOF
use somedb;
db.helloDoc.countDocuments();
EOF

docker compose exec -T shard2 mongosh --port 27023 --quiet <<EOF
use somedb;
db.helloDoc.countDocuments();
EOF