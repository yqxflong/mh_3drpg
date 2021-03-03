# -*- coding: utf-8 -*- 
import sys, os, time, shutil, re, json, inspect
NO = 0
ERROR = 1
DEBUG = 2
INFO = 3

COLOR_HEADER = '\033[95m'
COLOR_OKBLUE = '\033[94m'
COLOR_OKGREEN = '\033[92m'
COLOR_WARNING = '\033[93m'
COLOR_FAIL = '\033[91m'
COLOR_ENDC = '\033[0m'

class Cls():
    # ctor
    def __init__(self):
        # 0 = no
        # 1 = error
        # 2 = debug
        # 3 = info
        self.level = 1
    def log(self,t,msg,__name,__COLOR_HEADER):
        if isinstance(msg,(int,long,float,bool,str,basestring,complex) ):
            msg = '    {}'.format(msg)
        elif not msg:
            msg = 'None'
        elif isinstance(msg,(tuple,list,dict,set) ):
            msg = json.dumps(msg,indent=4,separators=(',',':'),encoding= sys.getfilesystemencoding())
        else:
            try:
                msg = json.dumps(msg.__dict__,indent=4,separators=(',',':'),encoding= sys.getfilesystemencoding())
            except:
                _dict = {}
                for k in msg.__dict__:
                    o = msg.__dict__[k]
                    if isinstance(o,(int,long,float,bool,str,basestring,complex,tuple,list,dict,set)):
                        _dict[k] = o
                self.log(t,_dict,__name,__COLOR_HEADER)
                return
                            
        print "{}[{}][{}][{}]=>\n{}{}".format(
                    __COLOR_HEADER,
                    t,
                    time.strftime('%Y-%m-%d %X',time.localtime(time.time())),
                    __name,
                    msg,
                    COLOR_ENDC 
                )
obj = Cls()
def d(msg):
    if obj.level >= DEBUG:
        frame = inspect.currentframe()
        frame = inspect.getouterframes(frame)[1]
        string = inspect.getframeinfo(frame[0]).code_context[0].strip()
        args = string[string.find('(')+1:-1].split(',')
        name = args[0]
        obj.log('debug',msg,name,COLOR_OKGREEN)
# print 
def e(msg):
    if obj.level >= ERROR:
        frame = inspect.currentframe()
        frame = inspect.getouterframes(frame)[1]
        string = inspect.getframeinfo(frame[0]).code_context[0].strip()
        args = string[string.find('(')+1:-1].split(',')
        name = args[0]
        obj.log('error',msg,name,COLOR_FAIL)
# print info msg
def i(msg):
    if obj.level >= INFO:
        frame = inspect.currentframe()
        frame = inspect.getouterframes(frame)[1]
        string = inspect.getframeinfo(frame[0]).code_context[0].strip()
        args = string[string.find('(')+1:-1].split(',')
        name = args[0]
        obj.log('info',msg,name,COLOR_OKBLUE)

# print warning msg
def e(msg):
    if obj.level >= ERROR:
        frame = inspect.currentframe()
        frame = inspect.getouterframes(frame)[1]
        string = inspect.getframeinfo(frame[0]).code_context[0].strip()
        args = string[string.find('(')+1:-1].split(',')
        name = args[0]
        obj.log('error',msg,name,COLOR_WARNING)


# test
if __name__ == "__main__":
    d( (1,2,3,) )
    c = Cls()
    c.a = 1
    c.b = 3
    d(c)
    d("123")
