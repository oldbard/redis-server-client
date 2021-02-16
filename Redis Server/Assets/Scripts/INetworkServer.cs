using System;
using System.Threading.Tasks;

public interface INetworkServer : IDisposable
{
    void StartListening(FakeSocketProvider fakeSocketProvider);

    void RegisterListener(IRedisMessagesListener listener);
    
    void UnregisterListener(IRedisMessagesListener listener);
}

public interface IRedisMessagesListener
{
    string ProcessMessage(string message);

    string ProcessError(string message);

}
