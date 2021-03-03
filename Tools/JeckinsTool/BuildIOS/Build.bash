#构建IOS主入口
#Johny

#Const
PROJECT_PATH=/Volumes/DataDisk/LT_IOS/client_gam/Source
HOTFIX_LT_FROM=/Volumes/DataDisk/LT_IOS/client_gam/Source/Temp/bin/Release/Unity_Hotfix.dll
HOTFIX_LT_TO=/Volumes/DataDisk/LT_IOS/client_gam/Source/Assets/Hotfix/HotfixDLLForFinalAB/Hotfix_LT.bytes


function IncVersionBundleNum(){
  arr=(${Version//./ })  
  arrCnt=${#arr[@]}-1
  num=$((${arr[$arrCnt]}+1))
  Version=${arr[0]}"."${arr[1]}"."${arr[2]}"."$num
}

function RestartUnity(){
    echo "重启Unity..."
    $UNITY -quit -projectPath $PROJECT_PATH
}

function BuildDll(){
    cd /Volumes/DataDisk/LT_IOS/client_gam/Tools/JeckinsTool
    /usr/local/bin/python3.9 SetHotfixDllProjectRight.py -t "ios"
    rm -rf /Volumes/DataDisk/LT_IOS/client_gam/Source/Temp
    msbuild=/Library/Frameworks/Mono.framework/Versions/Current/Commands/msbuild
    $msbuild /Volumes/DataDisk/LT_IOS/client_gam/Source/Unity_Hotfix.csproj /t:Clean,Build /p:configuration="release"
    if cp $HOTFIX_LT_FROM $HOTFIX_LT_TO;then
        echo "Copy Hotfix_LT.dll成功"
    else
        echo "不存在Hotfix_LT.dll,构建失败！！"
        exit    
    fi
}

#################################Main######################################
#先升小版本号
IncVersionBundleNum

#空启一次Unity
echo $RestartUnityOnce
if [ $RestartUnityOnce == true ];then
    RestartUnity
fi

#编译Hotfix_LT.dll
BuildDll

echo "正在构建： "$pkg_type"==Version: "$Version

#执行打包脚本
cd /Volumes/DataDisk/LT_IOS/client_gam/Tools/JeckinsTool/BuildIOS
/usr/local/bin/python3.9 BuildIOS.py -v $Version -t $pkg_type -l $Lang -o $BuildType -d $Debug -g $WithLog -b $use_bugly