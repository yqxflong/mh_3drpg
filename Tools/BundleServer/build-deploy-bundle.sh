#!/bin/bash
###########################################################################
#       Shell program to build and deploy bundles
#
###########################################################################

#GMPROJECTPATH=$(pwd)
#cd ..
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
	# check whether the Unity is launch
	#echo "check whether unity is running"
	unityProcess=$(pgrep "Unity")
	if [ ! -z "$unityProcess" ]; 
		then
    		echo "CHECK: Unity is running";
		echo "======================================================"
		echo "= NOTE: Please first save your work and close Unity  ="
		echo "======================================================"
		#kill -9 $unityProcess
		exit	
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
	#check whether install node and http-server
	node=$(which node)
 	http=$(which http-server)
	forE=$(which forever)

	if [ ! -z "$node" ]
		then 
		echo "CHECK: node installed"
	else
		echo "Install node ..."
		cd /usr/bin
                sudo git clone git://github.com/ry/node.git
                cd node
                sudo ./configure
                sudo make
                sudo make install
	fi
	if [ ! -z "$http" ]
		then 
		echo "CHECK: http-server installed"
	else
		echo "Install http-server"
		sudo npm install http-server -g
	fi
	if [ ! -z "$forE" ]
		then	
		echo "CHECK: forever installed"
	else
		echo "Install forever"
		sudo npm -g install forever
	fi

	#start http-server
	count=$(forever list|grep http-server|wc -l|tr -d ' ')
	if [ ${count} -eq 1 ];
                then
                echo 'CHECK: http-server is running, will restart'
		forever stopall
		cd ~/Sites/
		echo "---$(pwd)"
                forever start /usr/local/bin/http-server -p 9090
		echo "       Link: http://localhost:9090"
        else
		#forever start -o out.log -e err.log /usr/local/bin/http-server -p 9090
		cd ~/Sites
		forever start /usr/local/bin/http-server -p 9090
        	echo "NOTE: http-server is running"
		echo "==============================="
		echo "= Link: http://localhost:9090 ="
		echo "==============================="
	fi
}



function pushToRemoteServer
{
	#zip Sites folder
	tarName="sites.zip"
	cd ~/Sites/ 
        echo "tar -czf ${tarName} *"
        tar -czf ${tarName} *
	echo "push to remote server"
	
	#rsync tar to remote server
	rsync -e ssh -arvzP --timeout=30 ~/Sites/sites.zip root@10.80.0.191:/root/testRsync

	#unzip the file on remote server
	ssh root@10.80.0.191 "cd /root/testRsync; tar zxvf sites.zip; rm -f sites.zip; "

		
}

startLocalServer

buidAndDeploy

#pushToRemoteServer
