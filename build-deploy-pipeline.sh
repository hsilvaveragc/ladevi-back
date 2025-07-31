#!/bin/bash
#1.- me paro sobre el directorio DplusConnector/DplusConnector
#2.- valido si es develop o stagig_qa que son los unicos branchs que compilan y despliegan
#3.- genero el string de los build y deploy
#4.- corro el build y deploy

# Función que notifica el resultado de los deploys
function sendNotification(){
    # Se genera el mensaje que se visualizará en slack
    IMG_R=":large_green_circle: "
    IMG_W=":red_circle: "
    FECHA_HORA=$(date)
    OUTPUT_LINK="${BITBUCKET_GIT_HTTP_ORIGIN}/addon/pipelines/home#!/results/${BITBUCKET_BUILD_NUMBER}"
    JSON_REPLACE=$(cat ../../message_slack_deploy.json)
    MAIL_COMMITER=$(git --no-pager show -s --format=%ae)
    LAST_MESSAGE=$(git show -s --format=%s)
    if [ "$1" == "0" ]; then
        JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s@#IMG_RESULT#@${IMG_R}@")
    else
        JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s@#IMG_RESULT#@${IMG_W}@g")
    fi
    JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s/#MSG_IMAGE#/$2/")
    JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s/#FECHA#/${FECHA_HORA}/")
    JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s/#COMMITER#/${MAIL_COMMITER}/")
    JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s@#URL_BRANCH#@${BITBUCKET_GIT_HTTP_ORIGIN}/src/${BITBUCKET_BRANCH}/@g")
    JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s/#BRANCH#/${BITBUCKET_BRANCH}/")
    JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s/#COMMIT#/${BITBUCKET_COMMIT}/")
    JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s/#MENSAJE#/${LAST_MESSAGE:0:30}/")
    JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s@#LOG#@${OUTPUT_LINK}@g")
    JSON_REPLACE=$(echo "${JSON_REPLACE}" | sed "s@#TEST#@Total: ${TOTAL_TEST} Passed: ${TOTAL_PASSED}@g")
    echo "${JSON_REPLACE}" > payload.json
    # Se envia el sms a slack
    curl -X POST -H "Content-type:application/json" --data @payload.json https://hooks.slack.com/services/TRHCHUAF3/B04FE4H6JPL/${SLACK_WEBHOOK_URL}
}
remote_run() {
    echo "Ejecutando: ${2} en ${1}"
    echo "${1} ${2}"
    ssh -o "StrictHostKeyChecking=no" "${CICD_USER}@${1}" "${2}"
}

copy() {
    echo "Copiando: ${1} en ${2}:${3}"
    scp -o "StrictHostKeyChecking=no" "${1}" "${CICD_USER}@${2}:${3}"
    echo "Copiado: ${1} en ${2}:${3}"
}
clone(){
    FILE="${1}/axum-main-extensions/${2}"
    echo "#!/bin/sh" > tmpScript.sh
    echo "if [ -f \"$FILE\" ]; then" >> tmpScript.sh
    echo 'echo "OK"' >> tmpScript.sh
    echo "rm -Rf ${1}/axum-main-extensions" >> tmpScript.sh
    echo 'else' >> tmpScript.sh
    echo 'echo "ERROR"'  >> tmpScript.sh
    echo "fi" >> tmpScript.sh
    chmod +x tmpScript.sh
    copy "tmpScript.sh" "${5}" "${PATH_GIT_LOCAL}/tmpScript.sh"
    RESULTADO=$(remote_run "${5}" "${1}/tmpScript.sh")
    echo "El resultado es: ...$RESULTADO"
    remote_run "${5}" "cd ${1} && git clone ${3} && cd axum-main-extensions && git switch ${4}"
    #remote_run "${5}" "cd ${1} && git clone ${3} && cd axum-main-extensions && git switch ${BITBUCKET_BRANCH}"
}

# Función que ejecuta el string generado en la funcion make_string_build
function build_deploy_remote(){
    IFS=', ' read -r -a array <<< "$CONTAINERS"
    #clone "${PATH_GIT_LOCAL}" ".gitignore" "https://${USER_GIT}:${PASS_GIT}@bitbucket.org/greencodesoftware/axum-main-extensions.git" "${1}" "$2"
    #copy ".env" "${2}" "${PATH_GIT_LOCAL}/axum-main-extensions/DplusConnector/DplusConnector/.env"
    contador=0
    for i in "${array[@]}"
    do
        if [ $contador -eq 0 ] ; then
            echo "Build 1: ${array[-1]}"
            echo "Up 1: $i"
            remote_run "${2}" "echo 'BITBUCKET_BRANCH=${BITBUCKET_BRANCH}' > .env && export AWS_DEFAULT_REGION=$AWS_DEFAULT_REGION && export AWS_SECRET_ACCESS_KEY=$AWS_SECRET_ACCESS_KEY && export AWS_ACCESS_KEY_ID=$AWS_ACCESS_KEY_ID && aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 849369711646.dkr.ecr.us-east-1.amazonaws.com && docker pull 849369711646.dkr.ecr.us-east-1.amazonaws.com/ladevi:ladevi-ventas-api-$BITBUCKET_BRANCH && docker-compose -f docker-compose.pipeline.yml up -d --force-recreate $i"
            echo "Up 2: $i"
        else
            remote_run "${2}" "docker-compose -f docker-compose.pipeline.yml up -d --force-recreate $i"
        fi
        contador=$((contador+1))
    done

}

# Función que controla el deploy correcto del/los contenedores
function health_check(){
    docker context create "$1" --docker "host=ssh://$CICD_USER@$2"
    docker context use "$1"    
    IFS=', ' read -r -a contenedores <<< "$CONTAINERS"
    errores=0
    OK='"healthy"'
    for e in "${contenedores[@]}"
    do
        if [[ "$e" != *"migrate"* ]];then
            RESULTADO_HEALTH=$(docker --context $1 inspect "$e" | jq '.[].State.Health.Status')
            if [ "$RESULTADO_HEALTH"  != "$OK" ] ; then
                errores=$((errores+1))
            fi
        fi
    done
    if [ $errores -ne 0 ] ; then
        echo "El deploy no se ejecuto correctamente... (#$errores)"
        # pasar ultimo container
        sendNotification "1" "${contenedores[-1]}"
        exit 1
    else
        # pasar ultimo container
        sendNotification "0" "${contenedores[-1]}"
        echo "Envio notificacion y borro las imagenes de los tagas anteriores"
    fi

}


# Inicio el Script y voy al directorio donde esta el docker-compose
# Valido el branch
case "$BITBUCKET_BRANCH" in
    staging_qa)
        echo "Build & Deploy en Staging_QA"
        copy "docker-compose.pipeline.yml" "$REMOTE_HOST" "/home/ubuntu/docker-compose.pipeline.yml"
        build_deploy_remote "staging_qa" "$REMOTE_HOST"
       # sleep $TIME_WAIT
       # health_check "staging_qa"  "$HOST_QA"    
        ;;
        
    *)
        echo "No se hace deploy"
        ;;
esac
