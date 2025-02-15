using System.Text.RegularExpressions;

namespace FlacFinder
{
    public static class WriteDuplicates
    {
        public static void Write()
        {
            var filePrintStream = new List<string>();
            var musicFolders = MusicFolderConfigurator.GetMusicLocations();
            var songDictionary = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var musicFolder in musicFolders)
            {
                foreach (var file in Directory.GetFiles(musicFolder, "*.*", SearchOption.AllDirectories))
                {
                    if (!file.IsMusicFile()) continue;

                    string folderName = Path.GetFileName(Path.GetDirectoryName(file) ?? ""); // Album folder
                    string fileName = Path.GetFileNameWithoutExtension(file);

                    string normalizedKey = NormalizeSongTitle(folderName) + "_" + NormalizeSongTitle(fileName);

                    if (!songDictionary.ContainsKey(normalizedKey))
                        songDictionary[normalizedKey] = new List<string>();

                    songDictionary[normalizedKey].Add(file);
                }
            }

            // Find duplicates
            var duplicates = new Dictionary<string, List<string>>();
            foreach (var entry in songDictionary)
            {
                if (entry.Value.Count > 1) // Only store if multiple files match
                    duplicates[entry.Key] = entry.Value;
            }

            // Output results
            if (duplicates.Count > 0)
            {
                filePrintStream.Add("Duplicate Songs Found:\n");
                foreach (var duplicate in duplicates)
                {
                    filePrintStream.Add($"- {duplicate.Key} ");
                    foreach (var fileToWrite in duplicate.Value)
                        filePrintStream.Add($"  └ {fileToWrite} ");
                }

                FileWriter.WriteToFile(filePrintStream, "duplicates.txt");
            }
        }

        static string NormalizeSongTitle(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";

            string cleaned = Regex.Replace(input, @"\s*\(?\d*\)?\s*copy", "", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\s*\(\d+\)", ""); // Remove numbers in parentheses
            cleaned = Regex.Replace(cleaned, @"[^a-zA-Z0-9]", "", RegexOptions.IgnoreCase); // Remove special characters

            return cleaned.Trim().ToLower();
        }
    }
}
