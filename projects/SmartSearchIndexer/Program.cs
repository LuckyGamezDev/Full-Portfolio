using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SmartSearchIndexer {
    internal class Program {
        public static readonly string PROGRAM_CACHE_DIRECTORY; // For official build
        public const string PROGRAM_CACHE_DIRECTORY_DEBUGING = @"Z:\Programming\VisualStudio Projects\CS\SmartSearchIndexer\Cache\"; // For test build

        public static string inputtedPath = "";
        public static List<FileEntry> indexedFilesList = new List<FileEntry>();

        public static bool shouldRunProgram = true;

        static Program() {
            PROGRAM_CACHE_DIRECTORY = Directory.GetCurrentDirectory().ToString() + "\\Cache\\";
        }

        public static void Main(string[] args) {
            inputtedPath = args.Length > 0 ? args[0] : "";

            if (inputtedPath != "") {
                if (!LoadDirectoryCacheFromJson()) { // If no cache could be loaded/found, run the indexer
                    indexedFilesList = SmartSearcher.RunSmartSearchIndexer(inputtedPath);
                }
            } else {
                Console.WriteLine("There was no path given in the command line arguments, please pass in a path as argument!");

                return;
            }

            while (shouldRunProgram) {
                ProgramLoop();
            }
        }

        public static void ProgramLoop() {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Ready to search for files!");
            Console.ResetColor();

            string? userInput = Console.ReadLine();

            if (userInput != null) {
                userInput = userInput.Replace(@"""", "");

                if (userInput.StartsWith("search:")) {
                    string searchValue = userInput.Remove(0, 7);

                    SmartSearcher.RunSmartSearcher(searchValue,
                        out List<(string lineContents, int lineIndex, string name)> searchedFilesWithContentsMatchTupleList,
                        out List<FileEntry> searchedFilesWithNameMatchList, ref indexedFilesList
                    );

                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine("Found " + (searchedFilesWithNameMatchList.Count + searchedFilesWithContentsMatchTupleList.Count) + " matching files: ");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("File names found with the search: ");
                    Console.ResetColor();

                    if (searchedFilesWithNameMatchList.Count > 0) {
                        foreach (var file in searchedFilesWithNameMatchList) {
                            Console.WriteLine(file.Path);
                        }
                    } else {
                        Console.WriteLine("None.");
                    }

                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine("File contents found with the search: ");
                    Console.ResetColor();

                    if (searchedFilesWithContentsMatchTupleList.Count > 0) {
                        foreach (var fileTuple in searchedFilesWithContentsMatchTupleList) {
                            string NORMAL = Console.IsOutputRedirected ? "" : "\x1b[39m";
                            string GREY = Console.IsOutputRedirected ? "" : "\x1b[97m";
                            string BLUE = Console.IsOutputRedirected ? "" : "\x1b[94m";
                            string CYAN = Console.IsOutputRedirected ? "" : "\x1b[96m";
                            string GREEN = Console.IsOutputRedirected ? "" : "\x1b[92m";
                            string RED = Console.IsOutputRedirected ? "" : "\x1b[91m";
                            string MAGENTA = Console.IsOutputRedirected ? "" : "\x1b[95m";
                            string UNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[4m";
                            string NOUNDERLINE = Console.IsOutputRedirected ? "" : "\x1b[24m";
                            string BOLD = Console.IsOutputRedirected ? "" : "\x1b[1m";
                            string NOBOLD = Console.IsOutputRedirected ? "" : "\x1b[22m";

                            Console.WriteLine($"{GREY}{Regex.Replace(fileTuple.lineContents, searchValue, m => $"{UNDERLINE}{BOLD}{m.Value}{NOUNDERLINE}{NOBOLD}", 
                                RegexOptions.IgnoreCase)} {RED}found at line {CYAN}{fileTuple.lineIndex} {MAGENTA}of file {BLUE}{fileTuple.name}"
                            );
                        }
                    } else {
                        Console.WriteLine("None.");
                    }

                } else if (userInput.ToLower() == "exit") {
                    shouldRunProgram = false;

                    return;
                }
            }
        }

        public static void SaveDirectoryCacheToJson(List<FileEntry> fileEntriesList) {
            string strippedPath = inputtedPath.Replace("\\", "").Replace(":", "");

#if DEBUG
            string cacheIndexedFilesDirectory = PROGRAM_CACHE_DIRECTORY_DEBUGING + strippedPath;

#elif !DEBUG
            string cacheIndexedFilesDirectory = PROGRAM_CACHE_DIRECTORY + strippedPath;
#endif
            try {
                Directory.CreateDirectory(cacheIndexedFilesDirectory);
            } catch {
                Console.WriteLine("WARNING: Failed to create cache directory! Program will continue without cache");

                return;
            }

            foreach (FileEntry file in fileEntriesList) {
                string serializedJsonString = JsonSerializer.Serialize(file);

                using (FileStream fileStream = File.Create(cacheIndexedFilesDirectory + "\\" + Path.GetFileNameWithoutExtension(file.Path) + ".json")) {
                    byte[] serializedJsonStringBytes = Encoding.UTF8.GetBytes(serializedJsonString);
                    fileStream.Write(serializedJsonStringBytes);
                }
            }
        }

        public static bool LoadDirectoryCacheFromJson() {
            string strippedPath = inputtedPath.Replace("\\", "").Replace(":", "");

#if DEBUG
            if (!Directory.Exists(PROGRAM_CACHE_DIRECTORY_DEBUGING + strippedPath)) {
                Console.WriteLine("No cache yet for this directory, couldn't load anything! Check directory: " + PROGRAM_CACHE_DIRECTORY_DEBUGING + strippedPath);

                return false;
            }
#elif !DEBUG
            if (!Directory.Exists(PROGRAM_CACHE_DIRECTORY + strippedPath)) {
                Console.WriteLine("No cache yet for this directory, couldn't load anything!");

                return false;
            }
#endif

#if DEBUG
            string[] cachedFiles = Directory.GetFiles(PROGRAM_CACHE_DIRECTORY_DEBUGING + strippedPath);
#elif !DEBUG
            // Get all the files within the cache of the inputted directory
            string[] cachedFiles = Directory.GetFiles(PROGRAM_CACHE_DIRECTORY + strippedPath);
#endif
            int succesfullyLoadedFiles = cachedFiles.Length;
            if (cachedFiles != null && cachedFiles.Length > 0) {
                foreach (string cachedFile in cachedFiles) {
                    byte[] cachedFileBytes = File.ReadAllBytes(cachedFile);
                    string cachedFileString = Encoding.UTF8.GetString(cachedFileBytes);

                    FileEntry? deserializedFileEntry;
                    try {
                        deserializedFileEntry = JsonSerializer.Deserialize<FileEntry>(cachedFileString);
                    } catch {
                        // Delete the corrupt json
                        File.Delete(cachedFile);
                        succesfullyLoadedFiles--;

                        continue;
                    }

                    if (!Path.Exists(deserializedFileEntry?.Path)) {
                        File.Delete(cachedFile);

                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("WARNING: Found a cached file which caches a file that doesn't exist anymore. " +
                            "This could be because the file was deleted, or renamed. In case it was renamed, please re-run the indexer " +
                            "to make sure everything is indexed correctly!"
                        );
                        Console.ResetColor();

                        succesfullyLoadedFiles--;

                        continue;
                    }

                    if (deserializedFileEntry != null) {
                        // If the file entries' last modified date isn't the same as the file it defers from, re-read the original file and update the cached one
                        if (deserializedFileEntry.LastDateModified != File.GetLastWriteTime(deserializedFileEntry.Path)) {
                            string fileContents = File.ReadAllText(deserializedFileEntry.Path);

                            deserializedFileEntry.Contents = fileContents;
                            deserializedFileEntry.LastDateModified = File.GetLastWriteTime(deserializedFileEntry.Path);
                        }

                        // Check if the file the file entry defers from still exists, if not delete the cached file
                        if (!Path.Exists(deserializedFileEntry.Path)) { // Don't add the file to the list and delete it if it's no longer existent 
                            File.Delete(cachedFile);

                            continue;
                        }

                        indexedFilesList.Add(deserializedFileEntry);

                        Console.WriteLine(deserializedFileEntry.Path);
                    }

                }

                Console.WriteLine("Succesfully loaded from cache! Loaded " + succesfullyLoadedFiles + " Files.");

                return true;
            } else {
                Console.WriteLine("Couldn't find any existing cache.");

                return false;
            }
        }
    }

    public class FileEntry(string path, string contents, DateTime lastDateModified) {
        public string Path { get; set; } = path;
        public string Contents { get; set; } = contents;
        public DateTime LastDateModified { get; set; } = lastDateModified;
    }
}