#-*- coding: UTF-8 -*-
#By Johny

import sys, os, re
import shutil

#Hook位置
PATH_GIT_HOOK = r'../../../.git/hooks'
#文件名
FILE_PRE_COMMIT = r'pre-commit'

shutil.copy(FILE_PRE_COMMIT, PATH_GIT_HOOK + '/' + FILE_PRE_COMMIT)

print("Finish Install pre-commit!!! Press Any Key to Close ......")
sys.stdin.readline()
