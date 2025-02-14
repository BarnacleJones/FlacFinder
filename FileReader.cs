namespace FlacFinder
{
    public static class FileReader
    {
        public static List<string> ReadFile(string location)
        {
            var linesInFile = new List<string>();
            try
            {
                var sr = new StreamReader(location);

                var line = sr.ReadLine();

                while (line != null)
                {
                    linesInFile.Add(line);
                    line = sr.ReadLine();
                }

                sr.Close();
                return linesInFile;

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
