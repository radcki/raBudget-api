[![buddy pipeline](https://app.buddy.works/radekadamczewski/rabudget-api/pipelines/pipeline/207338/badge.svg?token=e526fabfec06767eae9d1e35382fa7f4a8db20b4cb73226ce7a174a6b0823957 "buddy pipeline")](https://app.buddy.works/radekadamczewski/rabudget-api/pipelines/pipeline/207338)
# raBudget - Api

WepApi application for budgeting app. Live version available at https://budget.rabt.pl/api

EntityFramework supported database is required for application, by default MySql is expected to be available at localhost, with option to easily switch connection string or sever type to Sql Server via appsettings.json. 

## Build instructions - docker
Run latest version from registry:
``` console
$ docker pull radekadamczewski/rabudget-api:latest
$ docker run -d --network host --name rabudget-api radekadamczewski/rabudget-api
```
Application is expecting MySql server to be available at localhost and listens on 4002 port by default. If you do not want to use 'host' network, you might want to edit connection string in appsettings.json, and build container manuall:

``` console
$ docker build -t rabudget-api .
$ docker run -d --network host --name rabudget-api rabudget-api
```

## Build instructions - without docker

``` console
$ dotnet restore raBudget-Api.sln
$ dotnet publish raBudget-Api.sln -c Release -o out
$ dotnet out/WebApi.dll

Hosting environment: Production
Now listening on: http://localhost:4002
Application started. Press Ctrl+C to shut down.
```
## Hosting - nginx configuration
Api is expecting to be accessed from base path \<host>/api . Example of nginx location definition:

``` nginx
location /api {
    rewrite ^/api(/.*)$ $1 break;
    proxy_pass http://localhost:4002;
    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection keep-alive;
    proxy_set_header Host $host;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header X-Forwarded-Proto $scheme;
    proxy_cache_bypass $http_upgrade;
    proxy_redirect   off;
}
```

## Swagger
Application is exposing api definition using Swagger:

UI: 
>https://\<host>/api/swagger/

JSON: 
>https://\<host>/api/swagger/v1/swagger.json