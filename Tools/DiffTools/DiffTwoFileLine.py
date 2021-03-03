#-*- coding: UTF-8 -*-
#输出两个文件不同的行

import argparse
import os,re
import datetime

OLD_FILE 	= r'.'
NEW_FILE    = r'.'
RESULT_Path = os.path.dirname(__file__) + '\\' + r'DiffTwoFileLineOutput'

list_src = []
list_diff = []


parser = argparse.ArgumentParser()
parser.add_argument("-o","--old_file", default="", help="old file") 
parser.add_argument("-n","--new_file", default="", help="new file") 



def _collectOneFileLines(filePath):
	global list_src
	theFile = open(filePath, "r+",encoding = 'UTF-8')
	for s in theFile.readlines():
		list_src.append(s)


def _compareAndCollectDiff(filePath):
	global list_src
	global list_diff
	theFile = open(filePath, "r+",encoding = 'UTF-8')
	for s in theFile.readlines():
		if s not in list_src:
		   list_diff.append(s)



def _outputResult(filePath):
	with open(filePath, 'w',encoding = 'UTF-8') as file_object:
		for s in list_diff:
			file_object.writelines(s)


def foo():
	args = parser.parse_args()
	if args.old_file == "" or args.new_file == "":
		print("对比的两个文件没有正确给定")
	else:
		_collectOneFileLines(args.old_file)
		_compareAndCollectDiff(args.new_file)
		now_time = datetime.datetime.now().strftime('%Y_%m_%d_%H_%M_%S')

		#Output
		if not os.path.exists(RESULT_Path):
		   os.makedirs(RESULT_Path)
     
		outputFile = RESULT_Path + r'\\' + now_time + r'.txt'
		_outputResult(outputFile)
		print("================================")
		print("Finish!")
		print("Author: Johny")
		print("================================")


if __name__=="__main__":
    foo()