var rate = 0.0;
var hours = 0.0;
var course = 0.0;
var withoutTax = 0.0;

for(var index = 0; index < Args.Count; index++)
{
	var arg = Args[index];

	if(arg == "--rate")
	{
		rate = double.Parse(Args[++index]);
		
		continue;
	}

	if(arg == "--hours")
	{
		hours = double.Parse(Args[++index]);

		continue;
	}

	if(arg == "--course")
	{
		course = double.Parse(Args[++index]);

		continue;
	}

	if(arg == "--withoutTax")
	{
		withoutTax = double.Parse(Args[++index]);

		continue;
	}
}

var conventionalUnits = rate * hours;
var currency = conventionalUnits * course;
var total = currency * withoutTax;

Console.WriteLine($"Conventional units {conventionalUnits}");
Console.WriteLine($"Currency {currency}");
Console.WriteLine($"Total {total}");
