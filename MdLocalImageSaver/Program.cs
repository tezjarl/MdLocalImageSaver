// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;

if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
{
    Console.WriteLine("You should pass a md file name");
    return;
}
var mdFilePath = args[0];
var mdFileDir = Path.GetDirectoryName(mdFilePath)!; //считаем что null тут невозможен
var mdFileContent = File.ReadAllText(mdFilePath);
var client = new HttpClient();

// Регулярка для поиска ссылок на изображения в формате ![alt text](url)
var imageLinkRegex = new Regex(@"!\[.*?\]\((.*?)\)");
var matches = imageLinkRegex.Matches(mdFileContent);

foreach (Match match in matches)
{
    var imageUrl = match.Groups[1].Value;
    var imageName = Path.GetFileName(imageUrl);

    // Загружаем изображение
    var imageBytes = await client.GetByteArrayAsync(imageUrl);
    await File.WriteAllBytesAsync(Path.Combine(mdFileDir, imageName), imageBytes);

    // Заменяем старую ссылку на новую
    mdFileContent = mdFileContent.Replace(imageUrl, imageName);
}

// Сохраняем обновленный .md файл
await File.WriteAllTextAsync(mdFilePath, mdFileContent);