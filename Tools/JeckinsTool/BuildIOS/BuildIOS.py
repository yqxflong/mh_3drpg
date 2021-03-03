# -*- coding:utf8 -*-

import subprocess
import shutil
import os,sys
import argparse
import BuildXcode
import ExportIPA

#################Jeckins对应参数名
#PKG_TYPE
JPARAM_PKG_TYPE = "pkg_type"
JPARAM_PKG_TYPE_INNER = "inner"
JPARAM_PKG_TYPE_XK = "xk"
JPARAM_PKG_TYPE_AST = "ast"

print('当前操作系统: ' + sys.platform)

##环境参数
NODE = os.environ.get('NODE', 'master')
UNITYPATH = os.environ.get('UNITY', 'UNKNOWN')
LTSITES_PATH = os.environ.get('LTSITES', 'UNKNOWN') 
PROJECTPATH = "/Volumes/DataDisk/LT_IOS/client_gam/Source"
FILE_UNITYLOG = "/tmp/jenkins_ios.log"
IOS_BUILD_PATH = "/Volumes/DataDisk/LT_IOS/client_gam/Build/Client/Temp/Client-iOS"
IPA_SRC = IOS_BUILD_PATH + '/mh.ipa'
PATH_BUGLY_SRC_FROM = PROJECTPATH + '/Sdk/Bugly/src'
PATH_BUGLY_SRC_TO = PROJECTPATH + '/Assets/_ThirdParty/Sdk/Bugly'
#执行Unity的命令
CMD_BUILD = UNITYPATH + " -quit" + " -logFile " + FILE_UNITYLOG + " -projectPath " + PROJECTPATH
#Login_Brand路径
PATH_LOGIN_BRAND = PROJECTPATH + '/Assets/Resources/UI/Textures'
#LoadingBG路径
PATH_LOADINGBG = PROJECTPATH + '/Assets/Resources/UI/Textures/LoadingBG'
PATH_LOADINGBG_FROM = PROJECTPATH + '/../Tools/JeckinsTool/loading/xinkuai'
#ReplaceableRes路径
PATH_REPLACEABLERES = PROJECTPATH + '/Assets/StreamingAssets/ReplaceableRes'
PATH_REPLACEABLERES_FROM_MHHX = PROJECTPATH + '/../Tools/JeckinsTool/mhhx'
PATH_REPLACEABLERES_FROM_SLHX = PROJECTPATH + '/../Tools/JeckinsTool/slhx'
PATH_REPLACEABLERES_FROM_MKHX = PROJECTPATH + '/../Tools/JeckinsTool/mkhx'
PATH_REPLACEABLERES_FROM_ASTGOOGLE = PROJECTPATH + '/../Tools/JeckinsTool/astgoogle'
#Audio路径
PATH_AUDIO_FROM = PROJECTPATH + '/../Tools/JeckinsTool/astgoogle/Audio/AudioSource'
PATH_AUDIO_TO = PROJECTPATH + '/Assets/_GameAssets/Res/Audio/AudioSource'
##################################Main####################################
#读取配置参数
parser = argparse.ArgumentParser("读取Jeckins传参")
parser.add_argument('-v', '--Version', default="0.0.0.0")
parser.add_argument('-t', '--pkg_type', default="inner")
parser.add_argument('-l', '--Lang', default="ZHCN")
parser.add_argument('-o', '--BuildType', default="XcodeProject")
parser.add_argument('-d', '--Debug', default="true")
parser.add_argument('-g', '--WithLog', default="false")
parser.add_argument('-b', '--use_bugly', default="true")
args = parser.parse_args()

#Common Used
version = args.Version
pkg_type = args.pkg_type
lang = args.Lang
buildType = args.BuildType
#Only整包会用到
isDebug = args.Debug
withLog = args.WithLog
usebugly = args.use_bugly
##############

print("===================Jenkins配置参数 Begin===============")
print("===version: " + version)
print("===pkg_type: " + pkg_type)
print("===lang: " + lang)
print("===buildType: " + buildType)
print("===isDebug: " + isDebug)
print("===withLog: " + withLog)
print("===================Jenkins配置参数 End===============")
######################
#输出Unity打包Log信息
def PrintAllUnityLog():
    print("=============================Begin 此次Unity打包的Log===============================")
    if os.path.exists(FILE_UNITYLOG):
        with open(FILE_UNITYLOG, "r", encoding="utf-8") as f:
            for line in f.readlines():
                print(line)
    print("=============================End 此次Unity打包的Log===============================")

#修改版本号
def ChangeVersion():
    print("===================Begin 修改版本号====================")
    file_version = PROJECTPATH + '/Assets/Resources/version.txt'
    with open(file_version, "w", encoding="utf-8") as f:
        f.write(version)
    print("===================End 修改版本号====================")
    
#将对应的音频替换到项目
def ReplaceAudios(audioSubPath, ext):
    srcDLG = PATH_AUDIO_FROM + audioSubPath
    dstDLG = PATH_AUDIO_TO + audioSubPath
    if not os.path.exists(srcDLG) or not os.path.exists(dstDLG):
        print("音频文件替换失败==不存在: " + srcDLG + "或" + dstDLG)
        return
    
    dlgs_dst = os.listdir(dstDLG)
    for i in range(0, len(dlgs_dst)):
        path_dst = os.path.join(dstDLG, dlgs_dst[i])
        if path_dst.endswith(ext):
            os.remove(path_dst)
            
    dlgs_src = os.listdir(srcDLG)
    for i in range(0, len(dlgs_src)):
        path_src = os.path.join(srcDLG, dlgs_src[i])
        path_dst = os.path.join(dstDLG, dlgs_src[i])
        if path_src.endswith(ext):
            shutil.copy(path_src, path_dst)

#拷贝bugly库
def HandleCopyBugly():
    if usebugly == "true":
        #copy src
        shutil.copytree(PATH_BUGLY_SRC_FROM, PATH_BUGLY_SRC_TO)
        
    
def FinalBuild_OnlyAB():
    print("===================Begin 仅构建AB====================")
    # 修改包名
    finalCMD = CMD_BUILD + ' -executeMethod PerformBuild.Automation.HandleOnlyBuildABForIOS'
    print(finalCMD)    
    retcode = subprocess.call(finalCMD, shell=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT) 
    PrintAllUnityLog()
    if retcode != 0:
        print("AB构建失败，退出构建!!!")
        exit()
    print("===================End 仅构建AB====================") 

#构建Xcode工程
def FinalBuild_Xcode():
    print("===================Begin 构建Xcode工程====================")
    finalCMD = CMD_BUILD + ' -executeMethod PerformBuild.Automation.HandlePackageIOS'
    # 修改包名
    if pkg_type == JPARAM_PKG_TYPE_AST:
        finalCMD += ' -company manhuangji -product Divinity\\\'s\ Rise'
    else:
        finalCMD += ' -company manhuangji -product 蛮荒幻想'
    # debug and log
    if isDebug == "true":
        finalCMD += ' -debug True'
    if withLog == "true":
        finalCMD += ' -withLog True'
    # 包类型
    finalCMD += ' -pkg ' + pkg_type
    #lang
    finalCMD += ' -lang ' + lang
    
    print(finalCMD)  
    
    retcode = subprocess.call(finalCMD, shell=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)
    PrintAllUnityLog()
    print("===================End 构建Xcode工程====================")   
    
#确保AB下没有Android资源 
def RemoveABAndroidByVersion():
    v = version.split('.')
    v.pop(-1)
    version_ab = '.'.join(v)
    if os.path.exists(LTSITES_PATH + "/" + version_ab):
        os.system("rm -rf " + LTSITES_PATH + "/" + version_ab + "/Android")
        
        
#for astgoogle
def OnlyAST():
    #将对应英语音频文件替换到项目
    ReplaceAudios('/DLG', '.mp3')
    ReplaceAudios('/SFX/Character/Appear', '.wav')
    #logoBrand
    shutil.copy(PATH_REPLACEABLERES_FROM_ASTGOOGLE + '/Login_Brand.png', PATH_LOGIN_BRAND + '/Login_Brand.png')
    shutil.copy(PATH_REPLACEABLERES_FROM_ASTGOOGLE + '/Login_Brand.png', PATH_REPLACEABLERES + '/Login_Brand.png')
 
###############################Main###################################
if version == "0.0.0.0":
    print("版本号是必须要的参数!!!")
    exit()
    
print("======================打包开始========================")
#iOS每次打包前要删除之前的Xcode工程
os.system("rm -rf /Volumes/DataDisk/LT_IOS/client_gam/Build/Client/Temp/Client-iOS/XcodeProject")

#删除上一次打包的缓存
os.system("rm -rf " + IPA_SRC)

#version
ChangeVersion()

#确保AB下没有Android资源 
RemoveABAndroidByVersion()

#only ast
if pkg_type == JPARAM_PKG_TYPE_AST:
    OnlyAST()

#Main Process
buildAB = os.environ.get('buildAB_type', 'no')
if buildType == "OnlyAB":
    FinalBuild_OnlyAB()
    ExportIPA.ExportAB()  
elif buildType == "XcodeProject": 
    FinalBuild_Xcode()
    if buildAB != "no":
        ExportIPA.ExportAB()
    ExportIPA.ExportXcode()  
elif buildType == "IPA":
    FinalBuild_Xcode()
    if buildAB != "no":
        ExportIPA.ExportAB()
    ExportIPA.ExportXcode()
    BuildXcode.BuildIPA()
    ExportIPA.ExportIPA()
print("======================打包结束========================")