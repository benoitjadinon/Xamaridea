Xamaridea
=========

[Visual Studio extension](https://visualstudiogallery.msdn.microsoft.com/9f5a516a-f4d0-4228-9d25-d0273abebf33) / [Xamarin Studio add-in](http://addins.monodevelop.com/Project/Index/233) / command-line tool that allows editing .axml files in **IntelliJ IDEA** or **Android Studio** (Xamarin.Android). It creates a fake android project and uses resources from your Xamarin.Android project by link (thanks to gradle) so every change made in Android IDE will be saved.

**WARNING:** Plugin may change your project structure (Resources folder), do not use it without version control (it's alpha version).

Tired?

![Alt text](http://habrastorage.org/files/485/2b5/c99/4852b5c9907f4e268ccc5b97fdf504ce.png)

Plugin will help!

![Alt text](http://habrastorage.org/files/de9/a76/7db/de9a767db59d40b19d9559b78cff7540.png)
![Alt text](http://habrastorage.org/files/c13/935/3c9/c139353c9b5c44119df24371f73ac92b.png)

## Command Line Usage
       
### Windows

    Xamaridea.Console.exe 
       -project="C:\Projects\SomeProject\Android\Android.csproj" 
       -sdk="C:\SDKs\android-sdk-macosx" 
       -template="C:\My Documents\CustomTemplate"
### Mac

    mono Xamaridea.Console.exe 
       -project="/Users/XXX/SomeProject/Android/Android.csproj" 
       -sdk="/Users/XXX/SDKs/android-sdk-macosx" 
       -template="/Users/XXX/CustomTemplate"       
