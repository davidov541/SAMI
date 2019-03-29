dim filesys

Const APPLICATION_DATA = &H1a&

Set objShell = CreateObject("Shell.Application")
Set objFolder = objShell.Namespace(APPLICATION_DATA)
Set objFolderItem = objFolder.Self

set filesys=CreateObject("Scripting.FileSystemObject")
If filesys.FileExists(objFolderItem.Path & "\TempSAMI\ConsoleConfiguration.xml") Then
filesys.CopyFile objFolderItem.Path & "\TempSAMI\ConsoleConfiguration.xml", objFolderItem.Path & "\SAMI\"
filesys.CopyFile objFolderItem.Path & "\TempSAMI\KinectConfiguration.xml", objFolderItem.Path & "\SAMI\"
fileSys.DeleteFolder objFolderItem.Path & "\TempSAMI"
End If