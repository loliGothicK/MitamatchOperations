@echo on&setlocal
echo "log01"
    setlocal enableextensions disabledelayedexpansion
 
    ::Arguments of either PreBuild or PostBuild
    set buildType=%1
    ::License key replacement file
    set sourceFile=%2
     
    ::Replacement string
    set DummyKey=##SyncfusionLicense##
    set LicenseKey=%SyncfusionLicenseKey%
         
    ::Replacement statement
    if NOT "%LicenseKey%" == "" (
    
        if "%buildType%" == "PostBuild" (
        powershell -Command "(gc %sourceFile%) -Replace '%LicenseKey%','%DummyKey%'|SC %sourceFile%"
        )
        if "%buildType%" == "PreBuild" (
        powershell -Command "(gc %sourceFile%) -Replace '%DummyKey%','%LicenseKey%'|SC %sourceFile%"
        )
        powershell -Command "cat %sourceFile%"
    )

    ::Replacement string
    set DummyCREDENCIALS=##GOOGLE_CLOUD_CREDENCIALS##
    set CREDENCIALS=%GOOGLE_CLOUD_CREDENCIALS%
         
    ::Replacement statement
    if NOT "%CREDENCIALS%" == "" (
    
        if "%buildType%" == "PostBuild" (
        powershell -Command "(gc %sourceFile%) -Replace '%CREDENCIALS%','%DummyCREDENCIALS%'|SC %sourceFile%"
        )
        if "%buildType%" == "PreBuild" (
        powershell -Command "(gc %sourceFile%) -Replace '%DummyCREDENCIALS%','%CREDENCIALS%'|SC %sourceFile%"
        )
        powershell -Command "cat %sourceFile%"
    )
