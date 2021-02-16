using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class DataManager : IDisposable
{
    ConcurrentDictionary<string, DataSet> _data = new ConcurrentDictionary<string, DataSet>();

    public bool Set(string key, string val, out string error)
    {
        try
        {
            var data = new DataSet(key, val);
            return SetData(data, out error);
        }
        catch (Exception e)
        {
            error = e.Message;
            return false;
        }
    }

    public bool Set(string key, string val, int expireTime, out string error)
    {
        try
        {
            var data = new DataSet(key, val, expireTime);

            Task.Run(() => DelayedDelete(key, expireTime));

            return SetData(data, out error);
        }
        catch (Exception e)
        {
            error = e.Message;
            return false;
        }
    }

    async Task DelayedDelete(string key, int expireTime)
    {
        await Task.Delay(expireTime * 1000);

        string error;
        if(Delete(key, out error))
        {
            throw new Exception(error);
        }
    }

    public int Incr(string key, out string error)
    {
        error = string.Empty;
        try
        {
            int val = 1;
            if (_data.ContainsKey(key))
            {
                DataSet data;
                if (_data.TryGetValue(key, out data))
                {
                    if (int.TryParse(data.Value, out val))
                    {
                        val++;

                        data.Value = val.ToString();
                        _data[data.Key] = data;

                        return val;
                    }
                    else
                    {
                        error = "A value already exists that is not an integer";
                        return 0;
                    }
                }
                else
                {
                    error = $"Unexpected Exception when trying to access the key {key}.";
                    return 0;
                }
            }
            else
            {
                var data = new DataSet(key, val.ToString());
                if (!_data.TryAdd(data.Key, data))
                {
                    error = $"Unexpected Exception when trying to set the value {data.Value} to the field {data.Key}.";
                    return 0;
                }
                return val;
            }
        }
        catch (Exception e)
        {
            error = e.Message;
            return 0;
        }
    }

    bool SetData(DataSet data, out string error)
    {
        try
        {
            if (_data.ContainsKey(data.Key))
            {
                _data[data.Key] = data;
            }
            else
            {
                if (!_data.TryAdd(data.Key, data))
                {
                    error = $"Unexpected Exception when trying to set the value {data.Value} to the field {data.Key}.";
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            error = e.Message;
            return false;
        }
        error = string.Empty;
        return true;
    }

    public string Get(string key, out string error)
    {
        try
        {
            DataSet data;
            if (_data.TryGetValue(key, out data))
            {
                error = string.Empty;
                return data.Value;
            }
            else
            {
                error = $"Could not find any record with the key {data.Key}.";
                return string.Empty;
            }
        }
        catch (Exception e)
        {
            error = e.Message;
            return string.Empty;
        }

    }

    public bool Delete(string key, out string error)
    {
        try
        {
            if(_data.ContainsKey(key))
            {
                DataSet data;
                if(_data.TryRemove(key, out data))
                {
                    error = string.Empty;
                    return true;
                }
                else
                {
                    error = $"Unexpected Exception when trying to delete record with key {key}.";
                    return false;
                }
            }
            else
            {
                error = $"Could not find any record with the key {key}.";
                return false;
            }
        }
        catch (Exception e)
        {
            error = e.Message;
            return false;
        }
    }

    public int Count(out string error)
    {
        var count = 0;
        try
        {
            count = _data.Count;
        }
        catch (Exception e)
        {
            error = e.Message;
            return -1;
        }

        error = string.Empty;
        return count;
    }

    public void Dispose()
    {
        _data.Clear();
        _data = null;
    }
}
