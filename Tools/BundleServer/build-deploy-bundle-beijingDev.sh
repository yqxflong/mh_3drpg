#!/bin/bash
###########################################################################
#       Shell program to build and deploy bundles
#
###########################################################################

GMPROJECTPATH=$(pwd)"/../..""/Source"
echo $GMPROJECTPATH
UNITYDIR="/Applications/Unity/Unity.app/Contents/MacOS/Unity" 
SERVERFLAG=-1

SITEDIR="~/Sites/"
if [ -d ~/Sites ] 
                then
                echo "CHECK: The directory of $SITEDIR is exists"
else
		echo "make dir for Sites"
		mkdir ~/Sites
fi

function buidAndDeploy
{	
	#get latest code first
	
	cd ~/gam/gam/
	unityProcess=$(pgrep "Unity")
	if [ ! -z "$unityProcess" ]; 
		then
    		echo "CHECK: Unity is running";
		echo "======================================================"
		echo "= NOTE: Please first save your work and close Unity  ="
		echo "======================================================"
		#kill -9 $unityProcess
		exit 1	
	else
		echo "NOTE: Relaunched Unity and run BuildForDeploy"
		$UNITYDIR -projectPath ${GMPROJECTPATH} -batchmode -executeMethod BuildHelper.BuildForDeploy -quit
	fi
	#relaunch Unity
	echo "NOTE: Build and deploy bundle is success!"
        echo "      Bundle location is: ~/Sites"

}

function startLocalServer
{
	#start http-server
	count=$(ps auxwww|grep http-server|wc -l|tr -d ' ')
	if [ ${count} -eq 2 ];
                then
                echo 'CHECK: http-server is already running'
		echo "       Link: http://localhost:9090"
        else
		#forever start -o out.log -e err.log /usr/local/bin/http-server -p 9090
		cd ~/Sites
        	sudo /usr/local/bin/node /usr/local/bin/forever stopall
		sudo /usr/local/bin/node /usr/local/bin/forever start /usr/local/bin/http-server -p 9090 
		echo "NOTE: http-server is running"
		echo "==============================="
		echo "= Link: http://localhost:9090 ="
		echo "==============================="
	fi
}

startLocalServer

buidAndDeploy

