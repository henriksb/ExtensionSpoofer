# ExtensionSpoofer

ExtensionSpoofer is a Visual Basic .NET desktop app that demonstrates how file names and icons can be visually manipulated on Windows while the underlying executable file type remains unchanged.

It uses the Unicode **Right-to-Left Override** character (`U+202E`) to reverse displayed characters in part of the file name and can swap the executable icon to match a chosen spoofed extension.

## How it works (example)

Original file name:

`test_application[RIGHT_TO_LEFT_OVERRIDE]gnp.exe`

Displayed in Explorer as:

`test_applicationexe.png`

The file still executes as an executable format.

## Supported source executable extensions

- `.exe`
- `.scr`
- `.com`

> Note: `.com` icon replacement is not always supported by the Windows shell.

## Usage

1. Download the precompiled executable from the [latest release](https://github.com/henriksb/ExtensionSpoofer/releases/latest).
2. Place the executable next to the [Win10Icons](https://github.com/henriksb/ExtensionSpoofer/tree/master/Win10Icons) folder (or choose a custom icon folder at startup).
3. Open an executable file.
4. Enter a file name, choose source extension and spoof extension.
5. Generate the spoofed file name.

### Icon requirements

- Icon files **must** be valid `.ico` files.
- Renaming `.png`, `.jpg`, or other formats to `.ico` is not sufficient.
- Each icon file should be named after the target display extension (for example `pdf.ico`, `txt.ico`).

## Educational use and safety notice

This project is intended for **education and awareness**: for example, showing students why file names and icons alone are not trustworthy security indicators.

Always use responsibly and only in authorized environments.

## Screenshots

![Usage animation](https://raw.githubusercontent.com/henriksb/ExtensionSpoofer/master/UsageGIF.gif)

![Spoofed file examples](https://raw.githubusercontent.com/henriksb/ExtensionSpoofer/master/Spoof.png)
