set -ex

# run script from root folder (i.e., `./scripts/create_k8s_secret.sh`)
. ./.env

kubectl create secret generic -n api amplitude \
    --from-literal=api-key=$AMPLITUDE_API_KEY