#!/bin/bash
###########################################################################
#       Shell program to update and checkin assets bundles
#
###########################################################################

#GMPROJECTPATH=$(pwd)
#cd ..
ASSETS=$(pwd)"/../..""/Source/Assets/StreamingAssets/AssetBundles"
echo $ASSETS
cd $ASSETS

echo "Start update Streaming Assets"
/usr/bin/git checkout feature/combat2
/usr/bin/git add *
/usr/bin/git commit -m "Jira: GAM-0; Description: Update Streaming Assets; Reviewer: none"
/usr/bin/git push
echo "Update Streaming Assets done"


