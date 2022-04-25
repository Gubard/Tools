var directory = new DirectoryInfo(Args[0]);
var files = directory.GetFiles();

foreach(var file in files)
{
	File.Move(file.FullName, Path.Combine(directory.FullName, $"{Guid.NewGuid()}.mp4"));
}
