using System.Threading.Tasks;

public class NetworkServer : INetworkServer
{
    FakeSocketProvider _fakeSocketProvider;
    IRedisMessagesListener _listener;

    public void StartListening(FakeSocketProvider fakeSocketProvider)
    {
        _fakeSocketProvider = fakeSocketProvider;
        Task.Run(Listen);
    }

    async Task Listen()
    {
        while (true)
        {
            var socket = new FakeSocketProvider.FakeSocket(_fakeSocketProvider, "Server");
            var client = await socket.AcceptAsync();

            Task.Run(() => DoTransactions(socket, client));
        }
    }

    async Task DoTransactions(FakeSocketProvider.FakeSocket socket, FakeSocketProvider.FakeSocket client)
    {
        var content = await socket.ReceiveAsync(client);

        var message = _listener == null ? content : _listener?.ProcessMessage(content);

        await socket.SendAsync(client, message);
    }

    public void RegisterListener(IRedisMessagesListener listener)
    {
        _listener = listener;
    }

    public void UnregisterListener(IRedisMessagesListener listener)
    {
        _listener = null;
    }

    public void Dispose()
    {
        throw new System.NotImplementedException();
    }
}
