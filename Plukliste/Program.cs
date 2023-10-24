//Eksempel på funktionel kodning hvor der kun bliver brugt et model lag
using System.Xml.Serialization;

namespace Plukliste;

class PluklisteProgram { 

    static void Main()
    {
        PluklisteFile Files = new PluklisteFile();
        Files.Reload();
        Files.GetCurrentFile();
        Files.CreateTemplate();
        //Arrange
        char ReadKey = ' ';
        var StandardColor = Console.ForegroundColor;
        Directory.CreateDirectory("import");

        if (!Directory.Exists("export"))
        {
            Console.WriteLine("Directory \"export\" not found");
            Console.ReadLine(); 
            return;
        }

        //ACT
        while (ReadKey != 'Q')
        {
            if (Files.FileCount == 0)
            {
                Console.WriteLine("No files found.");
            }
            else
            {
                Files.GetCurrentFile();                
                Console.WriteLine($"Plukliste {Files.CurrentIndex + 1} af {Files.FileCount}");
                Console.WriteLine($"\nfile: {Files.FileName}");

                //print plukliste
                if (Files.Plukliste != null && Files.Plukliste.Lines != null)
                {
                    Console.WriteLine("\n{0, -13}{1}", "Name:", Files.Plukliste.Name);
                    Console.WriteLine("{0, -13}{1}", "Forsendelse:", Files.Plukliste.Shipment);
                    //TODO: Add adresse to screen print

                    Console.WriteLine("\n{0,-7}{1,-9}{2,-20}{3}", "Antal", "Type", "Produktnr.", "Navn");
                    foreach (var item in Files.Plukliste.Lines)
                    {
                        Console.WriteLine("{0,-7}{1,-9}{2,-20}{3}", item.Amount, item.Type, item.ProductID, item.Title);
                    }
                }
            }

            void PrintOptions(char firstLetter, string value)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(firstLetter);
                Console.ForegroundColor = StandardColor;
                Console.WriteLine(value);
            }

            //Print options
            Console.WriteLine("\n\nOptions:");
            PrintOptions('Q', "uit");

            if (Files.CurrentIndex >= 0)
            {
                PrintOptions('A', "fslut plukseddel");
            }
            if (Files.HasPrevious())
            {
                PrintOptions('F', "orrige plukseddel");
            }
            if (Files.HasNext())
            {
                PrintOptions('N', "æste plukseddel");
            }
            PrintOptions('G', "enindlæs pluksedler");

            ReadKey = Console.ReadKey().KeyChar;
            ReadKey = Char.ToUpper(ReadKey);
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Red; //status in red
            switch (ReadKey)
            {
                case 'G':
                    Files.Reload();
                    Console.WriteLine("Pluklister genindlæst");
                    break;
                case 'F':
                    Files.Previous();
                    break;
                case 'N':
                    Files.Next();
                    break;
                case 'A':
                    Files.ImportDirectory();
                    break;
                case 'P':
                    string FileToCreate = Files.CreateTemplate();

                    Console.WriteLine("Added the template for {0} at print/{0}.html", FileToCreate);
                    break;
            }
            Console.ForegroundColor = StandardColor; //reset color

        }
    }
}
