using System.Text.Json;
using System.Security.Cryptography;

class DataItem
{
	public string Key { get; set; }
	public string Data { get; set; }
}

public static string EncryptString(string key, string plainInput)
{
	byte[] array;

       	using (HashAlgorithm hash = MD5.Create())
 	using (Aes aes = Aes.Create())
 	{
  		aes.Key = hash.ComputeHash(Encoding.UTF8.GetBytes(key));
  		aes.IV = new byte[16];
  		ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
  
		using (MemoryStream memoryStream = new MemoryStream())
  		{
   			using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
   			{
    				using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
    				{	
     					streamWriter.Write(plainInput);
    				}

    				array = memoryStream.ToArray();
   			}
  		}
 	}

 	return Convert.ToBase64String(array);
}

public static string DecryptString(string key, string cipherText)
{
	byte[] buffer = Convert.FromBase64String(cipherText);
 
       	using (HashAlgorithm hash = MD5.Create())
 	using (Aes aes = Aes.Create())
 	{
  		aes.Key = hash.ComputeHash(Encoding.UTF8.GetBytes(key));
  		aes.IV = new byte[16];
  		ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
  
		using (MemoryStream memoryStream = new MemoryStream(buffer))
  		{
   			using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
   			{		
    				using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
    				{
     					return streamReader.ReadToEnd();
    				}
   			}	
  		}
 	}
}

FileInfo fileData = null;
string password = null;
string key = null;
string data = null;

for(var index = 0; index < Args.Count(); index++)
{
	switch(Args[index])
	{
		case "--dataFile":
		{
			fileData = new FileInfo(Args[++index]);
			break;
		}
		case "--password":
		{
			password = Args[++index];
			break;
		}
		case "--key":
		{
			key = Args[++index];
			break;
		}
		case "--data":
		{
			data = Args[++index];
			break;
		}
	}
}


if(!fileData.Exists)
{
	var fileDataStream = fileData.Create();
	await JsonSerializer.SerializeAsync(fileDataStream, new DataItem[0]);	
	await fileDataStream.DisposeAsync();
}

switch(Args[0])
{
	case "list":
	{
		var fileDataStream = fileData.OpenRead();
		var items = await JsonSerializer.DeserializeAsync<DataItem[]>(fileDataStream);
		
		foreach(var item in items)
		{
			Console.WriteLine(item.Key);
		}

		await fileDataStream.DisposeAsync();
		break;
	}
	case "add":
	{
		var fileDataStream = fileData.OpenRead();
		var items = await JsonSerializer.DeserializeAsync<List<DataItem>>(fileDataStream);
		await fileDataStream.DisposeAsync();
		fileData.Delete();
		var item = items.SingleOrDefault(x => x.Key == key);

		if(item is null)
		{
			items.Add(new DataItem
			{
				Key = key,
				Data = EncryptString(password, data)
			});
		}
		else
		{
			item.Data = EncryptString(password, data);
		}

		fileDataStream = fileData.OpenWrite();
		await JsonSerializer.SerializeAsync(fileDataStream, items);
		await fileDataStream.DisposeAsync();
		break;
	}
	case "get":
	{
		var fileDataStream = fileData.OpenRead();
		var items = await JsonSerializer.DeserializeAsync<DataItem[]>(fileDataStream);
		var item = items.Single(x => x.Key == key);
		Console.WriteLine(DecryptString(password, item.Data));
		await fileDataStream.DisposeAsync();
		break;
	}
}
