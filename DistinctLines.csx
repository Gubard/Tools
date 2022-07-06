var file = new FileInfo($"C:\\Users\\{Environment.UserName}\\AppData\\Roaming\\Microsoft\\Windows\\PowerShell\\PSReadLine\\ConsoleHost_history.txt");
var allText = File.ReadAllText(file.FullName);
var lines = allText.Split(Environment.NewLine);
var resultLines = lines.Distinct();
var result = string.Join(Environment.NewLine, resultLines);
file.Delete();
var stream = File.CreateText(file.FullName);

try
{
	stream.Write(result);
}
catch
{
	await stream.DisposeAsync();
	
	throw;
}
