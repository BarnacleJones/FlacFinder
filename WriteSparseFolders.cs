namespace FlacFinder
{
    public static class WriteSparseFolders
    {
        public static void Write()
        {
            var filePrintStream = new List<string>();
            var musicFolders = MusicFolderConfigurator.GetMusicLocations();
            var folderSongCounts = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            // Scan all music folders and count music files per folder
            foreach (var musicFolder in musicFolders)
            {
                foreach (var file in Directory.GetFiles(musicFolder, "*.*", SearchOption.AllDirectories))
                {
                    if (!file.IsMusicFile()) continue;

                    string folderPath = Path.GetDirectoryName(file) ?? "";
                    if (!folderSongCounts.ContainsKey(folderPath))
                        folderSongCounts[folderPath] = new List<string>();

                    folderSongCounts[folderPath].Add(file);
                }
            }

            // Find folders with only 1 or 2 songs
            var sparseFolders = new Dictionary<string, List<string>>();
            foreach (var entry in folderSongCounts)
            {
                if (entry.Value.Count is 1 or 2) // Only log if folder has 1 or 2 songs
                    sparseFolders[entry.Key] = entry.Value;
            }

            // Output results
            if (sparseFolders.Count > 0)
            {
                filePrintStream.Add("Sparse Music Folders (Only 1 or 2 Songs Found):\n");
                foreach (var sparse in sparseFolders)
                {
                    filePrintStream.Add($"- {sparse.Key}");
                    foreach (var fileToWrite in sparse.Value)
                        filePrintStream.Add($"  └ {fileToWrite}");
                }

                FileWriter.WriteToFile(filePrintStream, "sparse_folders.txt");
            }
        }

        
    }
}
