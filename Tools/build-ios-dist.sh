#!/bin/bash

set -ue

UNITY_PATH="/Applications/Unity/Unity.app/Contents/MacOS:/cygdrive/c/Program Files/Unity/Editor"
PATH="$PATH:$UNITY_PATH"
UNITY="Unity"

TOOLS_PATH="$(dirname $0)"
TOOLS_PATH="$(cur=$(pwd); cd $TOOLS_PATH; pwd -P; cd $cur)"
BUILD_PATH="$(dirname $TOOLS_PATH)/Build"
PROJECT_PATH="$(dirname $TOOLS_PATH)/Source"
LOG_FILE="$BUILD_PATH/Logs/build.log"
PROJECT_NAME="mhj"
TARGET_PATH="${BUILD_PATH}/Client/Client-iOS/$PROJECT_NAME"
TARGET_NAME="Unity-iPhone"
CONFIGURATION="Release"
EXPORT_OPTIONS_PLIST="$TOOLS_PATH/exportOptionsDist.plist"
TIME=$(date +%Y%m%d%H%M%S)

if [ -n "$(uname -s | grep CYGWIN)" ]; then
	PROJECT_PATH="$(echo $PROJECT_PATH | sed 's#/cygdrive/\([a-z]\)/#\1:\\#')"
	mkdir -p Logs
	LOG_FILE="Logs/build.log"
fi

echo "TOOLS_PATH=$TOOLS_PATH"
echo "BUILD_PATH=$BUILD_PATH" 
echo "PROJECT_PATH=$PROJECT_PATH"
echo "TARGET_PATH=$TARGET_PATH"
echo "EXPORT_OPTIONS_PLIST=$EXPORT_OPTIONS_PLIST"
echo "LOG_FILE=$LOG_FILE"

echo "Clean target path ..."
rm -rf "$TARGET_PATH"

echo "Start build ..."

echo "Start Automation.UseiOSDeviceTarget ..."
"$UNITY" -projectPath "$PROJECT_PATH" -executeMethod Automation.UseiOSDeviceTarget -batchmode -quit -logFile "$LOG_FILE"
[ $? -ne 0 ] && echo "Automation.UseiOSDeviceTarget failed" && exit
echo "Automation.UseiOSDeviceTarget done"

echo "Start Automation.BuildiOSDeviceTempRelease ..."
"$UNITY" -projectPath "$PROJECT_PATH" -executeMethod Automation.BuildiOSDeviceRelease -batchmode -quit -logFile "$LOG_FILE"
[ $? -ne 0 ] && echo "Automation.BuildiOSDeviceTempRelease failed" && exit
[ ! -d "$TARGET_PATH" ] && echo "$TARGET_PATH not exists" && exit
echo "Automation.BuildiOSDeviceTempRelease done"

XCODEBUILD=`which xcodebuild`
[ -z "$XCODEBUILD" ] && echo "xcodebuild not found" && exit

#PROVISIONS_PATH="$(dirname $TOOLS_PATH)/Provisions"
#PROVISION_NAME="XC mhj development"
#SIGNER="iPhone Developer: tang baobiao (6PF59CK3XQ)"
#CERTIFICATE_NAME="$(echo "$SIGNER" | sed 's/://g')"
#PROFILE_FILE="${PROVISIONS_PATH}/${PROJECT_NAME}/${PROVISION_NAME}.mobileprovision"
#create keychain
#security create-keychain -p ${PROJECT_NAME} ${PROJECT_NAME}.keychain
#unlock keychain to avoid password promt
#security unlock-keychain -p ${PROJECT_NAME} ${PROJECT_NAME}.keychain
#import certificates
#security import "${PROVISIONS_PATH}/${PROJECT_NAME}/${CERTIFICATE_NAME}.p12" -k ${PROJECT_NAME}.keychain -T /usr/bin/codesign
#delete keychain
#security delete-keychain ${PROJECT_NAME}.keychain
#import provision
#UUID=`/usr/libexec/PlistBuddy -c 'Print :UUID' /dev/stdin <<< $(security cms -D -i "${PROFILE_FILE}.mobileprovision")`
#cp "${PROFILE_FILE}.mobileprovision" "$HOME/Library/MobileDevice/Provisioning Profiles/$UUID.mobileprovision"
#CODE_SIGN="CODE_SIGN_IDENTITY=\"$SIGNER\" PROVISIONING_PROFILE=\"$UUID\""

#code sign information in Automation.cs currently
CODE_SIGN=""

echo "Start xcodebuild ..."
"$XCODEBUILD" -project "$TARGET_PATH/$TARGET_NAME.xcodeproj" -target "$TARGET_NAME" -configuration "$CONFIGURATION" -sdk iphoneos clean build DEPLOYMENT_POSTPROCESSING=YES $CODE_SIGN
[ $? -ne 0 ] && echo "xcodebuild failed" && exit
echo "xcodebuild done"

echo "Start archive ..."
ARCHIVE_PATH="$TARGET_PATH/build/$PROJECT_NAME.xcarchive"
"$XCODEBUILD" archive -project "$TARGET_PATH/$TARGET_NAME.xcodeproj" -scheme "$TARGET_NAME" -archivePath "$ARCHIVE_PATH"
[ $? -ne 0 ] && echo "archive failed" && exit
echo "archive done"

echo "Start zip dSYMs ..."
pushd "${ARCHIVE_PATH}"

DSYM_PATH=dSYMs
DSYM_ZIP_NAME="${PROJECT_NAME}${TIME}.dSYMs.zip"
zip -r "${DSYM_ZIP_NAME}" "${DSYM_PATH}"

popd
echo "zip dSYMs done"

echo "Start export IPA ..."
"$XCODEBUILD" -exportArchive -archivePath "$ARCHIVE_PATH" -exportPath "$TARGET_PATH/build" -exportOptionsPlist "$EXPORT_OPTIONS_PLIST"
echo "export IPA done"

BUNDLE_PATH="${TARGET_PATH}/${PROJECT_NAME}${TIME}.ipa"
mv -f "${ARCHIVE_PATH}/${DSYM_ZIP_NAME}" "${TARGET_PATH}/${DSYM_ZIP_NAME}"
mv -f "${TARGET_PATH}/build/${TARGET_NAME}.ipa" "$BUNDLE_PATH"
echo "creating IPA done. $BUNDLE_PATH"

echo "Build done"

