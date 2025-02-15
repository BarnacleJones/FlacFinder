using System.Text.RegularExpressions;

namespace FlacFinder
{
    public static class ArtistSearch
    {
        static string Normalize(string input) => Regex.Replace(input, @"[^a-zA-Z0-9]", "", RegexOptions.IgnoreCase); // Remove non-alphanumeric characters
        static string CreateLoosePattern(string input) => Regex.Replace(input, @"\s+", ".*", RegexOptions.IgnoreCase); // "Run the Jewels" -> "Run.*the.*Jewels"

        public static void SearchArtist(string artistName)
        {
            var musicLocations = MusicFolderConfigurator.GetMusicLocations();

            var matchingFolders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var possibleMatches = new Dictionary<string, string>();

            var normalizedArtist = Normalize(artistName);
            var loosePattern = CreateLoosePattern(artistName);

            foreach (var location in musicLocations)
            {
                var directories = Directory.GetDirectories(location, "*", SearchOption.AllDirectories); //Recursively search all directories

                foreach (var dir in directories)
                {
                    var folderName = Path.GetFileName(dir);
                    var normalizedFolderName = Normalize(folderName);

                    // Exact (normalized) match
                    if (string.Equals(normalizedFolderName, normalizedArtist, StringComparison.OrdinalIgnoreCase))
                    {
                        if (matchingFolders.ContainsKey(folderName))
                            matchingFolders[$"{normalizedFolderName}1"] = dir;

                        else matchingFolders[folderName] = dir;
                    }
                    // Loose match using regex
                    else if (Regex.IsMatch(folderName, loosePattern, RegexOptions.IgnoreCase))
                    {
                        possibleMatches[folderName] = dir;
                    }
                }
            }

            HandleSearchResults(matchingFolders, possibleMatches);
        }

        private static void HandleSearchResults(Dictionary<string, string> matchingFolders, Dictionary<string, string> possibleMatches)
        {
            if (matchingFolders.Count > 0)
            {
                foreach (var folder in matchingFolders) 
                {
                    AnalyzeFolder(folder.Value); // Prioritize exact match

                }
                return;
            }

            if (possibleMatches.Count > 0)
            {
                Console.WriteLine("\nNo exact matches found, but these might be close:\n");
                int index = 1;
                foreach (var folder in possibleMatches)
                {
                    Console.WriteLine($"{index}. {folder.Key}");
                    index++;
                }

                Console.Write("\nEnter the number of your choice (or 'all' to check all):\n ");
                var input = Console.ReadLine();

                var selectedFolders = new List<string>();
                if (input.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    selectedFolders = possibleMatches.Values.ToList();
                }
                else if (int.TryParse(input, out int selectedIndex) && selectedIndex > 0 && selectedIndex <= possibleMatches.Count)
                {
                    selectedFolders.Add(possibleMatches.Values.ElementAt(selectedIndex - 1));
                }
                else
                {
                    Console.WriteLine("\nInvalid selection.");
                    return;
                }

                foreach (var folder in selectedFolders)
                {
                    AnalyzeFolder(folder);
                }
            }
            else
            {
                Console.WriteLine("\nNo matching artist folders found.");
            }
        }

        private static void AnalyzeFolder(string folderPath)
        {
            Console.WriteLine($"\nSearching files in: {folderPath}...");  // Debugging line to log which folder is being checked

            // Recursively check all files in the folder and its subdirectories
            var fileNamesInFolder = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            // Look for FLAC files and other audio files
            var flacFiles = fileNamesInFolder.Where(f => f.EndsWith(".flac", StringComparison.OrdinalIgnoreCase)).ToList();

            var otherAudioFiles = fileNamesInFolder.Where(f =>
                f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".ogg", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".aac", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".m4a", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".wma", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase) ||
                f.EndsWith(".alac", StringComparison.OrdinalIgnoreCase)).ToList();

            // If no FLAC files were found, check for nested directories with FLAC files
            if (flacFiles.Count == 0)
            {
                // Check subdirectories for FLAC files
                var subdirectories = Directory.GetDirectories(folderPath, "*", SearchOption.AllDirectories);
                Console.WriteLine($"\nFound {subdirectories.Length} subdirectories in {folderPath}");

                foreach (var subdir in subdirectories)
                {
                    var subFiles = Directory.GetFiles(subdir, "*.*", SearchOption.TopDirectoryOnly);
                    var flacInSubfolder = subFiles.Where(f => f.EndsWith(".flac", StringComparison.OrdinalIgnoreCase)).ToList();

                    if (flacInSubfolder.Count > 0)
                    {
                        Console.WriteLine($"\nFound FLAC files in subfolder: {subdir}");
                    }
                }
            }

            // Now check the FLAC files and other types
            if (flacFiles.Count > 0 && otherAudioFiles.Count == 0)
            {
                Console.WriteLine($"\nFolder '{folderPath}' (including subdirectories) contains only FLAC files.");
            }
            else if (flacFiles.Count == 0)
            {
                Console.WriteLine($"\nFolder '{folderPath}' (including subdirectories) contains no FLAC files.");
            }
            else
            {
                Console.WriteLine($"\nFolder '{folderPath}' (including subdirectories) contains a mix of FLAC and other file types.");
                Console.WriteLine("Non-FLAC files found:\n");

                foreach (var otherAudioFile in otherAudioFiles)
                    Console.WriteLine(otherAudioFile);
            }
        }

    }
}
