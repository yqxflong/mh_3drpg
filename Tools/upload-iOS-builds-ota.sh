#!/bin/bash

###########################################################################
####		Constants
###########################################################################

BUILDPATH="/Users/jenkins/builds"
GN="godsandmonsters"
REV=$(date +%Y_%m_%d)
GITREV=$(/usr/bin/git rev-parse --short=0 HEAD)
echo "Current Git revision: $GITREV"
TITLE="GAM_1.0.0_iOS_""$REV""_""$GITREV"

#Record the start time for build
STARTTIME=$(date +"%s")
echo "Start time to build is: $(date)"

###########################################################################
####            Functions
###########################################################################

function create_plist
{
	export INFOPLIST=${BUILDPATH}/${GN}.plist
	infoPlistContent="<?xml version=\"1.0\" encoding=\"UTF-8\"?>
	<!DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" \"http://www.apple.com/DTDs/PropertyList-1.0.dtd\">
	<plist version=\"1.0\">
	<dict>
	<key>items</key>
	<array>
	<dict>
	<key>assets</key>
	<array>
	<dict>
	<key>kind</key>
	<string>software-package</string>
	<key>url</key>
	<string>__URL__</string>
	</dict>
	</array>
	<key>metadata</key>
	<dict>
	<key>bundle-identifier</key>
	<string>com.kabam.godsandmonsters</string>
	<key>bundle-version</key>
	<string>1.0.0</string>
	<key>kind</key>
	<string>software</string>
	<key>title</key>
	<string>$TITLE</string>
	</dict>
	</dict>
	</array>
	</dict>
	</plist>"

	echo ${infoPlistContent}>${INFOPLIST}
	#remove the zip file which not used
	`cd ${ARCHIVEPATH} && rm *.zip`
}

function upload_ota
{
	#using lftp to upload iap and plist to smoke bomb server
	#Upload the bombbuddies.ipa and bombbuddies.plist to dir of ""
	SERVERDIR="iOS"
	echo "Server dir is ${SERVERDIR}"
	/opt/local/bin/lftp -e "cd ${SERVERDIR}; put ${BUILDPATH}/godsandmonsters.ipa; put ${BUILDPATH}/godsandmonsters.plist; exit " -u bw,kabambw 10.80.0.1

	echo "Done to upload the files to OTA"

}


###########################################################################
####           Program starts here 
###########################################################################

create_plist

upload_ota
