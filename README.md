# FolderShieldUI (Console Version)

FolderShieldUI is a lightweight, cross‑platform utility that allows you to
*hide* and *unhide* folders from the command line.  The original
FolderShieldUI project included a graphical interface; however, this
console‑based variant focuses on simplicity and portability while still
providing core functionality.  This version is implemented in **C#** and
builds on the .NET SDK to provide a native executable on Windows, Linux
and macOS.

## Features

* **Cross‑platform** – works on Windows and UNIX‑like systems (Linux/macOS).
* **Hide/Unhide** – quickly toggle the hidden status of a directory.
* **Simple interface** – intuitive menu driven flow with clear prompts.
* **Safe operations** – validates paths and reports descriptive errors.
* **Reusable API** – functions can be imported and used programmatically.

## How It Works

On **Windows** the program uses the built in `System.IO.File` APIs to
manipulate file attributes.  To hide a directory it sets both the *Hidden*
and *System* attributes via `File.SetAttributes`.  Clearing these
attributes makes the folder visible again.  Using the system attribute
prevents the folder from appearing when Explorer is configured to show
hidden files.

On **UNIX‑like systems** there is no dedicated hidden attribute.  The
convention is that any file or folder whose name begins with a dot (`.`)
is treated as hidden by most file explorers and shell commands.  To hide a
directory the program renames it by prefixing the name with a dot.  To
unhide it the program removes the leading dot.  Note that renaming a
directory changes its full path – plan accordingly if other scripts or
applications reference the directory by name.

## Installation

1. Ensure you have the **.NET 6 SDK** or newer installed.  You can
   download it from [dotnet.microsoft.com](https://dotnet.microsoft.com/download).
2. Clone or download this repository.
3. Open a terminal and navigate to the `FolderShieldUI` directory.
4. Build and run the application using the `dotnet` CLI:

```bash
dotnet run --project FolderShieldUI.csproj
```

No external NuGet packages are required.

## Usage

When you run the script you will see a simple menu:

```
FolderShieldUI (Console)
==========================
This tool hides or unhides folders across Windows and UNIX platforms.

Choose an option:
  1) Hide folder
  2) Unhide folder
  3) Exit
Enter your choice [1-3]:
```

1. Select **1** to hide a folder or **2** to unhide a folder.
2. Enter the full path of the folder when prompted.  Relative paths
   are accepted but will be converted to absolute paths internally.
3. The script performs the operation and reports success or failure.
4. Select **3** or press `Ctrl+C` at any time to exit.

### Programmatic Use

If you wish to incorporate this functionality into your own applications you
can reference the compiled assembly and call the static methods directly:

```csharp
using FolderShieldUI;

// Hide a folder (returns nothing on Windows, prints new path on UNIX)
Program.HideFolder(@"/path/to/directory");

// Unhide a folder
Program.UnhideFolder(@"/path/to/directory");
```

Both methods will throw descriptive exceptions on failure.  Ensure you
catch `DirectoryNotFoundException`, `IOException` or `ArgumentException`
as appropriate.

## Notes and Common Issues

* **Permissions** – On Windows, hiding or unhiding a system directory may
  require administrator privileges.  If you see an error such as
  `Access to the path is denied`, run the terminal or command prompt as
  an administrator.

* **Invalid path** – If the path does not exist or is not a directory a
  `DirectoryNotFoundException` or `ArgumentException` will be thrown.
  Double check the path and ensure it is correct.

* **Existing hidden name** – On UNIX systems, hiding a folder renames it
  by prefixing a dot.  If a directory with the hidden name already exists
  an `IOException` will be thrown.  Remove or rename the conflicting
  directory before retrying.

* **Unhiding non‑hidden folders** – Attempting to unhide a directory that
  does not begin with a dot on UNIX will throw an `ArgumentException`
  indicating that the folder is not considered hidden.  Confirm that you
  have specified the correct path (you may need to include the leading
  dot).

* **Changed paths** – After hiding a directory on UNIX, its name changes
  (e.g. from `Documents` to `.Documents`).  Any applications referencing
  the original path will need to be updated.  The program prints the new
  path to aid in automation.

* **Restoring original names** – When unhiding, only a single leading dot
  is removed.  If a folder name contains multiple leading dots (e.g.
  `..config`) the program will remove just one dot, resulting in
  `.config`.  Subsequent calls will remove additional dots one at a time.

## Version History

* **v1.0.0 (2025-10-02)** – Initial C# console release.  Provides hide
  and unhide functionality on Windows and UNIX platforms with
  comprehensive error handling and a simple text UI.  Replaces the
  original Python implementation.

## License

This project is released under the MIT License.  See the `LICENSE` file for
details.

## Acknowledgements

FolderShieldUI (Console Version) was developed to provide a minimalist
alternative to graphical folder hiding tools.  It draws inspiration from
common practices on both Windows and UNIX platforms.
