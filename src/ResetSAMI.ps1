param (
    [String]$path="SAMIConsole/bin/Debug",
    [String]$cred="mcginnda@hotmail.com",
    [Switch]$showoutput)

Get-PSSession | foreach-object { Remove-PSSession $_ }

$s = New-PSSession RIEMANN -credential $cred

Invoke-Command $s {
    $proc = (Get-Process | where { $_.ProcessName -eq "SAMIConsole"})
    if($proc -ne $null)
    {
        $proc.Kill()
    }

    Get-Job | where { $_.State -eq "Completed" -and
    -not $_.HasMoreData } | Remove-Job
}

"Removing files"

rm -Force -Recurse \\RIEMANN\Code\SAMIBin

"Copying files"

cp -Recurse $path \\RIEMANN\Code\SAMIBin

"Starting SAMI"

if($showoutput)
{
   Invoke-Command $s {
        cd E:/Code/SAMIBin
        ./SAMIConsole.exe
    }
}
else 
{
    Invoke-Command -Session $s -AsJob {
        cd E:/Code/SAMIBin
        ./SAMIConsole.exe
    }    
}

#powershell.exe $(SolutionDir)\ResetSAMI.ps1 $(TargetDir)*