using System.Globalization;

var random = new Random();
var builder = new StringBuilder();
var count = 20;
var ids = new int[count];
var historyIds = Enumerable.Range(0, 20).ToArray();
var numberFormat = new NumberFormatInfo
{
	NumberDecimalSeparator = "."
};
var methods = new string[]
{
	"CalculationMethodEnumView.SimpleInterest",
	"CalculationMethodEnumView.CompoundInterest"
};
builder.AppendLine("new List<CalculationsHistoryViewModelItem>");
builder.AppendLine("{");
FillRandomIntUnique(0, int.MaxValue, ids);

for(var index = 0; index < count; index++)
{
	builder.AppendLine("new()");
	builder.AppendLine("{");
	builder.AppendLine($"Id = {historyIds[CreateRandomByte(0, (byte)(historyIds.Length - 1))]},");
	builder.AppendLine($"CalculationMethod = {methods[CreateRandomByte(0, (byte)(methods.Length - 1))]},");
	builder.AppendLine($"MonthTerm = {CreateRandomUInt16(0, ushort.MaxValue)},");
	builder.AppendLine(string.Format(numberFormat, "Summa = {0:F2},", CreateRandomDouble(1, 10000)));
	builder.AppendLine(string.Format(numberFormat, "AnnualInterestRate = {0:F2},", CreateRandomDouble(0, 1)));
	builder.AppendLine("CreatedDataTime = DateTimeOffset.Now");
	/*builder.AppendLine(string.Format(numberFormat, "Accrual = {0:F2},", CreateRandomDouble(1, 1000)));
	builder.AppendLine($"Id = {ids[index]},");
	
	builder.AppendLine($"HistoryId = {historyIds[CreateRandomInt(0, historyIds.Length - 1)]},");
	*/
	builder.AppendLine("},");
	Console.WriteLine($"{((index + 1.0) / count) * 100.0}%");
}

builder.AppendLine("};");
Console.WriteLine(builder.ToString());

byte CreateRandomByte()
{
	var bytes = new byte[1];
	random.NextBytes(bytes);

	return bytes[0];
}

byte CreateRandomByte(byte min, byte max)
{
	while(true)
	{
		var value = CreateRandomByte();

		if(value < min)
		{
			continue;
		}

		if(value > max)
		{
			continue;
		}

		return value;
	}
}

ushort CreateRandomUInt16()
{
	var bytes = new byte[2];
	random.NextBytes(bytes);
	var result = BitConverter.ToUInt16(bytes);

	return result;
}

ushort CreateRandomUInt16(ushort min, ushort max)
{
	while(true)
	{
		var value = CreateRandomUInt16();

		if(value < min)
		{
			continue;
		}

		if(value > max)
		{
			continue;
		}

		return value;
	}
}


double CreateRandomDouble()
{
	var bytes = new byte[8];
	random.NextBytes(bytes);
	var result = BitConverter.ToDouble(bytes);

	return result;
}

double CreateRandomDouble(double min, double max)
{
	while(true)
	{
		var value = CreateRandomDouble();

		if(double.IsNaN(value))
		{
			continue;
		}

		if(double.IsInfinity(value))
		{
			continue;
		}

		if(value < min)
		{
			continue;
		}

		if(value > max)
		{
			continue;
		}

		return value;
	}
}

int CreateRandomInt()
{
	var bytes = new byte[4];
	random.NextBytes(bytes);
	var result = BitConverter.ToInt32(bytes);

	return result;
}

int CreateRandomInt(int min, int max)
{
	while(true)
	{
		var value = CreateRandomInt();

		if(value < min)
		{
			continue;
		}

		if(value > max)
		{
			continue;
		}

		return value;
	}
}

void FillRandomIntUnique(int min, int max, Span<int> result)
{
	for(var index = 0; index < result.Length; index++)
	{
		var slice = result.Slice(0, index);
		
		while(true)
		{
			var value = CreateRandomInt(min, max);

			if(slice.Contains(value))
			{
				continue;
			}

			result[index] = value;
			
			break;
		}
	}
}
