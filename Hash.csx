#r"nuget: PlainTool,1.0.0.58"

using System;
using System.Security.Cryptography;
using PlainTool.CommandLine;
using PlainTool;

var context = new CommandLineContext(new ICommandLineArgumentMeta<object>[]
{
    new StringCommandLineArgumentMeta("key"),
	new UInt16CommandLineArgumentMeta("maxLength"),
	new StringCommandLineArgumentMeta("values"),
});
var args = Environment.GetCommandLineArgs().ToArray();
var arguments = context.Parse(args.Where(x => x.Contains("=")));
var value = ((string)arguments["key"]).ToUpper().Trim();
Console.WriteLine($"key {value}");
var number = 0;
var values = string.Empty;

if(arguments.ContainsKey("maxLength"))
	number = (int)((RefUInt16)arguments["maxLength"]).Value;
if(arguments.ContainsKey("values"))
	values = (string)arguments["values"];

using (var sha = SHA512.Create())
{
	var encoding = Encoding.UTF8;
	var bytes = encoding.GetBytes(value);

	foreach(var byt in bytes)
	    Console.Write($"{byt:X2} ");

	Console.WriteLine();

	var resultBytes = sha.ComputeHash(bytes);

	foreach(var byt in resultBytes)
		Console.Write($"{byt:X2} ");

	Console.WriteLine();
	var normalazed = new byte[resultBytes.Length];

	for(var index = 0; index < normalazed.Length; index++)
	{
		normalazed[index] = Normalaze(resultBytes[index]);
		Console.Write($"{normalazed[index]:X2} ");
	}

	Console.WriteLine();
	var i = 0;

	foreach(var byt in normalazed)
	{
		var charString = Encoding.ASCII.GetString(new []{byt});

		if(values.Any() && !values.Contains(charString))
			continue;
		if(number == 0||i < number)
			Console.Write(charString);

		i++;
	}
}

byte Normalaze(byte b)
{
	if(b < 32)
		return (byte)(b + 32);
	if(b > 126)
		return (byte)(b - 126 + 32);

	return b;
}
