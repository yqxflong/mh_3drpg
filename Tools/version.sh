#!/bin/bash
########################################
# $1 relative path to version file
# This version assumes format 0.0.0.0d 
#  - major.minor.patch.revision 
#  - d is development version
########################################
FILE=`dirname $0`'/../'$1;
BUILD_TYPE=$2;                  # version.type
CURRENT_VERSION=`cat $FILE`;
#REV=`/usr/local/bin/p4 changes -m1 //gods/... | awk '{print $2}'`;
REV=`/Applications/Utilities/p4 changes -m1 //gods/... | awk '{print $2}'`;
# Get rid of default build type
# e.g. 0.0.0.1d -> 0.0.0.1
version=${CURRENT_VERSION//d/};

declare -a current=(${version//\./ });


if [ "$BUILD_TYPE" = "d" ]; # Development Versioning
then
	# Make sure that the format is 0.0.0.0
	if [ "${current[3]}" = "" ];
	then
		current[3]=0;
	fi;

	# Get SVN revision number
	
	current[3]=$REV;
	tmp=${current[*]};
	new=${tmp// /.}$BUILD_TYPE;

elif [ "$BUILD_TYPE" = "r" ]; # Release Versioning
then
	current[3]=$REV;
	tmp=${current[*]};
	new=${tmp// /.};
elif [ "$BUILD_TYPE" = "rc" ]; # Release Versioning
then
	current[3]=$REV;
	tmp=${current[*]}"rc";
	new=${tmp// /.};
else # Adhoc
	#current[2]=$((${current[2]} + 1)); No longer increasing this automatically
	current[3]=$REV;
	tmp=${current[*]};
	new=${tmp// /.};
fi;

# update to new version
echo -n $new > $FILE;

