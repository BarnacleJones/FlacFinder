using FlacFinder;

try
{
    Console.Title = "FlacFinder";
    Console.WriteLine(@"
  ______ _               _____   ______ _           _           
 |  ____| |        /\   / ____| |  ____(_)         | |          
 | |__  | |       /  \ | |      | |__   _ _ __   __| | ___ _ __ 
 |  __| | |      / /\ \| |      |  __| | | '_ \ / _` |/ _ \ '__|
 | |    | |____ / ____ \ |____  | |    | | | | | (_| |  __/ |   
 |_|    |______/_/    \_\_____| |_|    |_|_| |_|\__,_|\___|_|   
                                                                ");

    var running = true;
    while (running)
    {
        Console.WriteLine("Select an option:");
        Console.WriteLine("1. Search for an artist and check if you have the albums in FLAC");
        Console.WriteLine("2. Write potential duplicates to a log file (this will take some time, just wait...)");
        Console.WriteLine("3. Write all folders with only one or two songs to a log file");
        Console.WriteLine("4. Print instructions");
        Console.WriteLine("5. Exit\n");

        Console.Write("Enter your choice: ");
        string choice = Console.ReadLine();
        if (choice == null)
            return;

        switch (choice)
        {
            case "1":
                SearchFromArtist();
                break;
            case "2":
                WriteDuplicates.Write();
                break;
            case "3":
                WriteSparseFolders.Write();
                break;
            case "4":
                PrintInstructions();
                break;
            case "5":
                running = false;
                Console.WriteLine("Exiting FlacFinder. Goodbye!");
                break;
            case "6":
                break;
            case "7":
                return;
            default:
                Console.WriteLine("Invalid choice. Please enter a number between 1 and 7.\n");
                break;
        }
        Console.WriteLine("\n\n\n\n");
    }
}
catch (Exception e)
{
    Console.WriteLine("An error occurred: " + e.Message);
}


void PrintInstructions()
{
    Console.WriteLine("\nInstructions:");
    Console.WriteLine("1. Place 'music.txt' in the same directory as this executable.");
    Console.WriteLine("2. Ensure 'music.txt' contains paths to your music folders, one per line.");
    Console.WriteLine("3. Select an option from the menu to analyze your FLAC collection.");
    Console.WriteLine("4. Log files will be generated in the same directory.\n");
}

static void SearchFromArtist()
{
    Console.Write("Enter artist name: ");
    var artist = Console.ReadLine();
    if (artist == null)
        Console.WriteLine("No artist!");
    else
        ArtistSearch.SearchArtist(artist);
}