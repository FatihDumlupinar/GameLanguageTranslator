using GameLanguageTranslator;
using GameLanguageTranslator.Translate;
using System.Text.RegularExpressions;
using System.Xml;

//Console.Write("Enter game directory path: ");
//string gameDirectoryPath = Console.ReadLine() ?? "" ;
string gameDirectoryPath = "C:\\Users\\fatih\\Desktop\\Yeni klasör (2)";

if (!Directory.Exists(gameDirectoryPath))
{
    Console.WriteLine("Game directory not exist!");
    return;
}

string[] xmlFilePaths = Directory.GetFiles(gameDirectoryPath, "*.xml", SearchOption.AllDirectories);

//Console.Write("Calculate chars count? (yes/no): ");

//if (Console.ReadLine()?.ToLower().Trim() == "yes")
//{
//    Console.Write("chars counting...");

//    int totalWordCount = 0;
//    string text = "";

//    foreach (string file in xmlFilePaths)
//    {
//        try
//        {
//            XmlDocument doc = new XmlDocument();

//            doc.Load(file);

//            XmlNodeList textNodes = doc.SelectNodes("//Text");

//            if (textNodes != null)
//            {
//                foreach (XmlNode textNode in textNodes)
//                {
//                    text = textNode.InnerText;
//                    totalWordCount += text.Length;
//                }
//            }
//        }
//        catch
//        {
//            continue;
//        }


//    }

//    Console.WriteLine($"Total chars count : {totalWordCount.ToString("#,##0")} ");
//}


//while (true)
//{
//    Console.Write("press 1 to continue : ");

//    if (Console.ReadLine()?.Trim() == "1")
//    {
//        break;
//    }
//}

//Console.Write("Enter source language code (e.g. en, fr, es): ");
//string sourceLanguageCode = Console.ReadLine() ?? "";
string sourceLanguageCode = "en";

//Console.Write("Enter target language code (e.g. tr, de, ja): ");
//string targetLanguageCode = Console.ReadLine() ?? "";
string targetLanguageCode = "";

var Translator = new SeleniumBotTranslate(sourceLanguageCode, targetLanguageCode);
//ITranslator Translator = new MicrosoftTranslateText();
//ITranslator Translator = new GoogleTranslateText();

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

                    //string translatedText = await Translator.TranslateTextAsync(englishText, sourceLanguageCode, targetLanguageCode);

                    string[] sentences = Regex.Split(englishText, @"(?<=[\.!\?])\s+");

                    for (int h = 0; h < sentences.Length; h++)
                    {
                        string sentence = sentences[h];

                        string translatedSentence = await Translator.TranslateTextAsync(sentence);

                        sentences[h] = translatedSentence;
                    }

                    string translatedText = string.Join(" ", sentences);

                    Console.Clear();
                    Console.WriteLine($"Completed : {xmlFilePaths.Length}/{(i + 1)} ");
                    Console.WriteLine($"File Name : {Path.GetFileName(xmlFilePath)} ");
                    Console.WriteLine($"Text : {englishText} ");
                    Console.WriteLine($"Translated : {translatedText} ");

                    using var file = new StreamWriter("C:\\Users\\fatih\\Desktop\\Yeni Metin Belgesi.txt", true);

                    file.WriteLine($"Completed : {xmlFilePaths.Length}/{(i + 1)} ");
                    file.WriteLine($"File Name : {Path.GetFileName(xmlFilePath)} ");
                    file.WriteLine($"Text : {englishText} ");
                    file.WriteLine($"Translated : {translatedText} ");

                    file.Close();

                    rowNode.SelectSingleNode("Text").InnerText = translatedText;
                }
            }

            xmlDoc.Save(xmlFilePath);
        }

    }

}

Console.WriteLine("Translation complete.");

