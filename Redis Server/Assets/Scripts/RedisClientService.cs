using System;
using System.Threading.Tasks;

public class RedisClientService : IDisposable
{
	const string ErrorNotConnected = "This client is not connected to a Server.";

	#region Properties
	INetworkClient _client;

	FakeSocketProvider _fakeSocketProvider;

	public bool IsConnected => _client.IsConnected;
	#endregion

	public RedisClientService(FakeSocketProvider fakeSocketProvider)
	{
		_fakeSocketProvider = fakeSocketProvider;
		_client = new NetworkClient();
	}

	#region INetworkClient Communication
	public async Task<bool> Connect()
	{
		if (_client == null)
		{
			_client = new NetworkClient();
		}
		await ((NetworkClient)_client).Connect(_fakeSocketProvider);
		return true;
	}

	async Task<string> SendCommand(string command)
	{
		await ((NetworkClient)_client).SendAsync(command + "\r\n");

		var response = await ((NetworkClient)_client).ReceiveAsync();

		var parsedResponse = ParseResponse(response);

		return parsedResponse;
	}

	async Task<string> SendCommand(string command, params object[] args)
	{
		string fullCommand = command + " ";
		foreach (object arg in args)
		{
			string argStr = arg.ToString();
			fullCommand += argStr + " ";
		}
		fullCommand += "\r\n";

		await ((NetworkClient)_client).SendAsync(fullCommand);

		var response = await ((NetworkClient)_client).ReceiveAsync();

		var parsedResponse = ParseResponse(response);

		return parsedResponse;
	}
	#endregion

	#region Commands
	public async Task<string> DBSize()
	{
		await Connect();
		if (IsConnected)
		{
			return await SendCommand("DBSIZE");
		}
		else
		{
			return ErrorNotConnected;
		}
	}

	public async Task<string> Set(string key, string val)
	{
		await Connect();
		if (IsConnected)
		{
			var response = await SendCommand("SET", key, $"\"{val}\"");
			((NetworkClient)_client).Disconnect();
			return response;
		}
		else
		{
			return ErrorNotConnected;
		}

	}

	public async Task<string> SetEx(string key, string val, int seconds)
	{
		await Connect();
		if (IsConnected)
		{
			var response = await SendCommand("SETEX", key, seconds, $"\"{val}\"");
			((NetworkClient)_client).Disconnect();
			return response;
		}
		else
		{
			return ErrorNotConnected;
		}
	}

	public async Task<string> Get(string key)
	{
		await Connect();
		if (IsConnected)
		{
			var response = await SendCommand("GET", key);
			((NetworkClient)_client).Disconnect();
			return response;
		}
		else
		{
			return ErrorNotConnected;
		}
	}

	public async Task<string> Del(string key)
	{
		await Connect();
		if (IsConnected)
		{
			var response = await SendCommand("DEL", key);
			((NetworkClient)_client).Disconnect();
			return response;
		}
		else
		{
			return ErrorNotConnected;
		}
	}

	public async Task<string> Incr(string key)
	{
		await Connect();
		if (IsConnected)
		{
			var response = await SendCommand("INCR", key);
			((NetworkClient)_client).Disconnect();
			return response;
		}
		else
		{
			return ErrorNotConnected;
		}
	}

	public async Task<string> Keys(string param)
	{
		await Connect();
		if (IsConnected)
		{
			var response = await SendCommand("KEYS", $"\"{param}\"");
			((NetworkClient)_client).Disconnect();
			return response;
		}
		else
		{
			return ErrorNotConnected;
		}
	}
	#endregion

	#region Utils
	string ParseResponse(string response)
	{
		response = response.Trim().Replace("\r\n", " ").Replace(":", "").Replace("+", "");

		if (string.CompareOrdinal(response, "$-1") == 0)
		{
			return string.Empty;
		}
		if (response.Contains("-ERR"))
		{
			return $"An error ocurred: {response.Replace("-ERR ", "")}";
		}
		if (response.Contains("$") && response.Contains(" "))
		{
			var spacePos = response.IndexOf(" ") + 1;
			return response.Substring(spacePos, response.Length - spacePos);
		}
		if (response.Contains(":") && response.Contains(" "))
		{
			var spacePos = response.IndexOf(" ") + 1;
			return response.Substring(spacePos, response.Length - spacePos);
		}

		return response;
	}
    #endregion

    public void Dispose()
	{
		_client.Dispose();
		_client = null;
	}
}
