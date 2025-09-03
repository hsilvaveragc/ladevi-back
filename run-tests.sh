#!/bin/bash
docker network create custom_network
docker volume create dbdata
docker-compose up --force-recreate -d ladevi_ventas_api_pg_db_testing
docker-compose build ladevi_ventas_api_xunit_tests
docker-compose run ladevi_ventas_api_xunit_tests
if [ $? -ne 0 ] ; then echo "error al correr tests" ; exit 1 ; fi
docker-compose down
docker-compose build ladevi_ventas_api ladevi_ventas_api_pg_migrator
docker tag ladevi_ventas_api asmx1986/greencode:ladevi_ventas_api_latest
docker push asmx1986/greencode:ladevi_ventas_api_latest
docker tag ladevi_ventas_api_pg_migrator asmx1986/greencode:ladevi_ventas_api_pg_migrator_latest
docker push asmx1986/greencode:ladevi_ventas_api_pg_migrator_latest
docker image prune -f
docker volume rm dbdata
