# 📃 What is FileSpoofer?
FileSpoofer is a graphical user interface software designed for changing file extensions and icons. It works by using the right-to-left override character (U+202E) that mirrors all characters written after it, thus allowing the user to manipulate the file extension without changing the actual file. With this, the file name appears as if the extension has been changed, while the executable icon is also changed to match the fake extension. Below is an example of how the character works:

test_application[RIGHT_TO_LEFT_CHARACTER]gnp.exe

..and this results in the file name looking like:

test_applicationexe.png

The program allows the user to choose between .exe, .scr, and .com extensions, which are all similar in functionality to .exe. The .com and .scr extensions are often not well known, which makes it easier for the program to seem authentic. Many people know about the right-to-left override character and are thus very aware of this attack. Most people do, however, not know about .com and .scr and their similar functionality to .exe. Even regular computer users get suspicious of the exe ending, as there are not many words (if any) ending with exe.

## 🎥 GIF of software
![alt text](https://raw.githubusercontent.com/henriksb/ExtensionSpoofer/master/UsageGIF.gif)

## 📷 Some spoofed files
![alt text](https://raw.githubusercontent.com/henriksb/ExtensionSpoofer/master/Spoof.png)

(You cannot change the icon of .com files)


# 📁 Download and usage
Click [here](https://github.com/henriksb/ExtensionSpoofer/releases/download/1/ExtensionSpoof.exe) to download a pre-compiled executable.
You will also need the [Win10Icons](https://github.com/henriksb/ExtensionSpoofer/tree/master/Win10Icons) folder with the icons for the program to work correctly. Using regular images instead of .ico files will not work. After you have downloaded the executable and icon files, everything is finished, and the program is ready to use.

Adding new icons is extremely easy. Add the icon in the icons folder with the name of the extension. Meaning, if you have an 'exe' icon, name it 'exe.ico' and put it in the folder. Make sure they are an actual .ico file! Renaming "example.png" to "example.ico" will not work. I will be adding an ico converter soon.

## ❗ Notice
It is important to note that using the software may trigger Windows Defender to start changing the name of the program randomly with a .tmp extension. To avoid this, the user can either disable Windows Defender or whitelist the program.
