#!/bin/bash
# Description:
#   Activate build remotely. This script assumes that a host can reach build.c4.kabam.com.
#   This might be needed in the hosts file.
#   e.g. 
#       On build machine
#       127.0.0.1       build.c4.kabam.com
#
#       On other hosts
#       10.153.0.56     build.c4 build.c4.kabam.com
#
# CURL Example 
# curl -s -w "%{http_code}" "http://build.c4.kabam.com/job/Fusio%20OSX%20Development/build" -o /dev/null
#
# Crontab is set to run every hour from 10:00 - 18:00 on working day
# 00 10-18 * * 1-5 /Users/rsa/Development/Fusion/Tool/activate-build.sh -j "Fusion OSX Development":
#

CMD_CURL=`which curl`;
HOST_BUILD="build.c4.kabam.com";
CMD_STATUS=404;

if [ "$CMD_CURL" = "" ];
then
    echo "ERROR - no curl installed";
fi;

usage()
{
    cat << EOF
Usage: $0 options

Activate build remotely. This script assumes that a host can reach build.c4.kabam.com.
This might be needed in the hosts file.
e.g. 
    On build machine
    127.0.0.1       build.c4.kabam.com

    On other hosts
    10.153.0.56     build.c4 build.c4.kabam.com

OPTIONS
    -h  Show help message
    -j  Job name (e.g. "Fusion OSX Development", "Fusion iOS Development")

EOF
}

TASK="";
while getopts "hj:" OPTION 
do
    case $OPTION in 
        h)
            usage;
            exit 0;
            ;;
        j)
            TASK=$OPTARG;
            ;;
        ?)
            usage;
            exit 1;
            ;;
    esac
done;

if [ "$TASK" != "" ];
then
    # Replace space with %20
    TASK=${TASK// /%20};

    LINE="$CMD_CURL -s -w "%{http_code}" "http://$HOST_BUILD/job/$TASK/build" -o /dev/null";
    CMD_STATUS=`$LINE`;
fi;

if [ $CMD_STATUS -eq 404 ];
then
    echo "Build [$TASK] could not be started";
    exit 1;
fi;
