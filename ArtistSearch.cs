using System.Text.RegularExpressions;

namespace FlacFinder
{
    public static class ArtistSearch
    {
        public static void SearchArtist(string artistName)
        {
            var musicLocations = MusicFolderConfigurator.GetMusicLocations();

            var matchingFolders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var possibleMatches = new Dictionary<string, string>();

            // Normalize the artist name for comparison
            string Normalize(string input) => Regex.Replace(input, @"[^a-zA-Z0-9]", "", RegexOptions.IgnoreCase);

            var normalizedArtist = Normalize(artistName);

            foreach (var location in musicLocations)
            {
                var directories = Directory.GetDirectories(location);

                foreach (var dir in directories)
                {
                    var folderName = Path.GetFileName(dir);
                    var normalizedFolderName = Normalize(folderName);

                    if (string.Equals(normalizedFolderName, normalizedArtist, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingFolders[folderName] = dir;
                    }
                    else if (Regex.IsMatch(normalizedFolderName, normalizedArtist, RegexOptions.IgnoreCase))
                    {
                        possibleMatches[folderName] = dir;
                    }
                }
            }

            if (matchingFolders.Count == 0 && possibleMatches.Count > 0)
            {
                Console.WriteLine("No exact matches found, but these might be close:");
                int index = 1;
                foreach (var folder in possibleMatches)
                {
                    Console.WriteLine($"{index}. {folder.Key}");
                    index++;
                }

                Console.Write("Enter the number of your choice (or 'all' to check all): ");
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
                    Console.WriteLine("Invalid selection.");
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
                Console.WriteLine("No matching artist folders found.");
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
                Console.WriteLine($"Folder '{folderPath}' contains only FLAC files.");
            }
            else if (flacFiles.Count == 0)
            {
                Console.WriteLine($"Folder '{folderPath}' contains no FLAC files.");
                Console.WriteLine($"This is what was found.\n");

                foreach (var otherAudioFile in otherAudioFiles)
                {
                    Console.WriteLine($"{folderPath}/{otherAudioFile}");
                }
            }
            else
            {
                Console.WriteLine($"Folder '{folderPath}' contains a mix of FLAC and other file types.");
                Console.WriteLine($"This is what was found.\n");

                foreach (var otherAudioFile in otherAudioFiles)
                {
                    Console.WriteLine($"Non flac: {folderPath}/{otherAudioFile}");
                }
                foreach (var flacFile in flacFiles)
                {
                    Console.WriteLine($"FLAC: {folderPath}/{flacFile}");
                }

            }
        }
    }
}


