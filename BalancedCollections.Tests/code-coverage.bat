@echo off
OpenCover.Console.exe -target:"C:\Program Files (x86)\NUnit.org\nunit-console\nunit3-console.exe" -targetargs:"--labels=All bin\Debug\BalancedCollections.Tests.dll" -register:user -filter:"+[*]BalancedCollections* -[*]NUnit*"
"C:\Program Files (x86)\ReportGenerator\net47\ReportGenerator.exe" -reports:results.xml -targetdir:coveragereport