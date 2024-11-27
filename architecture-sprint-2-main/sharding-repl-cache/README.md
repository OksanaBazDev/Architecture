## -----------------------------------------------------------------------------Задание 4. Кэширование

##Запускаем mongodb и приложение
```shell
docker compose up -d
```

```Подключитесь к серверу конфигурации и сделайте инициализацию:
docker exec -it configSrv mongosh --port 27021
 
rs.initiate({_id:"configSrv", configsvr:true, members:[{_id:0,host: "configSrv:27021"}]});
exit()
```

```Инициализируйте шард 1:
docker exec -it shard1 mongosh --port 27022

rs.initiate({_id: "shard1", members: [ {_id: 0, host: "shard1:27022"}, {_id: 1, host: "shard1replica1:27030"},{_id: 2, host: "shard1replica2:27031"}, {_id: 3, host: "shard1replica3:27032"}]})
exit()

```Инициализируйте шард 2:
docker exec -it shard2 mongosh --port 27023

rs.initiate({_id: "shard2", members: [ {_id: 0, host: "shard2:27023"}, {_id: 1, host: "shard2secondary1:27033"}, {_id: 2, host: "shard2secondary2:27034"}, {_id: 3, host: "shard2secondary3:27035"}]})
exit()
```

```Инцициализируйте роутер и наполните его тестовыми данными:
docker exec -it mongos_router mongosh --port 27024

sh.addShard( "shard1/shard1:27022");
sh.addShard( "shard2/shard2:27023");

sh.enableSharding("somedb");
sh.shardCollection("somedb.helloDoc", { "name" : "hashed" } )

use somedb

for(var i = 0; i < 1000; i++) db.helloDoc.insertOne({age:i, name:"ly"+i})

db.helloDoc.countDocuments() 
exit(); 
```

```Сделайте проверку на шардах:
docker exec -it shard2 mongosh --port 27023

use somedb;
db.helloDoc.countDocuments();
exit(); 

```
#выдаст 508 докуметов

```
docker exec -it shard1 mongosh --port 27022

use somedb;
db.helloDoc.countDocuments();
exit();
```
#выдаст 492 документа
 
##В итоге документы распределились по шардам!

затем проследуйте по ссылке http://localhost:8080/
результат выполнения можно проверить http://localhost:8080/docs 
collectionName - helloDoc



