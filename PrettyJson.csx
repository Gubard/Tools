using System.Text.Json;

async Task<TObject> DeserializeJsonAsync<TObject>(FileInfo file)
{
	await using var stream = file.OpenRead();
	var result = await JsonSerializer.DeserializeAsync<TObject>(stream);
	
	return result;
}

async Task SaveJsonFileAsync<TObject>(FileInfo file, TObject obj, JsonSerializerOptions options)
{
	await using var stream = new StreamWriter(file.FullName, false);
	await JsonSerializer.SerializeAsync(stream.BaseStream, obj, options);
}

var fileJson = new FileInfo(Args[0]);
var options = new JsonSerializerOptions()
{
	WriteIndented = true
};
var jsonElement = await DeserializeJsonAsync<JsonElement>(fileJson);
await SaveJsonFileAsync(fileJson, jsonElement, options);
