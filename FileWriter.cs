namespace FlacFinder
{
    public static class FileWriter
    {
        public static void WriteToFile(IEnumerable<string> linesToWrite, string fileName)
        {
            using (StreamWriter outputFile = new StreamWriter(fileName))
            {
                foreach (string line in linesToWrite)
                    outputFile.WriteLine(line);
            }
        }
    }
}
