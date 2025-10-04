using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FolderShieldUI
{
    /// <summary>
    /// Entry point for the FolderShieldUI console application.
    /// Provides simple hide and unhide functionality for directories across
    /// Windows and UNIX platforms. On Windows it sets the Hidden and System
    /// file attributes; on UNIX it renames the directory by prefixing or
    /// removing a dot from the name.
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("FolderShieldUI (C# Console)");
            Console.WriteLine("============================");
            Console.WriteLine("This tool hides or unhides folders across Windows and UNIX platforms.");

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Choose an option:");
                Console.WriteLine("  1) Hide folder");
                Console.WriteLine("  2) Unhide folder");
                Console.WriteLine("  3) Exit");
                Console.Write("Enter your choice [1-3]: ");
                string? choice = Console.ReadLine()?.Trim();

                if (choice == "3")
                {
                    Console.WriteLine("Exiting. Goodbye!");
                    break;
                }

                if (choice != "1" && choice != "2")
                {
                    Console.WriteLine("Invalid selection. Please enter 1, 2, or 3.");
                    continue;
                }

                Console.Write("Enter the full path of the folder: ");
                string? path = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(path))
                {
                    Console.WriteLine("Path cannot be empty. Please try again.");
                    continue;
                }

                try
                {
                    if (choice == "1")
                    {
                        HideFolder(path!);
                    }
                    else
                    {
                        UnhideFolder(path!);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Hides the specified directory. On Windows this sets the Hidden and
        /// System attributes. On UNIX it renames the directory by prefixing a dot.
        /// </summary>
        /// <param name="path">Absolute or relative path to the directory.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if the directory does not exist.</exception>
        /// <exception cref="IOException">Thrown if the hidden name already exists or the rename fails.</exception>
        public static void HideFolder(string path)
        {
            string absPath = Path.GetFullPath(path);
            if (!Directory.Exists(absPath))
            {
                throw new DirectoryNotFoundException($"The directory '{absPath}' does not exist.");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Set Hidden and System attributes
                FileAttributes attributes = File.GetAttributes(absPath);
                attributes |= FileAttributes.Hidden;
                attributes |= FileAttributes.System;
                File.SetAttributes(absPath, attributes);
                Console.WriteLine($"Successfully hid '{absPath}'. It may not be visible in Explorer.");
            }
            else
            {
                string dirName = Path.GetFileName(absPath);
                string? parent = Path.GetDirectoryName(absPath);
                if (dirName.StartsWith('.'))
                {
                    Console.WriteLine($"'{absPath}' is already hidden (starts with a dot). No changes made.");
                    return;
                }
                string hiddenName = "." + dirName;
                string newPath = parent == null ? hiddenName : Path.Combine(parent, hiddenName);
                if (Directory.Exists(newPath))
                {
                    throw new IOException($"Cannot hide '{absPath}' because '{newPath}' already exists.");
                }
                Directory.Move(absPath, newPath);
                Console.WriteLine($"Successfully hid '{absPath}'. New path is '{newPath}'.");
            }
        }

        /// <summary>
        /// Unhides the specified directory. On Windows this clears the Hidden and
        /// System attributes. On UNIX it renames the directory by removing a
        /// single leading dot from the name.
        /// </summary>
        /// <param name="path">Absolute or relative path to the directory.</param>
        /// <exception cref="DirectoryNotFoundException">Thrown if the directory does not exist.</exception>
        /// <exception cref="ArgumentException">Thrown if the directory name does not begin with a dot on UNIX.</exception>
        /// <exception cref="IOException">Thrown if the target unhidden name already exists or the rename fails.</exception>
        public static void UnhideFolder(string path)
        {
            string absPath = Path.GetFullPath(path);
            if (!Directory.Exists(absPath))
            {
                throw new DirectoryNotFoundException($"The directory '{absPath}' does not exist.");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Clear Hidden and System attributes
                FileAttributes attributes = File.GetAttributes(absPath);
                attributes &= ~FileAttributes.Hidden;
                attributes &= ~FileAttributes.System;
                File.SetAttributes(absPath, attributes);
                Console.WriteLine($"Successfully unhid '{absPath}'. It should now be visible in Explorer.");
            }
            else
            {
                string dirName = Path.GetFileName(absPath);
                string? parent = Path.GetDirectoryName(absPath);
                if (!dirName.StartsWith('.'))
                {
                    throw new ArgumentException($"'{dirName}' does not begin with a dot and is therefore not considered hidden on UNIX systems.");
                }
                string unhiddenName = dirName.TrimStart('.');
                string newPath = parent == null ? unhiddenName : Path.Combine(parent, unhiddenName);
                if (Directory.Exists(newPath))
                {
                    throw new IOException($"Cannot unhide '{absPath}' because '{newPath}' already exists.");
                }
                Directory.Move(absPath, newPath);
                Console.WriteLine($"Successfully unhid '{absPath}'. New path is '{newPath}'.");
            }
        }
    }
}
