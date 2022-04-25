public static class StringExtension
{
	public static IEnumerable<string> GetLines(FileInfo file)
	{
		using(var stream = file.OpenText())
		{
			var line = string.Empty;

			while(line is not null)
			{
				line = stream.ReadLine();
				yield return line;
			}
		}
	}
}

var filePath = Environment.GetCommandLineArgs().Last();
var file = new FileInfo(filePath);
var lines = new List<string>(StringExtension.GetLines(file));

foreach(var line in lines)
	Console.WriteLine(line);

file.Delete();

using(var stream = file.Create())
	stream.Write(Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines.Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x))));
