//#r"nuget: PlainTool,1.0.0.58"

using System;
using System.Security.Cryptography;

public class RefUInt16
{
    public RefUInt16(ushort value) => Value = value;

    public ushort Value { get; }
}

public class UInt16CommandLineArgumentMeta : CommandLineArgumentMeta<RefUInt16>
{
    public UInt16CommandLineArgumentMeta(string key) : base(key)
    {
    }

    public override RefUInt16 Parse(string value) => new(ushort.Parse(value));
}

public abstract class Identifier<TKey> : IIdentifier<TKey>
{
    public Identifier(TKey key) => Key = key;

    public TKey Key { get; }

    public override int GetHashCode() => EqualityComparer<TKey>.Default.GetHashCode(Key);

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj is not IIdentifier<TKey> identifier)
            return false;

        return identifier.Key.Equals(Key);
    }
}

public abstract class CommandLineArgumentMeta<TType> : Identifier<string>, ICommandLineArgumentMeta<TType>
{
    protected CommandLineArgumentMeta(string key) : base(key)
    {
    }

    public abstract TType Parse(string value);
}

public class StringCommandLineArgumentMeta : CommandLineArgumentMeta<string>
{
    public StringCommandLineArgumentMeta(string key) : base(key)
    {
    }

    public override string Parse(string value) => value;
}

public interface IIdentifier<out TKey>
{
    public TKey Key { get; }
}

public interface ICommandLineArgumentMeta<out TType> : IIdentifier<string>
{
    TType Parse(string value);
}

public sealed class CommandLineContext
{
    public readonly Dictionary<string, ICommandLineArgumentMeta<object>> _metas = new();

    public ICommandLineArgumentMeta<object> this[string key] => _metas[key];

    public CommandLineContext(IEnumerable<ICommandLineArgumentMeta<object>> metas)
    {
        foreach (var meta in metas)
            _metas.Add(meta.Key, meta);
    }

    public IEnumerable<ICommandLineArgumentMeta<object>> Metas => _metas.Values;

    public bool ContainsKey(string key) => _metas.ContainsKey(key);

    public Dictionary<string, object> Parse(IEnumerable<string> args)
    {
        var result = new Dictionary<string, object>();

        foreach (var arg in args)
        {
            var items = arg.Split("=");
            result[items[0]] = this[items[0]].Parse(items[1]);
        }

        return result;
    }
}

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
