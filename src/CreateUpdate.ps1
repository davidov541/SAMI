MSBuild SAMI.sln /p:Configuration=Debug /t:Clean /v:m
MSBuild SAMI.sln /p:Configuration=Debug /v:m

pushd UpdateCreator
.\SAMIUpdateCreator.exe
popd