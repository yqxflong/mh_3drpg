# -*- coding:utf8 -*-

import subprocess
import shutil
import os,sys
import argparse
import ExportApk

#################Jeckins对应参数名
#PKG_TYPE
JPARAM_PKG_TYPE = "pkg_type"
JPARAM_PKG_TYPE_INNER = "inner"
JPARAM_PKG_TYPE_WECHAT = "wechat"
JPARAM_PKG_TYPE_XK = "xk"
JPARAM_PKG_TYPE_XKGDT = "xkgdt"
JPARAM_PKG_TYPE_XKTAP = "xktap"
JPARAM_PKG_TYPE_XKFX = "xkfx"
JPARAM_PKG_TYPE_XKX = "xkx"
JPARAM_PKG_TYPE_XKSLHX = "xkslhx"
JPARAM_PKG_TYPE_XKMKHX = "xkmkhx"
JPARAM_PKG_TYPE_ASTGOOGLE = "astgoogle"

#################乐变ID
LBID_PH = '<PLACEHOLDER_LBID>'
LBID_WECHAT = 'tap'
LBID_XK = 'xinkuai17'
LBID_XK_GDT = 'xinkuai19'
LBID_XK_TAP = 'xinkuaitap'
LBID_XK_FX = 'xinkuaifax'
LBID_XK_FX_PATCHV3 = 'PATCH_V3'
LBID_XK_X = 'xinkuaix'
LBID_XK_SLHX = 'xinkuaislhxx'
LBID_XK_MKHX = 'xinkuaimkhx'
############################


print('当前操作系统: ' + sys.platform)
##环境参数
NODE = os.environ.get('NODE', 'master')
UNITYPATH = os.environ.get('UNITY', 'UNKNOWN')
LTSITES_PATH = os.environ.get('LTSITES', 'UNKNOWN') 
PROJECTPATH_MAC = "/Volumes/DataDisk/LT_Android/client_gam/Source"
PROJECTPATH_WIN = "E:/Project/lt_trunk/client_gam/Source"
PROJECTPATH = PROJECTPATH_MAC
FILE_UNITYLOG = "/tmp/jenkins_android.log"
APK_DEV_SRC = '/Volumes/DataDisk/LT_Android/client_gam/Build/Client/Temp/Client-Android/mhj_dev_mono.apk'
APK_RELEASE_SRC = '/Volumes/DataDisk/LT_Android/client_gam/Build/Client/Client-Android/mhj_release_mono.apk'

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
#loading替换路径
PATH_LOADINGIMG = PROJECTPATH + '/Assets/Resources/UI/Textures/LoadingImg'
PATH_LOADINGIMG_FROM = PROJECTPATH + '/../Tools/JeckinsTool/loading/xinkuai'
#MainTemplateGralde
PATH_GRADLE_MAINTEMPLATE = PROJECTPATH + '/Assets/Plugins/Android/mainTemplate.gradle'
#乐变库路径
PATH_LB_LIB_FROM = PROJECTPATH + '/Sdk/Android/lebian'
PATH_LB_LIB_TO = PROJECTPATH + '/Assets/Plugins/Android/lebian'
#乐变ID配置路径
PATH_LBID_CONFIG = PROJECTPATH + '/Assets/Plugins/Android/lebian/lebian_cust.gradle'
#res/values
PATH_RES_VALUES = PROJECTPATH + '/Assets/Plugins/Android/res'
PATH_RES_VALUES_FROM_SLHX = PROJECTPATH + '/../Tools/JeckinsTool/slhx/Android/res'
PATH_RES_VALUES_FROM_MKHX = PROJECTPATH + '/../Tools/JeckinsTool/mkhx/Android/res'
#java libs
PATH_JAVALIB_APPCOMPATV7 = PROJECTPATH + '/Assets/Plugins/Android/appcompat-v7-26.1.0.aar'
PATH_JAVALIB_SUPPORTCOMPATV7 = PROJECTPATH + '/Assets/Plugins/Android/support-compat-26.1.0.aar'
#HotixScripts路径
PATH_HOTFIX_SCRIPTS_FROM = PROJECTPATH + '/Assets/_HotfixScripts'
PATH_HOTFIX_SCRIPTS_TO = PROJECTPATH + '/Assets/_GameAssets/Scripts/Game'
#bugly路径
PATH_BUGLY_JAR_FROM = PROJECTPATH + '/Sdk/Android/Bugly/jar'
PATH_BUGLY_JAR_TO = PROJECTPATH + '/Assets/Plugins/Android/bin'
PATH_BUGLY_LIB_FROM = PROJECTPATH + '/Sdk/Android/Bugly/libs'
PATH_BUGLY_LIB_TO = PROJECTPATH + '/Assets/Plugins/Android/libs'
PATH_BUGLY_SRC_FROM = PROJECTPATH + '/Sdk/Bugly/src'
PATH_BUGLY_SRC_TO = PROJECTPATH + '/Assets/_ThirdParty/Sdk/Bugly'
#AIHelp路径
PATH_AIHELP_AAR_FROM = PROJECTPATH + '/Sdk/Android/AIHelp'
PATH_AIHELP_AAR_TO = PROJECTPATH + '/Assets/Plugins/Android'
#AppsFlyer路径
PATH_APPSFLYER_AAR_FROM = PROJECTPATH + '/Sdk/Android/AppsFlyer'
PATH_APPSFLYER_AAR_TO = PROJECTPATH + '/Assets/Plugins/Android'
#Audio路径
PATH_AUDIO_FROM = PROJECTPATH + '/../Tools/JeckinsTool/astgoogle/Audio/AudioSource'
PATH_AUDIO_TO = PROJECTPATH + '/Assets/_GameAssets/Res/Audio/AudioSource'

##################################Main####################################
#需要替换Login_Brand的包
list_replace_logo = [JPARAM_PKG_TYPE_XK, JPARAM_PKG_TYPE_XKGDT, JPARAM_PKG_TYPE_XKTAP, JPARAM_PKG_TYPE_XKFX, JPARAM_PKG_TYPE_XKX]
#乐变ID对应的包类型
dic_replace_lbid = {JPARAM_PKG_TYPE_WECHAT : LBID_WECHAT, JPARAM_PKG_TYPE_XK : LBID_XK, JPARAM_PKG_TYPE_XKGDT : LBID_XK_GDT,
                    JPARAM_PKG_TYPE_XKTAP : LBID_XK_TAP, JPARAM_PKG_TYPE_XKX : LBID_XK_X,
                    JPARAM_PKG_TYPE_XKSLHX : LBID_XK_SLHX, JPARAM_PKG_TYPE_XKMKHX : LBID_XK_MKHX}
#新快SDK相关的包类型
list_replace_xk = [JPARAM_PKG_TYPE_XK, JPARAM_PKG_TYPE_XKGDT, JPARAM_PKG_TYPE_XKTAP, JPARAM_PKG_TYPE_XKFX, JPARAM_PKG_TYPE_XKX]

#读取配置参数
parser = argparse.ArgumentParser("读取Jeckins传参")
parser.add_argument('-v', '--Version', default="0.0.0.0")
parser.add_argument('-t', '--pkg_type', default=JPARAM_PKG_TYPE_INNER)
parser.add_argument('-l', '--Lang', default="ZHCN")
parser.add_argument('-o', '--OnlyAB', default="false")
parser.add_argument('-d', '--Debug', default="true")
parser.add_argument('-g', '--WithLog', default="false")
parser.add_argument('-e', '--use_lebian', default="true")
parser.add_argument('-b', '--use_bugly', default="true")
args = parser.parse_args()

#Common Used
version = args.Version
pkg_type = args.pkg_type
lang = args.Lang
onlyAB = args.OnlyAB
#Only整包会用到
isDebug = args.Debug
withLog = args.WithLog
uselebian = args.use_lebian
usebugly = args.use_bugly
##############

print("===================Jenkins配置参数 Begin===============")
print("===version: " + version)
print("===pkg_type: " + pkg_type)
print("===lang: " + lang)
print("===onlyAB:" + onlyAB)
print("===isDebug: " + isDebug)
print("===withLog: " + withLog)
print("===uselebian: " + uselebian)
print("===usebugly: " + usebugly)
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
    file_version = PROJECTPATH + '/Assets/Resources/version.txt'
    with open(file_version, "w", encoding="utf-8") as f:
        f.write(version)
        print("版本号修改为==========>verison: " + version)
        
        
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

####################Handle Lebian#############################
#更新乐变ID
def EditLBID(fromId, toId):
    lines = []
    with open(PATH_LBID_CONFIG, "r", encoding="utf-8") as f:
        lines = f.readlines()
    for idx in range(len(lines)):
        if fromId in lines[idx]:
            lines[idx] = lines[idx].replace(fromId, toId)
            print("修改乐变ID==========>lines[idx]: " + lines[idx])
            break
    with open(PATH_LBID_CONFIG, "w", encoding="utf-8") as f:
        f.writelines(lines)

#修改gradle中乐变注册信息
def UpdateGradleLBRegister(registerString):
    lines = []
    with open(PATH_GRADLE_MAINTEMPLATE, "r", encoding="utf-8") as f:
        lines = f.readlines()
    for idx in range(len(lines)):
        if "//<PLACEHOLDER_LEBIAN_REGISTER>" in lines[idx]:
            lines[idx] = registerString
            print("注册乐变gradle==========>lines[idx]: " + lines[idx])
            break
    with open(PATH_GRADLE_MAINTEMPLATE, "w", encoding="utf-8") as f:
        f.writelines(lines)
    
        
#装载乐变的库
def InstallLebian():
    if uselebian == "true":
        #拷贝乐变的库
        shutil.copytree(PATH_LB_LIB_FROM, PATH_LB_LIB_TO)
        #注册乐变的gradle
        strLBGradleRegister = \
        "apply plugin: 'com.loveota.lbsdk'\n\
apply from: project(':lebian').file('lebian_app.gradle')\n\
buildscript {\n\
    dependencies {\n\
        classpath 'com.loveota.plugins:lbplugin:latest.release'\n\
    }\n\
}\n"
        #注册乐变gradle
        UpdateGradleLBRegister(strLBGradleRegister)
        return True
    else:
        UpdateGradleLBRegister("")
        return False
#############################################################        

   
#slhx特殊处理        
def OnlySLHX():
    print("圣灵幻想的特殊操作==========>")
    #loading
    shutil.copy(PATH_LOADINGBG_FROM + '/loading_xinkuai_2.png', PATH_LOADINGBG + "/loading_xinkuai_2.png")
    shutil.copy(PATH_LOADINGBG_FROM + '/loading_xinkuai_3.png', PATH_LOADINGBG + "/loading_xinkuai_3.png")
    #all bg
    shutil.copy(PATH_REPLACEABLERES_FROM_SLHX + '/Loading_BG.png', PATH_REPLACEABLERES + '/Loading_BG.png')
    shutil.copy(PATH_REPLACEABLERES_FROM_SLHX + '/Login_BG.png', PATH_REPLACEABLERES + '/Login_BG.png')
    shutil.copy(PATH_REPLACEABLERES_FROM_SLHX + '/Login_Brand.png', PATH_REPLACEABLERES + '/Login_Brand.png')
    #strings
    shutil.copy(PATH_RES_VALUES_FROM_SLHX + '/values/strings.xml', PATH_RES_VALUES + '/values/strings.xml')
    shutil.copy(PATH_RES_VALUES_FROM_SLHX + '/values-zh-rCN/strings.xml', PATH_RES_VALUES + '/values-zh-rCN/strings.xml')
    shutil.copy(PATH_RES_VALUES_FROM_SLHX + '/values-zh-rTW/strings.xml', PATH_RES_VALUES + '/values-zh-rTW/strings.xml')

#mkhx特殊处理    
def OnlyMKHX():
    print("魔卡幻想的特殊操作==========>")
    #all bg
    shutil.copy(PATH_REPLACEABLERES_FROM_MKHX + '/Login_Brand.png', PATH_REPLACEABLERES + '/Login_Brand.png')
    #strings
    shutil.copy(PATH_RES_VALUES_FROM_MKHX + '/values/strings.xml', PATH_RES_VALUES + '/values/strings.xml')
    shutil.copy(PATH_RES_VALUES_FROM_MKHX + '/values-zh-rCN/strings.xml', PATH_RES_VALUES + '/values-zh-rCN/strings.xml')
    shutil.copy(PATH_RES_VALUES_FROM_MKHX + '/values-zh-rTW/strings.xml', PATH_RES_VALUES + '/values-zh-rTW/strings.xml')

#XK一系列包特殊处理    
def OnlyXK():
    print("新快SDK的特殊操作==========>")
    #移除冲突的aar
    if os.path.exists(PATH_JAVALIB_APPCOMPATV7):
        os.remove(PATH_JAVALIB_APPCOMPATV7)
        os.remove(PATH_JAVALIB_APPCOMPATV7 + '.meta')
    if os.path.exists(PATH_JAVALIB_SUPPORTCOMPATV7):
        os.remove(PATH_JAVALIB_SUPPORTCOMPATV7)
        os.remove(PATH_JAVALIB_SUPPORTCOMPATV7 + '.meta')
    #loading替换
    shutil.copy(PATH_LOADINGIMG_FROM + '/loading_xinkuai_1.png', PATH_LOADINGIMG + '/Loading_1.png')
    shutil.copy(PATH_LOADINGIMG_FROM + '/loading_xinkuai_2.png', PATH_LOADINGIMG + '/Loading_2.png')
    shutil.copy(PATH_LOADINGIMG_FROM + '/loading_xinkuai_1.png', PATH_LOADINGIMG + '/Loading_3.png')
    shutil.copy(PATH_LOADINGIMG_FROM + '/loading_xinkuai_2.png', PATH_LOADINGIMG + '/Loading_4.png')

################################Handle ASTGOOGLE#######################################
#AST出包处理
def OnlyAST():
    print("傲世堂SDK的特殊操作=========>")
    #将对应英语音频文件替换到项目
    ReplaceAudios('/DLG', '.mp3')
    ReplaceAudios('/SFX/Character/Appear', '.wav')
    #logoBrand
    shutil.copy(PATH_REPLACEABLERES_FROM_ASTGOOGLE + '/Login_Brand.png', PATH_LOGIN_BRAND + '/Login_Brand.png')
    shutil.copy(PATH_REPLACEABLERES_FROM_ASTGOOGLE + '/Login_Brand.png', PATH_REPLACEABLERES + '/Login_Brand.png')
    #移除冲突的aar
    if os.path.exists(PATH_JAVALIB_APPCOMPATV7):
        os.remove(PATH_JAVALIB_APPCOMPATV7)
        os.remove(PATH_JAVALIB_APPCOMPATV7 + '.meta')
    if os.path.exists(PATH_JAVALIB_SUPPORTCOMPATV7):
        os.remove(PATH_JAVALIB_SUPPORTCOMPATV7)
        os.remove(PATH_JAVALIB_SUPPORTCOMPATV7 + '.meta')
    #将AIHelp库拷到plugins下
    HandleCopyAIHelp()
    #将AppsFlyer库拷到plugins下
    HandleCopyAppsFlyer()
    #gradle中注册playservices库
    RegisterASTDependcyInGradle()


#gradle中注册playservices
def RegisterASTDependcyInGradle():
    strDependency = \
    "    implementation 'com.google.android.gms:play-services-base:15.0.0'\n\
    implementation 'com.google.android.gms:play-services-auth:16.0.0'\n\
    implementation 'com.google.android.gms:play-services-ads:16.0.0'\n\
    implementation 'com.facebook.android:facebook-login:4.37.0'\n\
    implementation 'com.google.android.gms:play-services-wallet:16.0.1'\n\
    implementation 'com.google.firebase:firebase-core:16.0.5'\n\
    implementation 'com.facebook.android:facebook-share:[4,5)'\n\
    implementation 'com.facebook.android:facebook-android-sdk:[4,5)'\n"
    lines = []
    with open(PATH_GRADLE_MAINTEMPLATE, "r", encoding="utf-8") as f:
        lines = f.readlines()
    for idx in range(len(lines)):
        if "//<PLACEHOLDER_PLAYSERVICES_REGISTER>" in lines[idx]:
            lines[idx] = strDependency
            print("注册PlayServices==========>lines[idx]: " + lines[idx])
            break
    with open(PATH_GRADLE_MAINTEMPLATE, "w", encoding="utf-8") as f:
        f.writelines(lines)    
    
#拷贝AIHelp库
def HandleCopyAIHelp():
    ll = os.listdir(PATH_AIHELP_AAR_FROM)
    for i in range(0, len(ll)):
        fromPath = os.path.join(PATH_AIHELP_AAR_FROM, ll[i])
        toPath = os.path.join(PATH_AIHELP_AAR_TO, ll[i])
        if fromPath.endswith(".aar"):
            shutil.copy(fromPath, toPath)

#拷贝AppsFlyer库
def HandleCopyAppsFlyer():
    ll = os.listdir(PATH_APPSFLYER_AAR_FROM)
    for i in range(0, len(ll)):
        fromPath = os.path.join(PATH_APPSFLYER_AAR_FROM, ll[i])
        toPath = os.path.join(PATH_APPSFLYER_AAR_TO, ll[i])
        if fromPath.endswith(".aar"):
            shutil.copy(fromPath, toPath)
#############################################################################

#热更代码合入主工程
def HandleMergeHotfixIntoMain():
    #将热更脚本拷到主工程代码路径下
    os.remove(PATH_HOTFIX_SCRIPTS_FROM + '/Unity_Hotfix.asmdef')
    os.remove(PATH_HOTFIX_SCRIPTS_FROM + '/Unity_Hotfix.asmdef.meta')
    shutil.move(PATH_HOTFIX_SCRIPTS_FROM, PATH_HOTFIX_SCRIPTS_TO + '/HotfixScripts')


#拷贝bugly库
def HandleCopyBugly():
    if usebugly == "true":
        #copy jar
        shutil.copy(PATH_BUGLY_JAR_FROM + '/bugly.jar', PATH_BUGLY_JAR_TO + '/bugly.jar')
        shutil.copy(PATH_BUGLY_JAR_FROM + '/buglyagent.jar', PATH_BUGLY_JAR_TO + '/buglyagent.jar')
        #copy libs
        shutil.copy(PATH_BUGLY_LIB_FROM + '/armeabi-v7a/libBugly.so', PATH_BUGLY_LIB_TO + '/armeabi-v7a/libBugly.so')
        shutil.copy(PATH_BUGLY_LIB_FROM + '/arm64-v8a/libBugly.so', PATH_BUGLY_LIB_TO + '/arm64-v8a/libBugly.so')
        shutil.copy(PATH_BUGLY_LIB_FROM + '/x86/libBugly.so', PATH_BUGLY_LIB_TO + '/x86/libBugly.so')
        #copy src
        shutil.copytree(PATH_BUGLY_SRC_FROM, PATH_BUGLY_SRC_TO)    
    
#仅打AB   
def FinalCallUnity_OnlyAB():
    print("===================Begin 最终打包(只打AB)====================")
    # 修改包名
    finalCMD = CMD_BUILD + ' -executeMethod PerformBuild.Automation.HandleOnlyBuildABForAndroid'
    print(finalCMD)    
    retcode = subprocess.call(finalCMD, shell=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT) 
    PrintAllUnityLog()
    if retcode != 0:
        print("AB构建失败，退出构建!!!")
        exit()
    print("===================End 最终打包(只打AB)====================") 

#最终打APK        
def FinalCallUnity():
    print("===================Begin 最终打包====================")
    finalCMD = CMD_BUILD + ' -executeMethod PerformBuild.Automation.HandlePackageAndroid'
    # 修改包名
    if pkg_type == JPARAM_PKG_TYPE_XKSLHX:
        finalCMD += ' -company manhuangji -product 圣灵幻想'
    elif pkg_type == JPARAM_PKG_TYPE_XKMKHX:
        finalCMD += ' -company manhuangji -product 魔卡幻想'
    elif pkg_type == JPARAM_PKG_TYPE_ASTGOOGLE:
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
    print("===================End 最终打包====================")
    
#确保AB下没有IOS资源 
def RemoveABIOSByVersion():
    v = version.split('.')
    v.pop(-1)
    version_ab = '.'.join(v)
    if os.path.exists(LTSITES_PATH + "/" + version_ab):
        os.system("rm -rf " + LTSITES_PATH + "/" + version_ab + "/IOS")

#AB包最终打包流程
def FinalBuild_OnlyAB():
    FinalCallUnity_OnlyAB()  
        
#新快发行最终打包流程
def FinalBuild_XKFX():
    buildAB = os.environ.get('buildAB_type', 'no')
    #本包
    hasInstallLB = InstallLebian()
    if hasInstallLB:
        EditLBID(LBID_PH ,LBID_XK_FX)
    FinalCallUnity()
    #export
    if buildAB != "no":
        ExportApk.ExportAB()
    ExportApk.ExportApk(pkg_type)
    #patchV3包
    os.environ.set('buildAB_type', 'no')
    if hasInstallLB:
        EditLBID(LBID_XK_FX, LBID_XK_FX_PATCHV3)
    FinalCallUnity()
    #export
    ExportApk.ExportApk(pkg_type + "_patchV3")
    
#其他渠道包最终打包流程
def FinalBuild_Other():
    buildAB = os.environ.get('buildAB_type', 'no')
    #装载乐变
    if InstallLebian():
        if dic_replace_lbid.__contains__(pkg_type):
            EditLBID(LBID_PH, dic_replace_lbid[pkg_type])
    #call unity
    FinalCallUnity()
    #export
    if buildAB != "no":
        ExportApk.ExportAB()
    ExportApk.ExportApk(pkg_type)
        
###############################Main###################################
#version
ChangeVersion()

#确保AB下没有IOS资源 
RemoveABIOSByVersion()
    
#加载图，登陆图，logo图等替换
if pkg_type in list_replace_logo:
    shutil.copy(PATH_REPLACEABLERES_FROM_MHHX + '/Login_Brand.png', PATH_LOGIN_BRAND + '/Login_Brand.png')

#slhx专属
if pkg_type == JPARAM_PKG_TYPE_XKSLHX:
    OnlyXK()
    OnlySLHX()
    
#mkhx专属
if pkg_type == JPARAM_PKG_TYPE_XKMKHX:
    OnlyXK()
    OnlyMKHX()
    
#xk专属
if pkg_type in list_replace_xk:
    OnlyXK()
    
#ast专属
if pkg_type == JPARAM_PKG_TYPE_ASTGOOGLE:
    OnlyAST()
    
#查看是否热更代码合入主工程
useIL2CPP = os.environ.get('use_il2cpp', 'false')
if useIL2CPP == 'true':
    HandleMergeHotfixIntoMain()

#Last Call Unity
if onlyAB == "true":
    FinalBuild_OnlyAB()
    ExportApk.ExportAB()
else:
    HandleCopyBugly() 
    if pkg_type == JPARAM_PKG_TYPE_XKFX:
        FinalBuild_XKFX()
    else:
        FinalBuild_Other()