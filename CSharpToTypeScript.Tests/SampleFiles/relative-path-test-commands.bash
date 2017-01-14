cd /c/GitHub/CSharpToTypeScript/CSharpToTypeScript.Tests; \
dotnet test; \
cd /c/GitHub/CSharpToTypeScript/CSharpToTypeScript; \
dotnet run -- -c ../CSharpToTypeScript.Tests/SampleFiles/sample.debug.cfg.json; \
cd /c/GitHub/CSharpToTypeScript; \
dotnet test CSharpToTypeScript.Tests/project.json; \
dotnet run -p CSharpToTypeScript -- -c CSharpToTypeScript.Tests/SampleFiles/sample.debug.cfg.json