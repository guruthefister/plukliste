using System;
using System.Reflection;

namespace Plukliste;
public class Pluklist
{
    public string? Name;
    public string? Shipment;
    public string? Address;
    public List<Item> Lines = new List<Item>();
    public void AddItem(Item item) { Lines.Add(item); }

    public string PrintType()
    {
        foreach (var item in Lines)
        {
            if (item.Type == ItemType.Print)
            {
                return item.ProductID;
            }
        }
        return "";
    }

    private string getNewColumn(string Item)
    {
        string columnStyle = "text-align:center; padding: 5px; border: 1px solid black;";
        return String.Format("<div style=\"{0}\">{1}</div>", columnStyle, Item);
    }

    public string getLinesAsStringForHTML()
    {
        string gridStyle = "display:grid; grid-template-columns: auto auto auto auto; padding: 5px;";
        string gridLines = String.Format("<div style=\"{0}\">", gridStyle);

        // Headers
        string headerStyle = "text-align:center; border: 1px solid black; padding: 5px; font-weight: bold;";

        gridLines += String.Format("<div style=\"{0}\">Antal</div>", headerStyle);
        gridLines += String.Format("<div style=\"{0}\">Type</div>", headerStyle);
        gridLines += String.Format("<div style=\"{0}\">Produktnr.</div>", headerStyle);
        gridLines += String.Format("<div style=\"{0}\">Navn</div>", headerStyle);

        foreach (var item in Lines)
        {
            if (item.Type.ToString() == "Print") { continue; }
            gridLines += getNewColumn(item.Amount.ToString());
            gridLines += getNewColumn(item.Type.ToString());
            gridLines += getNewColumn(item.ProductID);
            gridLines += getNewColumn(item.Title);
        }

        gridLines += "</div>";
        return gridLines;
        //string spaces = String.Join("", Enumerable.Repeat("&nbsp;", 4));
        //return getLinesAsString("<br>", spaces);
    }
}

public class Item
{
    public string ProductID;
    public string Title;
    public ItemType Type;
    public int Amount;
}



public class PluklisteFile
{
    public int CurrentIndex;
    public string FileName;
    public Pluklist Plukliste;
    public List<string> Files;
    public int FileCount
    {
        get { return Files.Count; }
    }

    public bool HasNext()
    {
        if (CurrentIndex < FileCount - 1)
        {
            return true;
        }
        return false;
    }

    public bool HasPrevious()
    {
       if (CurrentIndex > 0)
        {
            return true;
        }
       return false;
    }

    public void EnumerateFiles()
    {
        Files = Directory.EnumerateFiles("export").ToList();
    }

    public void Reload()
    {
        EnumerateFiles();
        CurrentIndex = 0;
    }

    public Pluklist GetCurrentFile()
    {
        EnumerateFiles();
        using(FileStream file = File.OpenRead(Files[CurrentIndex]))
        {
            System.Xml.Serialization.XmlSerializer xmlSerializer =
                new System.Xml.Serialization.XmlSerializer(typeof(Pluklist));
            Plukliste = (Pluklist?)xmlSerializer.Deserialize(file);
        }
        FileName = Files[CurrentIndex];
        return Plukliste;
    }

    public void ImportDirectory()
    {
        var filewithoutPath = FileName.Substring(FileName.LastIndexOf('\\'));
        File.Move(FileName, string.Format(@"import\\{0}", filewithoutPath));
        Console.WriteLine($"Plukseddel {FileName} afsluttet.");
        EnumerateFiles();
        if (CurrentIndex == Files.Count) CurrentIndex--;
    }

    public Pluklist Next()
    {
        if (HasNext()) 
        {
            CurrentIndex++;
        }
            return GetCurrentFile();
    }

    public Pluklist Previous()
    {
        if (HasPrevious())
        {
            CurrentIndex--;
        }
            return GetCurrentFile();
    }

    public string CreateTemplate()
    {
        var templateText = File.ReadAllText(String.Format("templates/{0}.html", Plukliste.PrintType()));

        templateText = templateText.Replace("[Name]", Plukliste.Name);

        string pluklistLines = Plukliste.getLinesAsStringForHTML();
        templateText = templateText.Replace("[Plukliste]", pluklistLines);

        templateText = templateText.Replace("[Adresse]", Plukliste.Address);
        return templateText;
    }
}

public enum ItemType
{
    Fysisk, Print
}
