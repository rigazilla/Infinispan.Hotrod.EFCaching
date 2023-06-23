#!/bin/bash +x

INFINISPAN_VERSION=${INFINISPAN_VERSION:-15.0.0.Dev01}

BUILD_DIR=build
ISPN_FILENAME=infinispan-server-${INFINISPAN_VERSION}
wget --progress=dot:giga -N http://downloads.jboss.org/infinispan/${INFINISPAN_VERSION}/${ISPN_FILENAME}.zip
rm -rf ${ISPN_FILENAME}
unzip -q ${ISPN_FILENAME}.zip
cp infinispan.xml ${ISPN_FILENAME}/server/conf
nohup $ISPN_FILENAME/bin/server.sh > console.log 2>&1 &
echo -n Waiting for Infinispan 
until curl -s http://localhost:11222/rest/v2/caches/default?action=entries -o /dev/null
do
	echo -n .
	sleep 1
done
echo
ISPN_PID=$!
dotnet run
kill $ISPN_PID
