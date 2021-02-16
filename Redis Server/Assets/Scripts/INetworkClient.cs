using System;
using System.Threading.Tasks;

public interface INetworkClient : IDisposable
{
    Task Connect(FakeSocketProvider provider);

    bool IsConnected { get; }

    Task<string> ReceiveAsync();

    Task<string> SendAsync(string message);

    void Disconnect();
}
