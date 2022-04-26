# PassGen
Small tool for safe password generation

The tool generates password for website based on provided website key and user master password. It allows you to get different passwords for different websites without need to remember them all - you must remember only single master secret password. Compromising pass for one website does not affect safety of other passwords.

The technique is based on hashing. It's easy to calculate hash of given website key and master password and to generate password based on that hash, but it's impossible to get master password or password for another website from password of some [compromised] website.

The tool is distributed in three different ways:

### 1. dotnet global tool for installing on Windows, Linux or MacOS

Usage: `passgen <website> [master-pass]`

master-pass can be provided with command line argument, environment variable PG_SALT or stored in text file under your home directory.

You can download nuget package from [GitHub packages](https://github.com/SpinDOS/PassGen/packages) and install it as dotnet global tool with the following command: `dotnet tool install passgen --global --add-source "path/to/directory/containing/passgen.1.2.0.nupkg"`

<details>
<summary>Example of password generating</summary>

```
> passgen example.com password1234
> p8075f5b4#7G
```
</details>

### 2. APK file for installation on Android OS

The *.apk can be found as an artifact of the latest build of [Build Xamarin.Android APK file](https://github.com/SpinDOS/PassGen/actions?query=workflow%3A%22Build+Xamarin.Android+APK+file%22) GitHub Action and installed on your mobile with default package manager.

<details>
<summary>Example of password generating</summary>
<p>

![Screenshot of Android interface](https://user-images.githubusercontent.com/20726264/102559956-273c9380-40e2-11eb-81f7-825e6b83301a.png)

</p>
</details>

### 3. Web interface for accessing with any browser

The service is hosted in Azure and can be accessed by the following link: https://passgen-alexanderplat.azurewebsites.net

<details>
<summary>Example of password generating</summary>
<p>

![Screenshot of web interface](https://user-images.githubusercontent.com/20726264/102559176-476b5300-40e0-11eb-950d-095dc462939c.png)

</p>
</details>


![Build whole solution](https://github.com/SpinDOS/PassGen/workflows/Build%20whole%20solution/badge.svg)
![Test whole solution](https://github.com/SpinDOS/PassGen/workflows/Test%20whole%20solution/badge.svg)
