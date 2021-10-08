using System.Security.Cryptography;

var value = Environment.GetCommandLineArgs().Last().Trim().ToUpper();
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

foreach(var byt in normalazed)
	Console.Write(Encoding.ASCII.GetString(new []{byt}));
}

byte Normalaze(byte b)
{
	if(b < 32)
		return (byte)(b + 32);
	if(b > 126)
		return (byte)(b - 126 + 32);

	return b;
}
