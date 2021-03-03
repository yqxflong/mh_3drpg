# -*- coding: utf-8 -*- 
import sys, os, time, shutil, re, subprocess, thread
import Log
import hashlib
import uuid

MB_PRECISION = 1024 * 1024
KB_PRECISION = 1024 


def get_mac_address_string():
    mac_num = hex(uuid.getnode()).replace('0x','')
    mac_num = mac_num.zfill(12)
    mac = ':'.join(mac_num[i:i+2] for i in range(0,11,2))
    return mac


def GetSizeString( size ):
    if size >= MB_PRECISION :
        return "{:.2f}MB".format( float(size) / float(MB_PRECISION))
    elif size >= KB_PRECISION :
        return "{:.2f}KB".format( float(size) / float(KB_PRECISION))
    return "{:.2f}B".format(float(size))

def getTimeString():
    return time.strftime("%Y%m%d_%H%M%S",time.localtime(time.time()))

def Assert(condition,msg,errCode = -1):
    if condition:
        Log.e(msg)
        Log.e("errCode:{}".format(errCode))
        raise Exception(errCode)

def Warning(condition,msg):
    if condition:
        Log.e("[warning]:{}".format(msg))


    
def tsplit(srcStr,*delimiters):
    pattern = '|'.join(map(re.escape,delimiters))
    return re.split(pattern,srcStr)



def runCmd(cmd,callback=None,out=sys.stdout,err=sys.stderr,stdin=sys.stdin,shell=False,cwd=None):
    Log.i("[run][start][{}]".format(cmd))
    returncode = None
    if isinstance(cmd,(str,basestring)):
        cmd = cmd.split(' ')
    out_pipe = False
    err_pipe = False
    r_out = None
    r_err = None
    if out == True:
        r_out = subprocess.PIPE
        out_pipe = True
    elif out == False:
        r_out = sys.stdout
        out_pipe = False
    else:
        r_out = out
        out_pipe = False
    out = ""
    if err == True:
        r_err = subprocess.PIPE
        err_pipe = True
    elif err == False :
        r_err = sys.stderr
        err_pipe = False
    else:
        r_err = err
        err_pipe = False
    err = ""
    if shell == True:
        cmd = ' '.join(cmd)
        Log.d(cmd)

    if cmd != None and len(cmd) > 0:
        print(cmd)
        p = None
        if cwd != None:
            p = subprocess.Popen(cmd, stderr=r_err,stdout=r_out,stdin=stdin,shell=shell,cwd=cwd)
        else:
            p = subprocess.Popen(cmd, stderr=r_err,stdout=r_out,stdin=stdin,shell=shell)
        times = 0
        while(returncode == None):
            _o,_e = p.communicate()
            if err_pipe and _e:
                err = err + _e

            if out_pipe and _o:
                out = out + _o
            returncode = p.poll()
    Log.i("[run][finish][{}]".format(cmd))
    if callback != None:
        callback()

    return returncode,out,err

def writFile(file_name,content):
    makeFileReady(file_name)
    open(file_name,'w').write(content)

def makeDirReady(path,mustClean=True):
    if os.path.exists(path):
        if mustClean == True:
            rmPath(path)
            os.makedirs(path)
            os.chmod(path,0777)
    else:
        os.makedirs(path)
        os.chmod(path,0777)

def linkPath(src,dst):
    Log.e('link')
    Log.e(src)
    Log.e(dst)
    Assert( src == None,"src is null")
    Assert( not os.path.exists(src),"path not exists" + src)
    if os.path.exists(dst):
        rmPath(dst)
    os.symlink(src,dst)

def copyPath(src,dst,ignoreExtList=None): 
    if not os.path.exists(src):
        Log.i("src des not exist: " + src)
        return
    if os.path.islink(dst):
        rmPath(dst)
    if os.path.isdir(src):
        if not os.path.exists(dst):
            os.mkdir(dst)
        for item in os.listdir(src):
            s = os.path.join(src,item)
            d = os.path.join(dst,item)
            copyPath(s,d,ignoreExtList)
    else:
        if ignoreExtList != None:
             _,ext = os.path.splitext(src)
             if ext in ignoreExtList:
                #Log.i("Ignore copy pattern:" + ext)
                return
        src = src.replace("\\","/")
        dst = dst.replace("\\","/")
        shutil.copy2(src,dst)
        #Log.e("Copy {0} to {1} Completed!".format(src,dst))

def rmPath(path):
    Log.i("rmPath"+path)
    Warning(not os.path.exists(path),"no file need to remove"+path)
    if os.path.exists(path):
        if os.path.islink(path) or os.path.isfile(path):
            os.remove(path)
        elif os.path.isdir(path):
            shutil.rmtree(path)

def clearPath(path,ignoreExtList=None):
    if not os.path.exists(path):
        Log.i("clearPath des not exist: " + path)
        return
    # remove ln
    if os.path.islink(path):
        rmPath(path)
        return   
    if os.path.isdir(path):
        for item in os.listdir(path):
            p = os.path.join(path,item)
            clearPath(p,ignoreExtList)
    elif os.path.isfile(path):
        if ignoreExtList != None:
             _,ext = os.path.splitext(path)
             if ext in ignoreExtList:
                #Log.i("Ignore clear pattern:" + ext)
                return
        os.remove(path)
    else:
        Log.e("Unknow path:" + path)
    pass

def renamePath(src,dst):
    Assert( not os.path.exists(src),"can't rename path because is not exists:"+src)
    shutil.move(src,dst)

def makeFileReady(path,backUp=True):
    if os.path.exists(path):
        if backUp == True:
            shutil.move(path,path + getTimeString())
        else:
            rmPath(path)
    else:
        # make sure the path is exists
        makeDirReady(os.path.dirname(path),False)
    open(path,'a').close()

tail_ids = []

def __refresh__file__(fp,tid):
    fp.seek(0,2)
    while True:
        line = fp.readline()
        if not line:
            if tid not  in tail_ids:
                thread.exit_thread()
            time.sleep(0.1)
            continue
        yield line

def __thread__tailf__(filename,tid):
    if os.path.exists(filename):
        fp = open(filename,'r')
        for line in __refresh__file__(fp,tid):
            print line.replace('\n','')

def tailf(filename):
    __tid = 0
    if len(tail_ids) > 0:
        __tid = tail_ids[len(tail_ids) - 1] + 1
    tail_ids.append(__tid)
    thread.start_new_thread(__thread__tailf__,(filename,__tid))
    return __tid

def finish_tailf(tid):
    try:
        tail_ids.remove(tid)
    except:
        Log.e("finish a unexist tailf tid")

def getCOPathName():
    self_path = os.path.dirname(
                os.path.abspath(__file__)
            )
    idx_proj = self_path.rindex("Tools") - 1
    idx_co_name_start = self_path.rindex('/',0,idx_proj) + 1
    co_path_name = self_path[idx_co_name_start:idx_proj]
    return co_path_name

def find_self_pid(appName):
    res = None
    try:
        res =  subprocess.check_output("ps -ef | grep " + getCOPathName() + " | grep " + appName  + " | grep -v grep| awk '{print $2}' ",shell=True)
        #  res =  subprocess.check_output("ps -ef | grep EOA_NewBuild | grep Unity  | grep -v grep",shell=True)
    except:
        res = None
    res = res.replace('\n','')
    Log.d(res)
    return res

def kill(pid):
    Log.d(pid)
    if pid:
        cmd = [
                'kill',
                '-9',
                pid
            ]
        runCmd(cmd)
def PlatformIsMacOs():
    return sys.platform == "darwin"

def PlatformIsWin32():
    return sys.platform == "win32"

def setTarget(target):
    global build_target
    build_target = target

def getTarget():
    global build_target
    return build_target

def getUnZipCmd():
    if sys.platform == "darwin" or sys.platform.startswith('linux'):
        return ["unzip",'-o']
    elif sys.platform == "win32":
        run_path = os.path.dirname(os.path.realpath(__file__))
        _7z_path = os.path.join(run_path,"../../7Zip/7z.exe")
        return [_7z_path,"x"]

def getZipCmd():
    print  sys.platform 
    if sys.platform == "darwin" or sys.platform.startswith('linux'):
        return ["zip"]
    elif sys.platform == "win32":
        run_path = os.path.dirname(os.path.realpath(__file__))
        _7z_path = os.path.join(run_path,"../../7Zip/7z.exe")
        return [_7z_path,"a"]

# unzip a file to a dir with diffrent application in diffrent platform
# @param src zip file need to unzip
# @param dst target dir to unzip
def unzip(src,dst):
    cmd = getUnZipCmd()
    if sys.platform == "darwin":
        cmd2 = [
                src,
                '-d',
                dst
            ]
    elif sys.platform == "win32":
        cmd2 = [
                src,
                '-o'+
                dst
            ]
    cmd = cmd + cmd2
    runCmd(cmd)

# zip a dir or file to a dir with diffrent application in diffrent platform
# @param src dir need to uip
# @param dst target dir to zip
def zip(src,dst):
    cmd = getZipCmd() + [
            dst,
            src,
            "-r"
        ]
    runCmd(cmd)
# add some file to a exist archive file
# @param target the exist archive file
# @param src a path need to add into the archive file
def zipa(target,src,cwd):
    cmd = getZipCmd() + [
            "-r",
            target,
            src
        ]
    runCmd(cmd,cwd=cwd)

# delete some file from a exist archive file
# @param target the exist archive file
# @param src the file need to delete if not in archive ignore
# @cwd run base path
def zipd(target,src,cwd):
    cmd = getZipCmd() + [
            "-d",
            target,
            src
        ]
    runCmd(cmd,cwd=cwd)

def zipv(target):
    cmd = getUnZipCmd() + [
            "-v",
            target
        ]
    return runCmd(cmd,out=True,err=True)

def zipax(target,src,xsurfix,cwd):
    cmd = getZipCmd() + [
            "-r",
            target,
            src,
            "-n",
            xsurfix
        ]
    runCmd(cmd,cwd=cwd,shell=True)

# copy from Unity4.7.2f1 source code UnityPlayer.java getMD5HashOfEOCD
def getMD5HashOfEOCD(filepath):
    maxEOCDsize = 22 + 64*1024
    info = os.stat(filepath)
    with open(filepath,'rb') as f:
        f.seek(info.st_size - min(info.st_size, maxEOCDsize), 0)
        md5obj = hashlib.md5()
        for block in iter(lambda: f.read(1024), b""):
            md5obj.update(block)
        hash = md5obj.hexdigest()
        #print(hash)
        return hash

if __name__ == "__main__":
    #  zip("D:/Work/HON_proj/Tools/7Zip","D:/xx.zip")
    #  unzip("D:/xx.zip","D:/xx_o")
    # copyPath('/work/demo/src','/work/demo/dst')
    #  kill(
            #  find_self_pid("Unity")    
        #  )
    #  src = "/work/crash/NGame_TW_GooglePlay_Publish_201611141632_beta02_Build0003_final_CHT_Garena_TW_1.12.1.1_r36276_OnlyFirstInstall.apk"
    #  cmd = ["ls", "-l", "/work"] 
     
    #  runCmd(cmd,out=open("/work/crash/tmp.log","w"))
    #  runCmd(cmd,out=True)
    #  runCmd(cmd,out=False)
    #  p.wait()
    #  dst = "/work/crash/out"
    #  unzip(src,dst)
    #  runCmd("svn cleanup /work/J4/EOA_trunk")
    #  runCmd("svn update /work/J4/EOA_trunk")
    #  tailf('/tmp/adb.log')
    #  runCmd("tree -f /tmp")
    #  stop=True

    # copyPath('/work/cmdwin32/Emmagee/','/tmp/')
    # linkPath('/work/cmdwin32/Emmagee','/work/XX/XX')
    zipa("/Volumes/EXT/ieg_ci/workspace/6369/Project/bin/main.107471.com.ngame.allstar.obb","*","/Volumes/EXT/ieg_ci/workspace/6369/Project/TargetAndroid/ADT_PROJECT/__obb_tmp__")
    #  zipd("game_resources_android1.17.1.1_r101667.zip","test.env","/mnt/j/Work/EOA_confirm/Tools/BuildScript/commonPy/example")
    #  c,o,e = zipv("/mnt/j/Work/EOA_confirm/Tools/BuildScript/commonPy/example/game_resources_android1.17.1.1_r101667.zip",)
    #  print c
    #  print o
    #  print e
    zipax("/Volumes/EXT/ieg_ci/workspace/6369/Project/bin/main.107471.com.ngame.allstar.obb","*",".png:.wem:.bnk","/Volumes/EXT/ieg_ci/workspace/6369/Project/TargetAndroid/ADT_PROJECT/__obb_tmp__")

