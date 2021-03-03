#!/bin/bash

###########################################################################
####		Constants
###########################################################################

BUILDPATH="/Users/jenkins/builds"
APPVERSION="/Users/jenkins/Home/jobs/Development-iOS-build/workspace/client_gam/Source/Assets/Resources/app_version.txt"
GN="godsandmonsters"
REV=$(date +%Y_%m_%d)

GITREV=$(cat $APPVERSION)
#GITREV=$(/usr/bin/git rev-parse --short=0 HEAD)
echo "Current Git revision: $GITREV"

IOSVERFILE=$BUILDPATH"/iosVersion.txt"
IOSVER=$(cat $BUILDPATH/iosVersion.txt)
URL="<![CDATA[https://10.80.0.1/BOMB/""iOS"$IOSVER"/godsandmonsters.ipa]]>"

TITLE="GAM_1.0.0_iOS""_""$REV""_""$GITREV"
#add 1 version for IOSVER
CHECK="50"
INIT=1

if [[ "$IOSVER" -ge "$CHECK" ]]
then
        echo "$INIT" > $IOSVERFILE
        echo "The number of builds alreday hit the limit"
        echo "Change back the build version to 1"

else
        IOSVERADD=$(( $IOSVER + 1 ))
        echo $IOSVERADD > $IOSVERFILE
        echo "Add 1 and current version is $IOSVERADD"
fi
#Record the start time for build
STARTTIME=$(date +"%s")
echo "Start time to build is: $(date)"

###########################################################################
####            Functions
###########################################################################

function backup_builds
{
	#backup builds to iOS
        mkdir -p "$BUILDPATH/""iOS"$IOSVER

        cp -a $BUILDPATH"/godsandmonsters.ipa" "$BUILDPATH/""iOS"$IOSVER
}

function create_plist
{
	export INFOPLIST=${BUILDPATH}"/iOS"${IOSVER}/${GN}.plist
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
	<string>$URL</string>
	</dict>
	</array>
	<key>metadata</key>
	<dict>
	<key>bundle-identifier</key>
	<string>com.kabam.godsandmonsters</string>
	<key>bundle-version</key>
	<string>$IOSVER</string>
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
}

function upload_ota
{
	#using lftp to upload iap and plist to smoke bomb server
	#Upload the bombbuddies.ipa and bombbuddies.plist to dir of ""
	SERVERDIR="iOS"$IOSVER
	echo "Server dir is ${SERVERDIR}"

	/opt/local/bin/lftp -e  "mkdir ${SERVERDIR}; cd ${SERVERDIR}; put ${BUILDPATH}/${SERVERDIR}/godsandmonsters.ipa; put ${BUILDPATH}/${SERVERDIR}/godsandmonsters.plist; exit " -u bw,kabambw 10.80.0.1

	echo "Done to upload the files to OTA"

}


###########################################################################
####           Program starts here 
###########################################################################
backup_builds

create_plist

upload_ota
