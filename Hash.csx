using System;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

public class Ref<T> where T : struct
{
    public Ref(T value) => Value = value;

    public T Value { get; set; }
}

public class RefUInt16
{
    public RefUInt16(ushort value) => Value = value;

    public ushort Value { get; }
}

public class UInt16CommandLineArgumentMeta : CommandLineArgumentMeta<Ref<ushort>>
{
    public UInt16CommandLineArgumentMeta(string key, Ref<ushort> value) : base(key, value)
    {
    }

    public override Ref<ushort> Parse(string value) => new(ushort.Parse(value));
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
    protected CommandLineArgumentMeta(string key, TType @default) : base(key) => Default = @default;

    public TType Default { get; }

    public abstract TType Parse(string value);
}

public class StringCommandLineArgumentMeta : CommandLineArgumentMeta<string>
{
    public StringCommandLineArgumentMeta(string key, string str) : base(key, str)
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
    TType Default { get; }

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

    public Dictionary<string, object> Parse(string[] args)
    {
        var result = new Dictionary<string, object>();

        foreach (var arg in args)
        {
            var items = arg.Split("=");
            result[items[0]] = this[items[0]].Parse(items[1]);
        }

        foreach (var meta in Metas)
            if (!result.ContainsKey(meta.Key))
                result[meta.Key] = meta.Default;

        return result;
    }
}

var context = new CommandLineContext(new ICommandLineArgumentMeta<object>[]
{
    new StringCommandLineArgumentMeta("key", string.Empty),
	new UInt16CommandLineArgumentMeta("length", new(512)),
    new StringCommandLineArgumentMeta("regex", "^[\\s\\S]$"),
});
var args = Environment.GetCommandLineArgs().ToArray();
var arguments = context.Parse(args.Where(x => x.Contains("=")).ToArray());
var value = ((string)arguments["key"]).ToUpper().Trim();
Console.WriteLine($"key {value}");
var number = (int)((Ref<ushort>)arguments["length"]).Value;
var regex = new Regex((string)arguments["regex"], RegexOptions.Compiled);
var result = new StringBuilder();

using (var sha = SHA512.Create())
{
	var bytes = GetBytes(sha, value);
    var bytesIndex = 0;

    for(var index = 0; index < number; index++, bytesIndex++)
    {
        if(bytes.Length == bytesIndex)
        {
            bytes = GetBytes(sha, result.ToString());
            bytesIndex = 0;
        }

        if(!IsASCIIChar(bytes[bytesIndex]))
        {
            index--;

            continue;
        }

        var stringChar = ToASCIIString(bytes[bytesIndex]);

        if(!regex.IsMatch(stringChar))
        {
            index--;

            continue;
        }

        result.Append(stringChar);
    }

    Console.WriteLine(result.ToString());
}

byte[] GetBytes(SHA512 sha, string str)
{
    var encoding = Encoding.UTF8;
	var bytes = encoding.GetBytes(str);

	return sha.ComputeHash(bytes);
}

string ToASCIIString(byte b) => Encoding.ASCII.GetString(new []{b});

bool IsASCIIChar(byte b)
{
    if(b < 32)
		return false;
	if(b > 126)
		return false;
    
    return true;
}