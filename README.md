# File Translation Service

Stores information about File translation tasks and documents.
If user wants to translate file, task is submitted to file translation workers via **RabbitMQ**

```
 -----------------------------------             --------------------------------
|                                   |           |                                |
|                                   |   → → →   |           Database             |
|                                   |   ← ← ←   |                                |
|                                   |            --------------------------------
|      File translation service     |
|             [Public]              |            --------------------------------
|                                   |           |                                |
|                                   |   → → →   |   File storage, linked to DB   |
|                                   |   ← ← ←   |                                |
 -----------------------------------             --------------------------------

            ↓           ↓
            ↓           ↓
            ↓           ↓
            ↓           ↓
            ↓           ↓   Fetch available translation systems for request validation
            ↓           ↓                        --------------------------------
            ↓           ↓                       |                                |
            ↓           ↓ → → → → → → → → → →   |   Translation system service   |
            ↓                                   |                                |
            ↓                                    --------------------------------
            ↓
            ↓
            ↓
            ↓
            ↓
            ↓   Submits translation task to be processed
            ↓   by file translation workers (via RabbitMQ)
            ↓

 ------------------------------------
|                                   |
|      File translation worker      |
|                                   |
 ------------------------------------

```

# Development tools

## Entity Framework tools

```Bash
dotnet tool install --global dotnet-ef
```

# Build

Using docker
```Shell
docker build -f ./FileTranslationService/Dockerfile -t file-translation-service .
```

# Test

Install prerequisites

```Shell
# install kubectl
choco install kubernetes-cli
# install helm
choco install kubernetes-helm
```

Install RabbitMQ, MySQL

```Shell
helm repo add bitnami https://charts.bitnami.com/bitnami
helm repo update

# RabbitMQ
helm install rabbitmq --set auth.username=root,auth.password=root,auth.erlangCookie=secretcookie bitnami/rabbitmq

# MySQL
helm install mysql --set auth.rootPassword=root bitnami/mysql
```

forward ports:

```Shell
# RabbitMQ
kubectl port-forward --namespace default svc/rabbitmq 15672:15672 5672:5672

# MySQL
kubectl port-forward --namespace default svc/mysql 3306:3306
```

Using docker compose
```
docker-compose up --build
```

Open Swagger
```
http://localhost:5001/swagger/index.html
```
