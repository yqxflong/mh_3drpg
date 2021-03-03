@echo off

REM set variable for project
SET GMTOOL=%cd%
SET GMPATH="%GMTOOL%/../Code/Client"

REM install node.js
start node-v0.10.28-x64.msi

Rem install http-server
cd "/Users/Public"
npm install http-server -g

REM create dir of Sites
if not exist "C:/Users/Public/Sites" (
	mkdir "C:/Users/Public/Sites"
) else (
	echo Folder already exists
)	

pause

