#! /bin/sh
# run script from root folder (i.e., `./scripts/send_test_event.sh`)
set -ex

. ./.env

curl -X POST \
    -H "content-type: application/json"  \
    -H "ce-specversion: 1.0"  \
    -H "ce-source: curl-command"  \
    -H "ce-type: test.amplitude2"  \
    -H "ce-id: b569a3be-ffc1-420f-9c5d-793d9ef10747"  \
    -d '{"resourceId":"edddafd3-caa4-4563-a274-22d0a0b3d8d2", "userId": "bad4e470-a594-4afa-b0ad-1407014bc6b8", "deviceId": "1090163e-02e5-45ab-a3c1-e640989cc305"}' \
    -v \
    http://localhost:5000