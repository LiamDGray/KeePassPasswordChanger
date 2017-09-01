# keepass-password-changer
A plugin for KeePass v2 to change supported password entries automatically

**Notice** - Dev docs are unavailable at the moment. Template instructions will follow! 

# Start - HowTo use
Select branch master, go to [Build/KeePass-Password-Changer](https://github.com/phi-el/KeePassPasswordChanger/tree/master/Build/KeePass-Password-Changer) and download the latest zip version of the plugin. The current version and the sha512 hash value of the zip file can be located in the README.md of the containing directory.   
Unzip the content of the zip folder into the KeePass v2 Directory. It's recommended to have another Directory with the KeePass executable for the plugin, because the plugin contains too many files at the moment and deleting the plugin will be horrible afterwards ;)
Release-Zip naming convention: KeePassPasswordChanger vX.X.X.X.zip

**Important:** Let CefBrowser communicate through your firewall BEFORE you start a browser action. The best solution therefor is to execute it after you extracted the plugin. If the **DuckDuckgo.com** site shows up, you are ready to go :)

**Notice(s)**
 - Backup your DB
 - Logout from active web-sessions (when they are currently opened in your browser)
 - when a lot of changes fail, this may be due to low internet bandwidth, low cpu power or other cpu-consuming applications (chrome and netflix are evil, believe me ^^)
 - The bigger your password DB is, the longer will it take so obay the following line:
 - **DO NOT CLOSE THE APPLICATION WHEN PW CHANGES ARE RUNNING; YOU WILL CORRUPT YOUR DATABASE!**


# Rebuild it on your own :)

## Known problems (Besides the requirements below)
 - When building the KeePassPasswordChanger solution, a CSC Error 'Metadata file .dll could not be found' occurs. Most of the time it is the 'CSharpTest.Net.RpcLibrary.dll'. This can happen is you restart your computer. Remove the 'CSharpTest.Net.RpcLibrary.dll'(or the missing DLL) from the Extension project(like RPC-Communication) and re-add it again, this should solve the problem and you should be able to rebuild the project, regardless if the branch is 'DEV' or 'MASTER'. 

## Branches
 - Master: When using master branch, you get the plugin which can be used with KeePass v2
 - Dev: When using this, you get the plugin which can be used for development

## HowTo Clone
Clone keepass-password-changer
Run following commands(keePass2 will fail, its ok)
```
git submodule foreach --recursive git checkout master  
git checkout 'PasswordManagementLibrary'
```
## HowTo Dev/Build
Recover the NUGET Packets:
 - **RPC-Communicaton**: Open Development\rpc-communication-net-2\rpc-communication-net2.sln
 - **CefBrowser**: Open Development\CefBrowser\CefBrowser.sln
 - **KeePass Password Changer**: Open Development\KeePassPasswordChanger\KeePass Password Changer.sln
In this order, these projects should immediately build


### Release
Select branch **master**, the project should immediately build(after you have recovered the NUGET packets).
Ensure that the pre-build actions look like this:
```
rmdir "$(ProjectPath)\..\..\..\..\Build\KeePass-Password-Changer\." /S /Q
rmdir $(TargetDir) /S /Q
REM xcopy "$(ProjectDir)..\..\CefBrowser\CefBrowser\bin\x86\Debug" "$(TargetDir)" /Y /E
xcopy "$(ProjectDir)..\..\CefBrowser\CefBrowser\bin\x86\Release" "$(TargetDir)" /Y /E
```
and the post-build actions look like this:
```
REM echo V | xcopy "$(TargetDir)." "$(ProjectPath)\..\..\..\KeePass2.x\Build\KeePass\Debug\." /Y /E
```
When this does not work try:
You have to remove the KeePass reference in the project **KeePass Password Changer** and add the KeePass executable from the Director **\Build\KeePass-2.36**, Then right click on **KeePass Password Changer** and click on **rebuild new**

### Develop
Select branch **dev**, the project should immediately build(after you have recovered the NUGET packets).

When this does not work try:
You have to remove the KeePass reference in the project **KeePass Password Changer** and add the KeePass executable from the Director **\Development\KeePass2.x\Build\KeePass\Debug**, Then right click on **KeePass Password Changer** and click on **rebuild new**
