dim filesys

Const APPLICATION_DATA = &H1a&

Set objShell = CreateObject("Shell.Application")
Set objFolder = objShell.Namespace(APPLICATION_DATA)
Set objFolderItem = objFolder.Self

set filesys=CreateObject("Scripting.FileSystemObject")

If filesys.FolderExists(objFolderItem.Path & "\TempSAMI") Then
fileSys.DeleteFolder objFolderItem.Path & "\TempSAMI"
End If
If filesys.FileExists(objFolderItem.Path & "\SAMI\ConsoleConfiguration.xml") Then
fileSys.CreateFolder objFolderItem.Path & "\TempSAMI"
filesys.CopyFile objFolderItem.Path & "\SAMI\ConsoleConfiguration.xml", objFolderItem.Path & "\TempSAMI\"
filesys.CopyFile objFolderItem.Path & "\SAMI\KinectConfiguration.xml", objFolderItem.Path & "\TempSAMI\"
End If