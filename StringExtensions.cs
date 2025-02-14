namespace FlacFinder
{
    public static class StringExtensions
    {
        public static bool IsMusicFile(this string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return false;

            string ext = Path.GetExtension(filePath).ToLower();
            return ext is ".mp3" or ".flac" or ".wav" or ".aac" or ".m4a" or ".ogg";
        }
    }
}
