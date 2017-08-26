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
In this order, these projects should immediately build

Open Development\KeePassPasswordChanger\KeePass Password Changer.sln
