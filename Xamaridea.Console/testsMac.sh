#!/bin/bash

ProjectPath="/Users/bja/Workspaces/Xamarin/MobilityWeek/Droid/MobilityWeek.Droid.csproj"

AndroidStudioPath="/Applications/Android Studio.app"

AndroidSDKPath="/Users/bja/SDKs/android-sdk-macosx"

CustomTemplatPath="/Users/bja/Workspaces/Xamarin/Xamaridea/CustomTemplate"
CustomTemplatZipPath="/Users/bja/Workspaces/Xamarin/Xamaridea/Xamaridea.Core/AndroidProjectTemplate.zip"


#mono bin/Debug/Xamaridea.Console.exe -?

ECHO Opening $ProjectPath

#mono bin/Debug/Xamaridea.Console.exe -p=$ProjectPath

#mono bin/Debug/Xamaridea.Console.exe -p=$ProjectPath -s=$AndroidSDKPath

mono bin/Debug/Xamaridea.Console.exe -p=$ProjectPath -s=$AndroidSDKPath -t=$CustomTemplatPath