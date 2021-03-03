@echo off

SET GM=%cd%
SET GMPATH="%GM%/../../Source"

REM make suer Sites dir exists
if not exist "C:/Users/Public/Sites" (
	mkdir "C:/Users/Public/Sites"
) else (
	echo Folder already exists
)	

REM first kill running http-server
echo Launch http-server
cd "C:/Users/Public/Sites"

REM restart http-server each time
tasklist /FI "IMAGENAME eq node.exe" 2>NUL | find /I /N "node.exe">NUL
if "%ERRORLEVEL%"=="0" ( 
	taskkill /F /IM node.exe
	start /min http-server -p 9090
) else (
	start /min http-server -p 9090
)

REM Launch Unity to do the build and push bundles
tasklist /FI "IMAGENAME eq Unity.exe" 2>NUL | find /I /N "Unity.exe">NUL
if "%ERRORLEVEL%"=="0" ( 
	echo =====================================================
	echo = NOTE: Please first save your work and close Unity =
	echo =====================================================
	pause
) else (
	echo "Assts bundles are building, please be patient!"
	"C:\Program Files (x86)\Unity\Editor\Unity.exe" -projectPath "%GMPATH%" -batchmode -executeMethod BuildHelper.BuildForDeploy -quit
	echo NOTE: Build and deploy bundle is success!
	echo       Bundle location is: c:/Users/Public/Sites       
	pause
)
