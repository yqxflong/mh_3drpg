# -*- coding:utf8 -*-
#将项目配置中的debug宏赋值给release宏

import os
import time
import shutil
import argparse
import xml.etree.ElementTree as ET
from xml.etree.ElementTree import Element

PROJECTPATH_MAC_ANDROID = "/Volumes/DataDisk/LT_Android/client_gam/Source"
PROJECTPATH_MAC_IOS = "/Volumes/DataDisk/LT_IOS/client_gam/Source"
PROJECTPATH_WIN = "E:/Project/lt_unity184_hotfix/client_gam/Source"


def ChangeProjectToRelease(path):
    ET.register_namespace('','http://schemas.microsoft.com/developer/msbuild/2003')
    tree = ET.parse(path)
    root = tree.getroot()
    #取得debug的宏
    debugCons = ''
    for node in list(root):
        for key in node.attrib.keys():
            if key == 'Condition' and 'Debug' in node.attrib[key]:
                for child in list(node):
                    if 'DefineConstants' in child.tag:
                        debugCons = child.text
    debugCons = debugCons.replace('DEBUG;', '')
    debugCons = debugCons.replace('TRACE;', '')
    #修改release宏
    for node in list(root):
        for key in node.attrib.keys():
            if key == 'Condition' and 'Release' in node.attrib[key]:
                releaseConsNode = Element('DefineConstants')
                releaseConsNode.text = debugCons
                node.append(releaseConsNode)
                    
    #save
    tree.write(path, encoding='utf-8', xml_declaration=True)
    
                
    
############################Main###################################
#读取输入
parser = argparse.ArgumentParser("读取Jeckins传参")
parser.add_argument('-t', '--type', default="unknown")
args = parser.parse_args()
#确定编译平台
projectPath = PROJECTPATH_WIN
if args.type == 'ios':
    projectPath = PROJECTPATH_MAC_IOS
elif args.type == 'android':
    projectPath = PROJECTPATH_MAC_ANDROID
else:
    print("ERROR: 不支持传入的平台: " + args.type)
    exit()
#impl    
ChangeProjectToRelease(projectPath + '/Unity_Hotfix.csproj')
ChangeProjectToRelease(projectPath + '/Unity_Main.csproj')
ChangeProjectToRelease(projectPath + '/Unity_ThirdParty.csproj')
ChangeProjectToRelease(projectPath + '/Utils_Editor.csproj')
ChangeProjectToRelease(projectPath + '/EBScripts_Editor.csproj')