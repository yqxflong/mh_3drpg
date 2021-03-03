#!/bin/bash
# Remove untracked files
CMD_SVN=`which svn`;
CMD_AWK=`which awk`;

if [ "$CMD_SVN" = "" ];
then
	echo "ERROR - no git installed";
fi;

if [ "$CMD_AWK" = "" ];
then
	echo "ERROR - no awk installed";
fi;

#untracked=`$CMD_SVN status | grep ^?`;
#echo $untracked;

svn status | grep ^? | awk '{print $2}' | sed 's/^/.\//g' | xargs rm -R

