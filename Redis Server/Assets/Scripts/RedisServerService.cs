using System;
using System.Threading.Tasks;

public class RedisServerService : IRedisMessagesListener, IDisposable
{
	#region Properties
	INetworkServer _server;

	DataManager _dataManager = new DataManager();

	public Action<string> MessageReceived;
	public Action<string> MessageSent;
	#endregion

	public RedisServerService()
	{
		_server = new NetworkServer();
	}

	public RedisServerService(INetworkServer server)
	{
		_server = server;
	}

	#region INetworkServer Communication
	public async Task<bool> Connect(FakeSocketProvider fakeSocketProvider)
	{
		if(_server == null)
		{
			_server = new NetworkServer();
		}
		_server.RegisterListener(this);
		((NetworkServer)_server).StartListening(fakeSocketProvider);

		return true;
	}
	#endregion

	#region Utils
	string ParseMessage(string message)
	{
		message = message.Trim().Replace("\r\n", "");

		MessageReceived?.Invoke(message);

		var data = message.Split(' ');

		if (data.Length > 0)
		{
			switch(data[0].ToUpper())
			{
				case "SET":
					if (data.Length > 2)
					{
						var key = data[1];
						var val = data[2];

						return OnSet(key, val);
					}
					else
					{
						return "Missing Paramenters. Expected Key and Value parameters";
					}
				case "SETEX":
					if (data.Length > 3)
					{
						var key = data[1];
						var val = data[3];
						int seconds;
						if(int.TryParse(data[2], out seconds))
						{
							return OnSetEx(key, val, seconds);
						}
						else
						{
							return $"Parameter {data[2]} is not a valid integer.";
						}
					}
					else
					{
						return "Missing Paramenters. Expected Key, Value and Expire Time parameters";
					}
				case "GET":
					if (data.Length > 1)
					{
						var key = data[1];

						return OnGet(key);
					}
					else
					{
						return "Missing Paramenters. Expected Key parameter";
					}
				case "DEL":
					if (data.Length > 1)
					{
						var key = data[1];

						return OnDel(key);
					}
					else
					{
						return "Missing Paramenters. Expected Key parameter";
					}
				case "DBSIZE":
					return OnDBSize();
				case "INCR":
					if (data.Length > 1)
					{
						var key = data[1];

						return OnIncr(key);
					}
					else
					{
						return "Missing Paramenters. Expected Key parameter";
					}
				default:
					return "Invalid Command";
			}
		}
		else
		{
			return "Missing command and parameter(s)";
		}
	}

	string OnSet(string key, string val)
	{
		string error = string.Empty;
		bool success = _dataManager.Set(key, val, out error);

		if(!success || !string.IsNullOrEmpty(error))
		{
			return ProcessResponse($"-Error {error}");
		}
		return ProcessResponse("OK");
	}

	string OnSetEx(string key, string val, int expireTime)
	{
		string error = string.Empty;
		bool success = _dataManager.Set(key, val, expireTime, out error);

		if (!success || !string.IsNullOrEmpty(error))
		{
			return ProcessResponse($"-Error {error}");
		}
		return ProcessResponse("OK");
	}

	string OnIncr(string key)
	{
		string error = string.Empty;
		var result = _dataManager.Incr(key, out error);

		if (result == 0 || !string.IsNullOrEmpty(error))
		{
			return ProcessResponse($"-Error {error}");
		}
		return ProcessResponse(result.ToString());
	}

	string OnGet(string key)
	{
		string error = string.Empty;
		var val = _dataManager.Get(key, out error);

		if (!string.IsNullOrEmpty(error))
		{
			return ProcessResponse($"-Error {error}");
		}
		return val;
	}

	string OnDBSize()
	{
		string error = string.Empty;
		var val = _dataManager.Count(out error);

		if (!string.IsNullOrEmpty(error))
		{
			return ProcessResponse($"-Error {error}");
		}
		return val.ToString();
	}

	string OnDel(string key)
	{
		string error = string.Empty;
		var success = _dataManager.Delete(key, out error);

		if (!success || !string.IsNullOrEmpty(error))
		{
			return ProcessResponse($"-Error {error}");
		}
		return ProcessResponse("OK");
	}

	string ProcessResponse(string response)
	{
		MessageSent?.Invoke(response);
		return response;
	}

	#endregion

	#region INetworkMessagesListener

	public string ProcessMessage(string message)
	{
		return ParseMessage(message);
	}

	public string ProcessError(string message)
	{
		return $"-Error {message}";
	}

	#endregion

	public void Dispose()
	{
		MessageReceived = null; ;
		MessageSent = null;;
		
		_server.Dispose();
		_server = null;
	}
}
