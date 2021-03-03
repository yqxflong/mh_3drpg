
import sys, os, subprocess
import shutil
import Utils
import SendMsg

reload(sys)
sys.setdefaultencoding('utf8')

tool_path = os.path.dirname(os.path.realpath(__file__))
basePath = os.path.join(tool_path, "..", "..", "Source\\")
HASERROR = 0
errorStr = unicode("", "utf-8")
errorNum = 0

def launchUnity(unityPath, projPath, cmd, outFile, shell=False, params = "" ):
	cmd = [
				unityPath,
				"-projectPath",
				projPath,
				"-batchmode",
				"-executeMethod",
				cmd,
				"-logFile",
				outFile,
				"-nographics",
				"-params",
				params,
				"-quit"
			]
	return Utils.runCmd(cmd,None,sys.stdout,sys.stderr,shell)

def writeLogErrorFile(LogContent, log, fo, startPos):
	global errorNum
	global errorStr
	if startPos < LogContent.__len__():
		hasStartIndex = LogContent.find(log, startPos)
		if (hasStartIndex != -1):
			startIndex = LogContent.index(log, startPos)
			hasEndIndex = LogContent.find("\n",startIndex)
			if (hasEndIndex != -1):
				endIndex = LogContent.index("\n",startIndex)
				errorlogStr = unicode(LogContent[startIndex - 1:endIndex], "utf-8")
				print "Error " + errorlogStr
				#print "startIndex:" + str(startIndex)
				#print "endIndex:" + str(endIndex)
				errorStr += "【错误信息：" + errorlogStr + "】"
				fo.write(errorlogStr + "\n")
				errorNum += 1
				if (errorNum >= 100):
					fo.close()
				else:
					writeLogErrorFile(LogContent, log, fo, endIndex + 1)
			else:
				fo.close()
		else:
			fo.close()
	else:
		fo.close()

def commonCheckFile(className, function, log):
	global HASERROR

	print "-------------------------Begin " + function + " -------------------"
	LogPath = os.path.join(convOutPutFolderRoot, function + ".txt")
	launchUnity(unityPath, unityProjPath, className + "." + function, LogPath)

	LogFile = open(LogPath, "r")
	if(LogFile != None):
		LogContent = LogFile.read()
		if ("Debug:LogError" in LogContent):
			print "Error Happen in " + function
			HASERROR = 1
			LogPath = os.path.join(convOutPutFolderRoot, log + ".txt")
			#移动相关的log到指定目录
			# 打开文件
			fo = open(LogPath, "w")
			writeLogErrorFile(LogContent, log, fo, 0);



# executions begin-----------------------------------------------------------------------------------------------------------------------------

##############删除进程
command_Untiy = "taskkill /F /IM Unity.exe"
os.system(command_Untiy)

##########################删除所有输出文件, 从新添加目录
print "-------------------------Clean Out Put Folder -------------------"
convOutPutFolderRoot = os.path.join(tool_path, "ConvertCheckUpdateOutPut")
Utils.makeDirReady(convOutPutFolderRoot,True)

print "convOutPutFolderRoot: " + convOutPutFolderRoot

print "-------------------------Begin Convert bundleConfig -------------------"

print "Launch Unity"
unityProjPath = basePath
print "unityProjPath : "  + unityProjPath
unityPath = os.getenv("UNITY_BIN_PATH")
print "Unity.exe Path : " + unityPath


commonCheckFile("ResCheckMenuItemEntry", "GUIChecker", "GUICheckerErrorOut:")
commonCheckFile("ResCheckMenuItemEntry", "PrefabMissMatChecker", "PrefabMissMatCheckerErrorOut:")
commonCheckFile("ResCheckMenuItemEntry", "PrefabsMissChecker", "PrefabsMissCheckerErrorOut:")

if (HASERROR == 1):
	# 发送消息告知
	# 如果更改了to_who需要关闭cmd重新来才能获取
	to_who='前端程序'
	msg="@所有人 资源检查到有错误：" + errorStr + "具体请查看输出文件"
	SendMsg.send_qq(to_who, msg)
	sys.exit(1)

sys.exit(0)