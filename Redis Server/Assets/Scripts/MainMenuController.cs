
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    const string HostAddressKey = "HostAddressKey";
    const string HostPortKey = "HostPortKey";

    RedisServerService _redisServerService;
    RedisClientService _redisClientService;

    [Header("Host")]
    [SerializeField]
    InputField _inputHost;
    [SerializeField]
    InputField _inputPort;

    [Header("SET")]
    [SerializeField]
    InputField _inputSetKey;
    [SerializeField]
    InputField _inputSetValue;

    [Header("SETEX")]
    [SerializeField]
    InputField _inputSetExKey;
    [SerializeField]
    InputField _inputSetExValue;
    [SerializeField]
    InputField _inputSetExSecs;

    [Header("GET")]
    [SerializeField]
    InputField _inputGetKey;
    [SerializeField]
    Text _textGetValue;

    [Header("DEL")]
    [SerializeField]
    InputField _inputDelKey;

    [Header("DBSize")]
    [SerializeField]
    Text _textDBSizeValue;

    [Header("INCR")]
    [SerializeField]
    InputField _inputIncrKey;
    [SerializeField]
    Text _textIncrValue;

    [Header("Response")]
    [SerializeField]
    Text _textResponseValue;

    string HostAddress { get => _inputHost.text; }
    int HostPort { get => int.Parse(_inputPort.text); }

    private void Start()
    {
        _inputHost.text = PlayerPrefs.GetString(HostAddressKey, "127.0.0.1");
        _inputPort.text = PlayerPrefs.GetInt(HostPortKey, 6379).ToString();

        _redisServerService = new RedisServerService();

        Task.Run(OnConnect);
    }

    private void OnDestroy()
    {
        _redisClientService?.Dispose();
    }

    public void DoConnect()
    {
        Task.Run(OnConnect);
    }

    public async Task OnConnect()
    {
        var provider = new FakeSocketProvider();

        _redisClientService = new RedisClientService(provider);

        var connected = await _redisServerService.Connect(provider);

        if(connected)
        {
            _textResponseValue.text = $"Successfully connected to: {HostAddress}:{HostPort}";
            Debug.Log(_textResponseValue.text);
        }
        else
        {
            _textResponseValue.text = $"Connection failed";
            Debug.LogError(_textResponseValue.text);
        }
    }

    public void DoSet()
    {
        OnSet();
    }

    public async Task OnSet()
    {
        var response = await _redisClientService.Set(_inputSetKey.text, _inputSetValue.text);
        ProcessMessage("SET", response);
    }

    public void DoSetEx()
    {
        OnSetEx();
    }

    public async Task OnSetEx()
    {
        int secs = 0;
        if(int.TryParse(_inputSetExSecs.text, out secs))
        {
            var response = await _redisClientService.SetEx(_inputSetExKey.text, _inputSetExValue.text, secs);
            ProcessMessage("SETEX", response);
        }
        else
        {
            _textResponseValue.text = $"SETEX Response: Value at the seconds field is not a number";
        }
    }

    public void DoGet()
    {
        OnGet();
    }

    public async Task OnGet()
    {
        var response = await _redisClientService.Get(_inputGetKey.text);
        _textGetValue.text = response;
        ProcessMessage("GET", response);
    }

    public void DoDel()
    {
        OnDel();
    }

    public async Task OnDel()
    {
        var response = await _redisClientService.Del(_inputDelKey.text);
        ProcessMessage("DEL", response);
    }

    public void DoDBSize()
    {
        OnDBSize();
    }

    public async Task OnDBSize()
    {
        var response = await _redisClientService.DBSize();
        _textDBSizeValue.text = response;
        ProcessMessage("DBSIZE", response);
    }

    public void DoIncr()
    {
        OnIncr();
    }

    public async Task OnIncr()
    {
        var response = await _redisClientService.Incr(_inputIncrKey.text);
        _textIncrValue.text = response;
        ProcessMessage("INCR", response);
    }

    void ProcessMessage(string command, string message)
    {
        Debug.Log(message);
        _textResponseValue.text = $"{command} Response: {message}";
    }
}