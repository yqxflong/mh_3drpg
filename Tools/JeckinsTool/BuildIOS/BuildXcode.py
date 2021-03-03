# -*- coding:utf8 -*-
#使用Xcode打包IPA

import os
import time
import shutil
import subprocess

##环境参数
NODE = os.environ.get('NODE', 'master')
LTSITES_PATH = os.environ.get('LTSITES', 'UNKNOWN')
JENKINS_WORKSPACE = os.environ.get('JENKINS_WORKSPACE', 'UNKNOWN')
IOS_BUILD_PATH = "/Volumes/DataDisk/LT_IOS/client_gam/Build/Client/Temp/Client-iOS"
XCODE_PATH = IOS_BUILD_PATH + "/XcodeProject"
CODE_SIGN_IDENTITY = '"Apple Development: tang baobiao (6PF59CK3XQ)"'
PROVISIONING_PROFILE = '"65a56cc4-ba5b-49fe-9fe5-90dfa8214b33"'
FILE_EXPORTOPTIONS = "ExportOptions.plist"


#from jenkins
jdebug = os.getenv('Debug')
#######################################Main###############################
def BuildIPA():
    archivePath = IOS_BUILD_PATH + '/mhhx_dev.xcarchive'
    if jdebug == "false":
        archivePath = IOS_BUILD_PATH + '/mhhx_release.xcarchive'

    print("正在构建Archive......")
    cdCommand = "cd " + XCODE_PATH
    buildComand = 'xcodebuild \
                archive \
                -project Unity-iPhone.xcodeproj \
                -scheme Unity-iPhone \
                -configuration Release \
                -archivePath ' + archivePath + ' \
                clean archive \
                -quiet \
                PROVISIONING_PROFILE=' + PROVISIONING_PROFILE  + '\
                CODE_SIGN_IDENTITY=' + CODE_SIGN_IDENTITY +'  || exit'
    print(buildComand)
    ret = subprocess.call(cdCommand + " && " +  buildComand, shell=True)
    if ret != 0:
        print("构建IPA失败(archive)，退出构建!!!")
        exit()

    print("正在构建IPA......")    
    archiveComand = 'xcodebuild \
                    -exportArchive \
                    -archivePath ' + archivePath + ' \
                    -exportPath ' + IOS_BUILD_PATH + ' \
                    -exportOptionsPlist ' + FILE_EXPORTOPTIONS + ' \
                    -allowProvisioningUpdates -allowProvisioningDeviceRegistration \
                    -quiet || exit'        
    print(archiveComand)
    ret = subprocess.call(archiveComand, shell=True)
    if ret != 0:
        print("构建IPA失败，退出构建!!!")
        exit()