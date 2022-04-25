Encoding inputEncoding;
Encoding outputEncoding;
FileInfo file;

for(var index = 0; index < Args.Count; index++)
{
	var arg = Args[index];
	var value = Args[index + 1];

	switch(arg)
	{
		case "--filePath":
		{
			file = new FileInfo(value);
			index++;
			break;
		}
		case "--inputEncoding":
		{
			inputEncoding = Encoding.GetEncoding(value);
			index++;
			break;
		}
		case "--outputEncoding":
		{
			outputEncoding = Encoding.GetEncoding(value);
			index++;
			break;
		}
	}
}

Console.WriteLine($"Input encoding: {inputEncoding.BodyName}");
Console.WriteLine($"Output encoding: {outputEncoding.BodyName}");
Console.WriteLine($"File path: {file.FullName}");
byte[] bytes;

using(var stream = file.OpenRead())
{
	bytes = new byte[stream.Length];
	await stream.ReadAsync(bytes);
}

var resultString = inputEncoding.GetString(bytes);
file.Delete();

using(var stream = file.Create())
{
	stream.WriteAsync(outputEncoding.GetBytes(resultString));
}
