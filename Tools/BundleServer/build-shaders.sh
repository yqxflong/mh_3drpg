#!/bin/bash
###########################################################################
#       Shell program to build and deploy shaders
#
###########################################################################

GMPROJECTPATH=$(pwd)"/../..""/Source"
echo $GMPROJECTPATH
UNITYDIR="/Applications/Unity/Unity.app/Contents/MacOS/Unity" 

SHADERDIR=$GMPROJECTPATH"/Assets/Merge/Shaders"


function buidAndDeploy
{	
	
	# check whether the Unity is launch
	unityProcess=$(pgrep "Unity")
	if [ ! -z "$unityProcess" ]; 
		then
    		echo "CHECK: Unity is running";
		echo "======================================================"
		echo "= NOTE: Please first save your work and close Unity  ="
		echo "======================================================"
		exit	
	else
		echo "NOTE: Relaunched Unity\n"
		echo "Run the command of CompositeAllShaders.ToComposited"
		$UNITYDIR -projectPath ${GMPROJECTPATH} -batchmode -executeMethod CompositeAllShaders.ToComposited -quit
		echo "Run the command of CompositeAllShaders.ToUber"
		$UNITYDIR -projectPath ${GMPROJECTPATH} -batchmode -executeMethod CompositeAllShaders.ToUber -quit
	fi
	#delete the new created shaders
	if [ -d "$SHADERDIR" ]
                then
		rm -rf $SHADERDIR
		echo "Delete the new created shaders."
	fi
	echo "NOTE: Build and deploy shadersis success!"

}


buidAndDeploy
