# -*- coding:utf8 -*-

#导出打的包
import os
import time
import shutil


##环境参数
NODE = os.environ.get('NODE', 'master')
LTSITES_PATH = os.environ.get('LTSITES', 'UNKNOWN') 
JENKINS_WORKSPACE = os.environ.get('JENKINS_WORKSPACE', 'UNKNOWN')
PROJECTPATH = "/Volumes/DataDisk/LT_IOS/client_gam/Source"
IOS_BUILD_PATH = "/Volumes/DataDisk/LT_IOS/client_gam/Build/Client/Temp/Client-iOS"
IPA_SRC = IOS_BUILD_PATH + '/mh.ipa'

jpkgType = os.getenv('pkg_type')
jdebug = os.getenv('Debug')
jwithLog = os.getenv('WithLog')

def GetVersion():
    file_version = PROJECTPATH + '/Assets/Resources/version.txt'
    version = ""
    with open(file_version, "r", encoding="utf-8") as f:
        version = f.readline()
    
    print("当前版本号: " + version)
    return version

def ExportIPA():
    print("正在导出最终ipa包......")
    current = time.strftime('%Y_%m_%d_%H_%M_%S',time.localtime(time.time()))
    debug_release = "debug"
    withLog = ""
    
    if jdebug == "false":
        debug_release = "release"
        
    if jwithLog == "true":
        withLog = "_WithLog"
        
    newApkName = current + "_" + jpkgType + "_" + debug_release + "_" + GetVersion() + withLog + ".ipa"
    print("新包名：" + newApkName)
    shutil.copy(IPA_SRC, JENKINS_WORKSPACE + "/" + newApkName)
    
def ExportXcode():
    print("正在导出最终Xcode工程......")
    version = GetVersion()
    if os.path.exists(IOS_BUILD_PATH + "/XcodeProject"):
        os.chdir(IOS_BUILD_PATH)
        os.system("zip -r XcodeProject.zip XcodeProject")
        current = time.strftime('%Y_%m_%d_%H_%M_%S',time.localtime(time.time()))
        debug_release = "debug"
        withLog = ""
    
        if jdebug == "false":
            debug_release = "release"
            
        if jwithLog == "true":
            withLog = "_WithLog"
        newZipName = "Xcode_" + current + "_" + jpkgType + "_" + debug_release + "_" + version + withLog + ".zip"
        shutil.copy("XcodeProject.zip", JENKINS_WORKSPACE + "/" + newZipName)
        os.system("rm -rf XcodeProject.zip")
    else:
        print("不存在此Xcode工程，导出失败, version: " + version)

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
        #删除本次构建的AB包压缩文件
        os.system("rm -rf " + version_ab + ".zip")
    else:
        print("不存在此AB包，导出失败, version: " + version_ab)