using System.Text.RegularExpressions;

namespace FlacFinder
{
    public static class ArtistSearch
    {
        static string Normalize(string input) => Regex.Replace(input, @"[^a-zA-Z0-9]", "", RegexOptions.IgnoreCase);//Normalize a name by removing non-alphanumeric characters
        static string CreateLoosePattern(string input) => Regex.Replace(input, @"\s+", ".*", RegexOptions.IgnoreCase); // Turns "Run the Jewels" into "Run.*the.*Jewels"

        public static void SearchArtist(string artistName)
        {
            var musicLocations = MusicFolderConfigurator.GetMusicLocations();

            var matchingFolders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var possibleMatches = new Dictionary<string, string>();

            var normalizedArtist = Normalize(artistName);
            var loosePattern = CreateLoosePattern(artistName); 

            foreach (var location in musicLocations)
            {
                var directories = Directory.GetDirectories(location);

                foreach (var dir in directories)
                {
                    var folderName = Path.GetFileName(dir);
                    var normalizedFolderName = Normalize(folderName);

                    // Exact (normalized) match
                    if (string.Equals(normalizedFolderName, normalizedArtist, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingFolders[folderName] = dir;
                    }
                    // Loose match using regex (e.g., "Run the Jewels" matches "Run the Jewls - RTJ Quatro [1969] HyperXOXO")
                    else if (Regex.IsMatch(folderName, loosePattern, RegexOptions.IgnoreCase))
                    {
                        possibleMatches[folderName] = dir;
                    }
                }
            }

            if (matchingFolders.Count == 0 && possibleMatches.Count > 0)
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
                    Console.WriteLine("\n\nInvalid selection.");
                    return;
                }

                foreach (var folder in selectedFolders)
                {
                    AnalyzeFolder(folder);
                }
            }
            else if (matchingFolders.Count > 0)
            {
                AnalyzeFolder(matchingFolders.Values.First());
            }
            else
            {
                Console.WriteLine("\n\nNo matching artist folders found.");
            }
        }

        private static void AnalyzeFolder(string folderPath)
        {
            var fileNamesInFolder = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

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

            if (flacFiles.Count > 0 && otherAudioFiles.Count == 0)
            {
                Console.WriteLine($"\n\nFolder '{folderPath}' contains only FLAC files.\n\n");
            }
            else if (flacFiles.Count == 0)
            {
                Console.WriteLine($"{folderPath}'\n contains no FLAC files.\n");
                //Console.WriteLine($"This is what was found.\n\n\n");
                //foreach (var otherAudioFile in otherAudioFiles)
                //{
                //    Console.WriteLine($"{otherAudioFile}");
                //}
                //Console.WriteLine("\n\n");
            }
            else
            {
                Console.WriteLine($"\n\nFolder '{folderPath}' contains a mix of FLAC and other file types.\n");
                Console.WriteLine($"This is what was found.\n");

                Console.WriteLine("Non FLAC:\n\n");

                foreach (var otherAudioFile in otherAudioFiles)
                    Console.WriteLine(otherAudioFile);

                //Console.WriteLine("FLAC:\n\n");
                //foreach (var flacFile in flacFiles)
                //    Console.WriteLine(flacFile);
            }
        }
    }
}


