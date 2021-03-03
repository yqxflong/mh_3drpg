# -*- coding:utf8 -*-
#用于多语言，将繁中都改成简中
#Johny

import os, shutil

PATH_MYPROJ = '../../Source'
PATH_LANG = PATH_MYPROJ + '/Assets/Bundles/Languages'
PATH_RANDOMNAME = PATH_MYPROJ + '/Assets/Bundles/Name'
PATH_ATLAS = PATH_MYPROJ + '/Assets/_GameAssets/Res/Textures/Language/Atlas'
PATH_TEX = PATH_MYPROJ + '/Assets/_GameAssets/Res/Textures/Language/Textures'

def LangOrRandomName_TW2ZH(path):
    ll = os.listdir(path)
    for i in range(0, len(ll)):
        file = ll[i]
        if r'-CN' in file:
            newFileName = file.replace(r'-CN', r'-TW')
            path_tw = os.path.join(path, newFileName)
            if os.path.exists(path_tw):
                os.remove(path_tw)
                path_cn = os.path.join(path, file)
                shutil.copy(path_cn, path_tw)


def AtalsOrTex_TW2ZH(path):
    ll = os.listdir(path)
    for i in range(0, len(ll)):
        file = ll[i]
        if r'_CN' in file:
            newFileName = file.replace(r'_CN', r'_TW')
            path_tw = os.path.join(path, newFileName)
            if os.path.exists(path_tw):
                os.remove(path_tw)
                path_cn = os.path.join(path, file)
                shutil.copy(path_cn, path_tw)
            
#main            
print("==============Begin TW2ZH================")
LangOrRandomName_TW2ZH(PATH_LANG)
LangOrRandomName_TW2ZH(PATH_RANDOMNAME)
AtalsOrTex_TW2ZH(PATH_ATLAS)
AtalsOrTex_TW2ZH(PATH_TEX)
print("==============Finished! by Johny================")