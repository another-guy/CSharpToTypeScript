<#
.SYNOPSIS
  This is a helper function that runs a scriptblock and checks the PS variable $lastexitcode
  to see if an error occcured. If an error is detected then an exception is thrown.
  This function allows you to run command-line programs without having to
  explicitly check the $lastexitcode variable.
.EXAMPLE
  exec { svn info $repository_trunk } "Error executing SVN. Please verify SVN command-line client is installed"
#>
function Exec
{
    [CmdletBinding()]
    param(
        [Parameter(Position=0,Mandatory=1)][scriptblock]$cmd,
        [Parameter(Position=1,Mandatory=0)][string]$errorMessage = ($msgs.error_bad_command -f $cmd)
    )
    & $cmd
    if ($lastexitcode -ne 0) {
        throw ("Exec: " + $errorMessage)
    }
}

echo "============================ REMOVE OLD ARTIFACTS ==========================="
if(Test-Path .\artifacts) { Remove-Item .\artifacts -Force -Recurse }

echo "============================ RESTORE DEPENDENCIES ==========================="
exec { & dotnet restore }

echo "========================== BUILD TO RUN UNIT TESTS =========================="
exec { & dotnet test .\CSharpToTypeScript.Tests -c Release }

echo "============================= BUILD NUGET PACKAGE ==========================="
$tagOfHead = iex 'git tag -l --contains HEAD'
$prefixExpected = $tagOfHead + "-"

$coreProjectJsonVersion = Get-Content '.\CSharpToTypeScript.Core\project.json' | Out-String | ConvertFrom-Json | select -ExpandProperty version
$appProjectJsonVersion = Get-Content '.\CSharpToTypeScript\project.json' | Out-String | ConvertFrom-Json | select -ExpandProperty version

if ([string]::IsNullOrEmpty($tagOfHead)) {
  $revision = @{ $true = $env:APPVEYOR_BUILD_NUMBER; $false = 1 }[$env:APPVEYOR_BUILD_NUMBER -ne $NULL];
  $revision = "b{0:D5}" -f [convert]::ToInt32($revision, 10)
  
  exec { & dotnet pack .\CSharpToTypeScript.Core -c Release -o .\artifacts --version-suffix=$revision }
  exec { & dotnet pack .\CSharpToTypeScript -c Release -o .\artifacts --version-suffix=$revision }
} elseif ($coreProjectJsonVersion.StartsWith($prefixExpected,"CurrentCultureIgnoreCase") -and
	$appProjectJsonVersion.StartsWith($prefixExpected,"CurrentCultureIgnoreCase")) {
  exec { & dotnet pack .\CSharpToTypeScript.Core -c Release -o .\artifacts }
  exec { & dotnet pack .\CSharpToTypeScript -c Release -o .\artifacts }
} else {
  throw ("Target commit is marked with tag " + $tagOfHead + " which is not compatible with project version(s) retrieved from metadata: " + $coreProjectJsonVersion + " | " + $appProjectJsonVersion)
}

echo "=============================== BUILD COMPLETE! ============================="