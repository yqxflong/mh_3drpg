#!/bin/bash

###########################################################################
####		Constants
###########################################################################

BUILDPATH="/Users/jenkins/builds"
GN="godsandmonsters"
REV=$(date +%Y_%m_%d)
GITREV=$(/usr/bin/git rev-parse --short=0 HEAD)
echo "Current Git revision: $GITREV"
TITLE="GAM_1.0.0_Android_""$REV""_""$GITREV"
#Record the start time for build
STARTTIME=$(date +"%s")
echo "Start time to build is: $(date)"

###########################################################################
####            Functions
###########################################################################

function create_plist
{
	export INFOPLIST=${BUILDPATH}/${GN}.json
	infoPlistContent="{\"title\":\"$TITLE\",\"versionCode\":1,\"versionName\":\"1.0.0\"}"

	echo ${infoPlistContent}>${INFOPLIST}
}

function upload_ota
{
	#using lftp to upload iap and plist to smoke bomb server
	SERVERDIR="Android"
	echo "Server dir is ${SERVERDIR}"
	/opt/local/bin/lftp -e "cd ${SERVERDIR}; put ${BUILDPATH}/godsandmonsters.apk; put ${BUILDPATH}/godsandmonsters.json; exit " -u bw,kabambw 10.80.0.1

	echo "Done to upload the files to OTA"

}


###########################################################################
####           Program starts here 
###########################################################################

create_plist

upload_ota
