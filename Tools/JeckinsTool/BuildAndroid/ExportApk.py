# -*- coding:utf8 -*-

#导出打的包
import os
import time
import shutil

##环境参数
NODE = os.environ.get('NODE', 'master')
LTSITES_PATH = os.environ.get('LTSITES', 'UNKNOWN') 
JENKINS_WORKSPACE = os.environ.get('JENKINS_WORKSPACE', 'UNKNOWN')
PROJECTPATH = "/Volumes/DataDisk/LT_Android/client_gam/Source"
APK_DEV_SRC = '/Volumes/DataDisk/LT_Android/client_gam/Build/Client/Temp/Client-Android/mhj_dev_mono.apk'
APK_RELEASE_SRC = '/Volumes/DataDisk/LT_Android/client_gam/Build/Client/Client-Android/mhj_release_mono.apk'
OBB_RELEASE_FROM = '/Volumes/DataDisk/LT_Android/client_gam/Build/Client/Client-Android'

jdebug = os.getenv('Debug')
jwithLog = os.getenv('WithLog')

def GetVersion():
    file_version = PROJECTPATH + '/Assets/Resources/version.txt'
    version = ""
    with open(file_version, "r", encoding="utf-8") as f:
        version = f.readline()
    
    print("当前版本号: " + version)
    return version

def GetVersionCode():
    version = GetVersion()
    v = version.split('.')
    return v[-1]

def ExportApk(pkgType):
    print("正在导出最终APK包......")
    current = time.strftime('%Y_%m_%d_%H_%M_%S',time.localtime(time.time()))
    debug_release = "debug"
    withLog = ""
    
    if jdebug == "false":
        debug_release = "release"
        
    if jwithLog == "true":
        withLog = "_WithLog"
        
    newApkName = current + "_" + pkgType + "_" + debug_release + "_" + GetVersion() + withLog + ".apk"
    print("新包名：" + newApkName)
    apkFrom = APK_DEV_SRC
    if debug_release == "release":
        apkFrom = APK_RELEASE_SRC
    shutil.move(apkFrom, JENKINS_WORKSPACE + "/" + newApkName)
    #导出obb
    obbextend = current + "_" + pkgType + "_" + GetVersion() + withLog
    ExportOBB(obbextend)
    
def ExportOBB(obbextend):
    print("正在导出最终OBB......")
    for root,dirs,files in os.walk(OBB_RELEASE_FROM):
        for file in files:
            if file.endswith(".obb"):
                versionCode = GetVersionCode()
                #先给ast写死
                bundleId = "com.outstandinggame.divinityand"
                shutil.move(OBB_RELEASE_FROM + "/" + file, JENKINS_WORKSPACE + "/main." + versionCode + "." + bundleId + ".obb_" + obbextend)
                break
    

def ExportAB():
    print("正在导出最终AB包......")
    version_ab = ''
    version = GetVersion()
    v = version.split('.')
    v.pop(-1)
    version_ab = '.'.join(v)
    if os.path.exists(LTSITES_PATH + "/" + version_ab):
        os.chdir(LTSITES_PATH)
        os.system("zip -r " + version_ab + ".zip " + version_ab)
        current = time.strftime('%Y_%m_%d_%H_%M_%S',time.localtime(time.time()))
        newZipName = "AB_" + current + "_" + version_ab + ".zip"
        shutil.copy(version_ab + ".zip", JENKINS_WORKSPACE + "/" + newZipName)
        print("删除本次构建的AB包的压缩文件==========>")
        os.system("rm -rf " + version_ab + ".zip")
    else:
        print("不存在此AB包，导出失败, version: " + version_ab)