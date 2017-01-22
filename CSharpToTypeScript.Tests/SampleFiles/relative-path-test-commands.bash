# These commands assume:
#    sample.debug.cfg.json uses /c/new/ as an output directory
#    /c/GitHub/ is the root for the solution directory

rm -rf /c/new/; \
cd /c/GitHub/CSharpToTypeScript/CSharpToTypeScript.Tests; \
dotnet test; \
cd /c/GitHub/CSharpToTypeScript/CSharpToTypeScript; \
dotnet run -- -c ../CSharpToTypeScript.Tests/TestFiles/sample.debug.cfg.json; \
cd /c/GitHub/CSharpToTypeScript; \
dotnet test CSharpToTypeScript.Tests/project.json; \
dotnet run -p CSharpToTypeScript -- -c CSharpToTypeScript.Tests/TestFiles/sample.debug.cfg.json