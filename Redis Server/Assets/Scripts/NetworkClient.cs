using System.Threading.Tasks;

public class NetworkClient : INetworkClient
{
    FakeSocketProvider.FakeSocket _socket;
    public bool IsConnected => _socket != null && _socket.IsConnected;

    public async Task Connect(FakeSocketProvider provider)
    {
        _socket = new FakeSocketProvider.FakeSocket(provider, "Client"); // Name is set for test purposes
        await _socket.ConnectAsync();
    }

    public async Task<string> ReceiveAsync()
    {
        return await _socket.ReceiveAsync(_socket.ConnectedSocket);
    }

    public async Task<string> SendAsync(string message)
    {
        var response = await _socket.SendAsync(_socket.ConnectedSocket, message);

        return response;
    }

    public void Disconnect()
    {
        _socket.Close();
    }

    public void Dispose()
    {
        _socket.Dispose();
    }
}
