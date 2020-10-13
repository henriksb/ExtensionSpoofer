# 📃 What is FileSpoofer?
A simple program with a graphical user interface to spoof file extensions and icons. The program relies on the right-to-left override character (U+202E), which essentially mirrors all characters written after it. Below is an example of how the character works:

test_application[RIGHT_TO_LEFT_CHARACTER]gnp.exe

..and this results in the file name looking like:

test_applicationexe.png

In addition to the extension seemingly being changed, the executable icon has changed to one matching the fake extension.

More file extensions have identical functionalities to the .exe extension, namely .scr and .com. In FileSpoofer, you can choose between these extensions, making your names and programs (malware) seem extremely authentic. Many people know about the right-to-left override character and are thus very aware of this attack. Most people do, however, not know about .com and .scr and their identical functionality to .exe. Even regular computer users get suspicious of the exe ending, as there are not many words (if any) ending with exe. The .jar (java files) could also be a possibility, though not added yet.

## 🎥 GIF of software
![alt text](https://raw.githubusercontent.com/henriksb/ExtensionSpoofer/master/UsageGIF.gif)

## 📷 Some spoofed files
![alt text](https://raw.githubusercontent.com/henriksb/ExtensionSpoofer/master/Spoof.png)

(You cannot change the icon of .com files)


# 📁 Download and usage
Click [here](https://github.com/henriksb/ExtensionSpoofer/releases/download/1/ExtensionSpoof.exe) to download pre-compiled executable.
You will also need the [Win10Icons](https://github.com/henriksb/ExtensionSpoofer/tree/master/Win10Icons) folder with the icons for the program to work correctly. Using regular images instead of .ico files will not work. After you have downloaded the executable and icon files, everything is finished, and the program is ready to use.

Adding new icons is extremely easy. Add the icon in the icons folder with the name of the extension. Meaning, if you have an 'exe' icon, name it 'exe.ico' and put it in the folder.

## ⚠️ A small warning
The program you are trying to spoof may start to change its name randomly (with a .tmp extension). That is the Windows defender's fault, and you will most likely see a pop-up message in your bottom right corner. To fix this, you can either disable Windows defender (not recommended) or whitelist the program.
