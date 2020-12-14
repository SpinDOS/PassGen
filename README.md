# PassGen
Small tool for safe password generation

The tool generates password for website based on provided website key and user master password. It allows you to get different passwords for different websites without need to remember them all - you must remember only single master secret password. Compromising pass for one website does not affect safety of other passwords.

The technique is based on hashing. It's easy to calculate hash of given website key and master password and to generate password based on that hash, but it's impossible to get master password or password for another website from password of some [compromised] website.

Usage: `dotnet passgen.dll <website> [master-pass]`

master-pass can be provided with command line argument, environment variable PG_SALT or stored in text file under your home directory.

You can download nuget package from [GitHub packages](https://github.com/SpinDOS/PassGen/packages) and install it as dotnet global tool with the following command: `dotnet tool install passgen --global --add-source "path/to/directory/containing/passgen.1.0.0.nupkg"`

![Build whole solution](https://github.com/SpinDOS/PassGen/workflows/Build%20whole%20solution/badge.svg)
![Test whole solution](https://github.com/SpinDOS/PassGen/workflows/Test%20whole%20solution/badge.svg)
