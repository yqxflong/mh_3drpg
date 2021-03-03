#构建IOS主入口
#Johny

#Const
PROJECT_PATH=/Volumes/DataDisk/LT_Android/client_gam/Source
HOTFIX_LT_FROM=/Volumes/DataDisk/LT_Android/client_gam/Source/Temp/bin/Release/Unity_Hotfix.dll
HOTFIX_LT_TO=/Volumes/DataDisk/LT_Android/client_gam/Source/Assets/Hotfix/HotfixDLLForFinalAB/Hotfix_LT.bytes


function IncVersionBundleNum(){
  arr=(${Version//./ })  
  arrCnt=${#arr[@]}-1
  num=$((${arr[$arrCnt]}+1))
  Version=${arr[0]}"."${arr[1]}"."${arr[2]}"."$num
}

function CleanBuildCache(){
    cd /Volumes/DataDisk/LT_Android
    git checkout .
    git clean -df
}

function RestartUnity(){
    echo "重启Unity..."
    $UNITY -quit -projectPath $PROJECT_PATH
}

function BuildDll(){
    cd /Volumes/DataDisk/LT_Android/client_gam/Tools/JeckinsTool
    /usr/local/bin/python3.9 SetHotfixDllProjectRight.py -t "android"
    rm -rf /Volumes/DataDisk/LT_Android/client_gam/Source/Temp
    msbuild=/Library/Frameworks/Mono.framework/Versions/Current/Commands/msbuild
    $msbuild /Volumes/DataDisk/LT_Android/client_gam/Source/Unity_Hotfix.csproj /t:Clean,Build /p:configuration="release"
    if cp $HOTFIX_LT_FROM $HOTFIX_LT_TO;then
        echo "Copy Hotfix_LT.dll成功"
    else
        echo "不存在Hotfix_LT.dll,构建失败！！"
        exit    
    fi
}


function BuildOnce(){
	_pkg=$1
    _lang=$2
    _version=$3
    _onlyAB=$4
    _debug=$5
    _withLog=$6
    _useLB=$7
    _useBugly=$8

    echo "==========>正在构建： "$_pkg"==Version: "$_version

    #执行打包脚本
    cd /Volumes/DataDisk/LT_Android/client_gam/Tools/JeckinsTool/BuildAndroid
    /usr/local/bin/python3.9 BuildAndroid.py -t $_pkg -l $_lang -v $_version -o $_onlyAB -d $_debug -g $_withLog -e $_useLB -b $_useBugly
}

#################################Main###################################### 
#防止小版本号为0，所以启动前先+1
IncVersionBundleNum

#空启一次Unity
if [ $RestartUnityOnce == true ];then
    RestartUnity
fi

#打Hotfix.dll
if [ "$pkg_type" != "astgoogle" ];then
    BuildDll
fi

#Main
if [ "$pkg_type" == "allchannel" ];then
	echo "正在批量打包，包含以下渠道："
    echo $allchannelValue
    channelKeys=(${allchannelValue//,/ })
    for tp in ${channelKeys[@]}  
    do  
        BuildOnce $tp $Lang $Version false $Debug $WithLog $use_lebian $use_bugly
        #不再重复打AB
        buildAB_type=false
        #清除打包缓存
        CleanBuildCache
        #每一个包小版本号+1
        IncVersionBundleNum
    done 
else
	BuildOnce $pkg_type $Lang $Version $OnlyAB $Debug $WithLog $use_lebian $use_bugly
fi