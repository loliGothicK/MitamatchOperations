@echo on&setlocal
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
    set DummyPrivateKey=##GOOGLE_CLOUD_PRIVATE_KEY##
    set PrivateKey=%GOOGLE_CLOUD_PRIVATE_KEY%

    ::Replacement statement
    if NOT "%PrivateKey%" == "" (
    
        if "%buildType%" == "PostBuild" (
        powershell -Command "(gc %sourceFile%) -Replace '%PrivateKey%','%DummyPrivateKey%'|SC %sourceFile%"
        )
        if "%buildType%" == "PreBuild" (
        powershell -Command "(gc %sourceFile%) -Replace '%DummyPrivateKey%','%PrivateKey%'|SC %sourceFile%"
        )
        powershell -Command "cat %sourceFile%"
    )
