@echo off
:: Paths
set solutionFolder=NRG.CalendarFinder
set testDir=.test-results
set reportDir=%testDir%/report
:: Files
set coverageReport=%testDir%/coverage.cobertura.xml
set reportHtml=%reportDir%/index.html
set openReport=%testDir%/show-report.bat
@echo on

:: Recreate testfolder.
IF EXIST %testDir% (
    rmdir /s /q %testDir%
)

mkdir %testDir%

:: Navigate to solution.
cd %solutionFolder%

echo ===== START CLEAN =====
dotnet clean
echo ===== START BUILD =====
dotnet build
echo ===== START RESTORE =====
dotnet restore

:: Generate test coverage result.
echo ===== START TEST =====
dotnet-coverage collect ^
	"dotnet test --no-build --no-restore -m:1" ^
	-o ../%coverageReport% ^
	-f cobertura

:: Navigate back to test-dir
cd ..

:: Generate report from coverage result.
reportgenerator ^
	-reports:%coverageReport% ^
	-targetdir:%reportDir% ^
	-reporttypes:Html ^
	-assemblyfilters:-*Test*;-Xunit*;-Argon*

:: Generate link to report.
echo start %reportHtml% > %testDir%/show-report.bat

:: Open report html.
start %reportHtml%

:: Wait for debug purposes.
pause