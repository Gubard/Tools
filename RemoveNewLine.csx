var file = new FileInfo(Args[0]);

using(var fileStream = file.OpenText())
{
	var result = new StringBuilder();
	var line = await fileStream.ReadLineAsync();

	while (line != null)
	{
		result.Append(line);
		line = await fileStream.ReadLineAsync();
	}

	Console.WriteLine(result.ToString());
}
