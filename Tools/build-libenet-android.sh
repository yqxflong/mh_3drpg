#!/bin/bash
set -uex

#require android-ndk-11b

TOOLS_PATH="$(dirname $0)"
TOOLS_PATH="$(cur=$(pwd); cd $TOOLS_PATH; pwd -P; cd $cur)"
BUILD_PATH="$(dirname $TOOLS_PATH)/Build"
PROJECT_PATH="$(dirname $TOOLS_PATH)/Source"

if [ -n "$(uname -s | grep CYGWIN)" ]; then
	ANDROID_SDK_PATH="$LOCALAPPDATA/Android/sdk"
	ANDROID_NDK_PATH="$ANDROID_SDK_PATH/ndk-bundle"

	if [ -d "$PROGRAMFILES/Java" ]; then
		pushd "$PROGRAMFILES/Java"
		JDK_PATH="$(pwd)/$(ls | grep -o jdk.*)"
		popd
		PATH="$JDK_PATH/bin:$PATH"
	else
		JDK_PATH=""
	fi
	
	# android-ndk-11b
	OSTYPE=""
	export OSTYPE
else
	ANDROID_SDK_PATH="/$HOME/Library/Android/sdk"
	ANDROID_NDK_PATH="/$HOME/Library/Android/sdk/ndk-bundle"
	JDK_PATH="/System/Library/Frameworks/JavaVM.framework/Versions/Current"
fi

[ ! -d "$ANDROID_SDK_PATH" ] && echo "SDK not found" && exit
[ ! -d "$ANDROID_NDK_PATH" ] && echo "NDK not found" && exit
[ ! -d "$JDK_PATH" ] && echo "JDK not found" && exit

# Setup architectures, library name and other vars + cleanup from previous runs
ARCHS=("arm" "arm64" "x86" "x86_64")
HOSTFLAGS=("--host=arm-linux-androideabi" "--host=aarch64-linux-android" "--host=i686-linux-android" "--host=x86_64-linux-android")
PLATFORMS=("android-21" "android-21" "android-21" "android-21")
TOOLCHAINS=("arm-linux-androideabi-4.9" "aarch64-linux-android-4.9" "x86-4.9" "x86_64-4.9")
LIB_NAME="enet-1.3.13"

LIB_DEST_DIR="$BUILD_PATH/libenet-dest-lib"
HEADER_DEST_DIR="$BUILD_PATH/libenet-dest-include"

TEMP_DIR="$BUILD_PATH/tmp"
TEMP_LIB_PATH="$TEMP_DIR/${LIB_NAME}"

rm -rf "$TEMP_DIR"

configure_make()
{
	ARCH=$1; PLATFORM=$2; TOOLCHAIN=$3; HOSTFLAG=$4

	TOOL_CHAIN_PATH="$TEMP_DIR/toolchain/${PLATFORM}-${ARCH}"
	
	"${ANDROID_NDK_PATH}/build/tools/make-standalone-toolchain.sh" \
	--platform=$PLATFORM \
	--install-dir=$TOOL_CHAIN_PATH \
	--toolchain=$TOOLCHAIN \
	--arch=$ARCH
	
	PATH="$TOOL_CHAIN_PATH/bin:$PATH"

	tar xfz "${LIB_NAME}.tar.gz";

	pushd "${LIB_NAME}";

	mkdir -p "${TEMP_LIB_PATH}-${ARCH}"

	./configure --prefix="${TEMP_LIB_PATH}-${ARCH}" ${HOSTFLAG} --disable-static --enable-shared

	make -j2 && make install;

	#clean
	popd;
	rm -rf "${LIB_NAME}";
	rm -rf "$TOOL_CHAIN_PATH"
}

ARCHDIRS=("armeabi-v7a" "arm64-v8a" "x86" "x86_64")
LIBS=("libenet.so")
PROJECT_LIBS=("libenetlib.so")
for ((i=0; i < ${#ARCHS[@]}; i++)); do
	configure_make "${ARCHS[i]}" "${PLATFORMS[i]}" "${TOOLCHAINS[i]}" "${HOSTFLAGS[i]}"
   
	for ((j=0; j < ${#LIBS[@]}; j++)); do
		cp "${TEMP_LIB_PATH}-${ARCHS[i]}/lib/${LIBS[j]}" "${PROJECT_PATH}/Assets/Plugins/Android/libs/${ARCHDIRS[i]}/${PROJECT_LIBS[j]}"
	done
done
