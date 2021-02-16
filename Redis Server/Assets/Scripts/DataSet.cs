public struct DataSet
{
    public DataSet(string key, string val, int expireTime = 0)
    {
        Key = key;
        Value = val;
        ExpireTime = expireTime;
    }

    public string Key;
    public string Value;
    public int ExpireTime;
}
