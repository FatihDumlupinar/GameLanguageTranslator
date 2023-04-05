using GameLanguageTranslator.Selenium;
using GameLanguageTranslator.Selenium.Base;
using System.Text.RegularExpressions;
using System.Xml;

Console.Write("Enter game directory path: ");
string gameDirectoryPath = Console.ReadLine() ?? "";
//string gameDirectoryPath = @"D:\SteamLibrary\steamapps\common\Sid Meier's Civilization VI";
string backupFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Sid Meier's Civilization VI");

if (!Directory.Exists(backupFolderPath))
{
    Directory.CreateDirectory(backupFolderPath);
}

string[] xmlFilePaths = Directory.GetFiles(gameDirectoryPath, "*.xml", SearchOption.AllDirectories);

foreach (string xmlFilePath in xmlFilePaths)
{
    try
    {
        string xml = File.ReadAllText(xmlFilePath);
        xml = xml.Replace("version=\"2.0\"", "version=\"1.0\"");
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        if (xmlDoc.SelectSingleNode("//BaseGameText") != null || xmlDoc.SelectSingleNode("//EnglishText") != null)
        {
            string relativePath = xmlFilePath.Substring(gameDirectoryPath.Length + 1);
            string backupFilePath = Path.Combine(backupFolderPath, relativePath);

            string backupDirectoryPath = Path.GetDirectoryName(backupFilePath) ?? "";

            if (!Directory.Exists(backupDirectoryPath))
            {
                Directory.CreateDirectory(backupDirectoryPath);
            }

            File.Copy(xmlFilePath, backupFilePath, true);
            Console.WriteLine($"Backed up: {backupFilePath}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error found: {ex.Message} \n\n File Path: {xmlFilePath}");
    }
}

Console.WriteLine("All XML files are backed up.");

//xml dosyalarını artık yedeklenen klasorden alacak
xmlFilePaths = Directory.GetFiles(backupFolderPath, "*.xml", SearchOption.AllDirectories);

Console.Write("Enter source language code (e.g. en, fr, es): ");
string sourceLanguageCode = Console.ReadLine() ?? "";
//string sourceLanguageCode = "en";

Console.Write("Enter target language code (e.g. tr, de, ja): ");
string targetLanguageCode = Console.ReadLine() ?? "";
//string targetLanguageCode = "tr";

using IBaseSelenium seleniumBot = new GoogleTranslateSeleniumBot();

for (int i = 0; i < xmlFilePaths.Length; i++)
{
    string xmlFilePath = xmlFilePaths[i];

    XmlDocument xmlDoc = new XmlDocument();
    xmlDoc.Load(xmlFilePath);

    XmlNodeList rowNodes = xmlDoc.SelectNodes("//Row");

    if (rowNodes != null)
    {
        if (rowNodes.Count > 0)
        {
            for (int j = 0; j < rowNodes.Count; j++)
            {
                XmlNode rowNode = rowNodes[j];
                XmlAttribute tagAttribute = rowNode.Attributes["Tag"];

                if (tagAttribute != null)
                {
                    string englishText = rowNode.SelectSingleNode("Text").InnerText;

                    if (IsTurkish(englishText))
                    {
                        continue;
                    }

                    string translatedText = await seleniumBot.TranslateTextAsync(englishText, sourceLanguageCode, targetLanguageCode);

                    Console.Clear();
                    Console.WriteLine($"Completed : {xmlFilePaths.Length}/{(i + 1)} ");
                    Console.WriteLine($"File Name : {Path.GetFileName(xmlFilePath)} ");
                    Console.WriteLine($"Text : {englishText} ");
                    Console.WriteLine($"Translated : {translatedText} ");

                    rowNode.SelectSingleNode("Text").InnerText = translatedText;
                }
            }

            xmlDoc.Save(xmlFilePath);

        }

    }

}

Console.WriteLine("Translation complete.");

static bool IsTurkish(string text)
{
    // Check if the sentence contains Turkish characters
    if (Regex.IsMatch(text, @"[ğüşıöçĞÜŞİÖÇ]"))
    {
        return true;
    }

    return false;
}