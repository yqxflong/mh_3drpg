KEYPATH="E:/Project/lt_trunk/client_gam/Source/Assets/Editor/Android/key.keystore"
APKPATH="C:/Users/Administrator/Downloads/2020_11_18_15_38_01_xkslhx_release_1.0.1.3669.apk"


jarsigner -verbose -sigalg MD5withRSA -digestalg SHA1 -keystore $KEYPATH  -storepass xinyou -keypass xinyou $APKPATH xinyou