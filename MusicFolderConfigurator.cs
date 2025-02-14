namespace FlacFinder
{
    public static class MusicFolderConfigurator
    {       
        public static List<string> GetMusicLocations() 
        {
            try
            {
                return FileReader.ReadFile("music.txt");
            }
            catch (Exception e)
            {
                Console.WriteLine("Oh no! Unable to read the music library path file..\n");
                Console.WriteLine("Is it in the same directory as the program?..\n");
                Console.WriteLine("Is it named music.txt?\n");
                Console.WriteLine("This was the error: " + e.Message);
                Console.ReadLine();
                throw;
            }
            
        }
    }
}
