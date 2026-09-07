using SmartSearchIndexer;
using System;

public class SmartSearcher {

    public static List<FileEntry> RunSmartSearchIndexer(string path) {
        Console.WriteLine("Running search in path: " + path);

        Queue<string> directoriesToSearchQueue = new Queue<string>();
        List<string> filesList = new List<string>();

        List<FileEntry> indexedFilesList = new List<FileEntry>();

        // Enqueue the initial inputted path
        directoriesToSearchQueue.Enqueue(path);

        while (directoriesToSearchQueue.Count > 0) {
            string currentDirectoryToSearch = directoriesToSearchQueue.Dequeue();

            foreach (string file in Directory.EnumerateFiles(currentDirectoryToSearch)) {
                filesList.Add(file);
            }

            foreach (string directory in Directory.EnumerateDirectories(currentDirectoryToSearch)) {
                directoriesToSearchQueue.Enqueue(directory);
            }
        }

        Console.WriteLine("Done searching all files! Items found: ");

        List<FileEntry> fileEntriesList = new List<FileEntry>(); // List of indexed files
        foreach(var item in filesList) {
            Console.WriteLine(item);

            string fileContents;
            DateTime fileLastModifiedDate;

            try {
                fileContents = File.ReadAllText(item);
                fileLastModifiedDate = File.GetLastWriteTime(item);

                FileEntry fileEntry = new FileEntry(item, fileContents, fileLastModifiedDate);

                fileEntriesList.Add(fileEntry);
                indexedFilesList.Add(fileEntry);

            } catch {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Unable to open a certain file at path:" + item + " skipping this item.");
                Console.ResetColor();
            }
        }

        SmartSearchIndexer.Program.SaveDirectoryCacheToJson(fileEntriesList); // Save all the indexed files to a directory to avoid indexing all files again next time

        Console.WriteLine("Indexed all files, files count is: " + indexedFilesList.Count);

        return indexedFilesList;
    }

    public static void RunSmartSearcher(string searchInput, out List<(string lineContents, int lineIndex, string name)> searchedFilesWithContentsMatchTupleList,
            out List<SmartSearchIndexer.FileEntry> searchedFilesWithNameMatchList, ref List<FileEntry> indexedFilesList) {
        Console.WriteLine("Searching...");

        string[] searchInputSplitted = searchInput.Split(" ");

        string? searchInputBeforeOperator = null;
        string? searchInputAfterOperator = null;

        QueryOperatorsEnum? queryOperator = null;

        for (int i = 0; i < searchInputSplitted.Length; i++) {
            string word = searchInputSplitted[i];

            (bool hasOperator, QueryOperatorsEnum? detectedQueryType) result = ContainsStringOperator(word);

            if (result.hasOperator) {
                searchInputBeforeOperator = string.Join(" ", searchInputSplitted.Take(i));
                searchInputAfterOperator = string.Join(" ", searchInputSplitted.Skip(i + 1));

                Console.WriteLine($"Substring of the input before the operator is: {searchInputBeforeOperator}, " +
                    $"substring of the input after the operator is: {searchInputAfterOperator}");

                queryOperator = result.detectedQueryType;

                break;
            }
        }

        string searchInputToLower = searchInput.ToLower();

        string? searchInputBeforeOperatorToLower = null;
        if (searchInputBeforeOperator != null)
            searchInputBeforeOperatorToLower = searchInputBeforeOperator.ToLower();
        else {
#if DEBUG
            Console.WriteLine("searchInputBeforeOperator is null!");
#endif
        }

        string? searchInputAfterOperatorToLower = null;

        if (searchInputAfterOperator != null)
            searchInputAfterOperatorToLower = searchInputAfterOperator.ToLower();
        else {
#if DEBUG
            Console.WriteLine("searchInputAfterOperator is null!");
#endif
        }

        searchedFilesWithContentsMatchTupleList = new List<(string, int, string)>();
        searchedFilesWithNameMatchList = new List<FileEntry>();

        // Search for file name matches
        if (queryOperator != null && searchInputBeforeOperatorToLower != null && searchInputAfterOperatorToLower != null) {
            switch (queryOperator) {
                case QueryOperatorsEnum.AND:
                    searchedFilesWithNameMatchList = indexedFilesList.Where
                        (file => Path.GetFileName(file.Path).ToLower().Contains(searchInputBeforeOperatorToLower) &&
                        Path.GetFileName(file.Path).ToLower().Contains(searchInputAfterOperatorToLower)).ToList();

                    Console.WriteLine("Found AND operator in user input! Searching for file name match!");
                    break;
                case QueryOperatorsEnum.OR:
                    searchedFilesWithNameMatchList = indexedFilesList.Where
                        (file => Path.GetFileName(file.Path).ToLower().Contains(searchInputBeforeOperatorToLower) ||
                        Path.GetFileName(file.Path).ToLower().Contains(searchInputAfterOperatorToLower)).ToList();
                    break;
                case QueryOperatorsEnum.NOT:
                    searchedFilesWithNameMatchList = indexedFilesList.Where
                        (file => Path.GetFileName(file.Path).ToLower().Contains(searchInputBeforeOperatorToLower) &&
                        !Path.GetFileName(file.Path).ToLower().Contains(searchInputAfterOperatorToLower)).ToList();
                    break;
            }
        } else {
            searchedFilesWithNameMatchList = indexedFilesList.Where(file => Path.GetFileName(file.Path).ToLower().Contains(searchInputToLower)).ToList();
        }

        // Do a foreach for all the indexed files contents to check for a search match
        foreach (FileEntry file in indexedFilesList) {
            string[] fileLinesStringArray = file.Contents.Split("\n");

            int iteratorIndex = 0;
            foreach (string fileLine in fileLinesStringArray) {
                iteratorIndex++;

                if (searchInputBeforeOperatorToLower != null && searchInputAfterOperatorToLower != null) {
                    string fileLineToLower = fileLine.ToLower();

                    switch (queryOperator) {
                        case QueryOperatorsEnum.AND:
                            if (fileLineToLower.Contains(searchInputBeforeOperatorToLower) && fileLineToLower.Contains(searchInputAfterOperatorToLower)) {
                                searchedFilesWithContentsMatchTupleList.Add((fileLine, iteratorIndex, Path.GetFileName(file.Path)));
                            }

                            Console.WriteLine("Found AND operator in user input! Searching for contents match within file lines!");
                            break;
                        case QueryOperatorsEnum.OR:
                            if (fileLineToLower.Contains(searchInputBeforeOperatorToLower) || fileLineToLower.Contains(searchInputAfterOperatorToLower)) {
                                searchedFilesWithContentsMatchTupleList.Add((fileLine, iteratorIndex, Path.GetFileName(file.Path)));
                            }
                            break;
                        case QueryOperatorsEnum.NOT:
                            if (fileLineToLower.Contains(searchInputBeforeOperatorToLower) && !fileLineToLower.Contains(searchInputAfterOperatorToLower)) {
                                searchedFilesWithContentsMatchTupleList.Add((fileLine, iteratorIndex, Path.GetFileName(file.Path)));
                            }
                            break;
                    }

                } else if (fileLine.ToLower().Contains(searchInputToLower)) {
                    searchedFilesWithContentsMatchTupleList.Add((fileLine, iteratorIndex, Path.GetFileName(file.Path)));
                }
            }

            iteratorIndex = 0;
        }
    }

    public static (bool hasOperator, QueryOperatorsEnum? detectedQueryType) ContainsStringOperator(string stringToCheck) {
        if (stringToCheck == QueryOperatorsEnum.AND.ToString()) {
            return (true, QueryOperatorsEnum.AND);
        } else if (stringToCheck == QueryOperatorsEnum.OR.ToString()) {
            return (true, QueryOperatorsEnum.OR);
        } else if (stringToCheck == QueryOperatorsEnum.NOT.ToString()) {
            return (true, QueryOperatorsEnum.NOT);
        }

        return (false, null);
    }

    public enum QueryOperatorsEnum {
        AND,
        OR,
        NOT
    }
}
