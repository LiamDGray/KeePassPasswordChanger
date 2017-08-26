# keepass-password-changer
A plugin for KeePass v2 to change supported password entries automatically

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


### Build
Select branch **master**, the project should immediately build(after you have recovered the NUGET packets).

When this does not work try:
You have to remove the KeePass reference in the project **KeePass Password Changer** and add the KeePass executable from the Director **\Build\KeePass-2.36**, Then right click on **KeePass Password Changer** and click on **rebuild new**

### Develop
Select branch **dev**, the project should immediately build(after you have recovered the NUGET packets).

When this does not work try:
You have to remove the KeePass reference in the project **KeePass Password Changer** and add the KeePass executable from the Director **\Development\KeePass2.x\Build\KeePass\Debug**, Then right click on **KeePass Password Changer** and click on **rebuild new**
