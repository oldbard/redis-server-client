using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class FakeSocketProvider
{
    public class FakeSocket : IDisposable
    {
        FakeSocketProvider _fakeSocketProvider;

        public FakeSocket ConnectedSocket;
        public QueuedMessage QueuedMessage;

        // For debug purposes
        public string Name;

        public bool IsConnected => ConnectedSocket != null;

        public Func<string, string> MessageReceivedParsed;

        public FakeSocket(FakeSocketProvider fakeSocketProvider, string name = default(string))
        {
            Name = name;
            _fakeSocketProvider = fakeSocketProvider;
        }

        public async Task<FakeSocket> AcceptAsync()
        {
            ConnectedSocket = await _fakeSocketProvider.Accept(this);
            return ConnectedSocket;
        }

        public async Task ConnectAsync()
        {
            ConnectedSocket = await _fakeSocketProvider.Connect(this);
        }

        public async Task<string> ReceiveAsync(FakeSocket sender)
        {
            var response = await _fakeSocketProvider.Receive(sender, this);
            return response;
        }

        public async Task<string> SendAsync(FakeSocket receiver, string message)
        {
            QueuedMessage = new QueuedMessage();
            QueuedMessage.Target = receiver;
            QueuedMessage.Message = message;
            QueuedMessage.Received = false;
            await _fakeSocketProvider.Send(this);
            return "OK";
        }

        public override string ToString()
        {
            return Name.ToString();
        }

        public void Close()
        {
            ConnectedSocket = null;
        }

        public void Dispose()
        {
            Close();
        }
    }

    public class QueuedMessage
    {
        public FakeSocket Target;
        public string Message;
        public bool Received;
    }

    ConcurrentQueue<FakeSocket> _pendingConnections = new ConcurrentQueue<FakeSocket>();

    public async Task<FakeSocket> Accept(FakeSocket socket)
    {
        while (_pendingConnections.Count < 1)
        {
            await Task.Delay(100);

            FakeSocket target = null;
            if(_pendingConnections.TryDequeue(out target))
            {
                target.ConnectedSocket = socket;
                return target;
            }
        }

        return null;
    }

    public async Task<FakeSocket> Connect(FakeSocket socket)
    {
        _pendingConnections.Enqueue(socket);
        while (!socket.IsConnected)
        {
            await Task.Delay(100);
        }
        return socket.ConnectedSocket;
    }

    public async Task<string> Send(FakeSocket sender)
    {
        while (!sender.QueuedMessage.Received)
        {
            await Task.Delay(10);
        }
        sender.QueuedMessage = null;
        return "OK";
    }

    public async Task<string> Receive(FakeSocket sender, FakeSocket receiver)
    {
        while (sender.QueuedMessage == null || sender.QueuedMessage.Target != receiver)
        {
            await Task.Delay(10);
        }
        sender.QueuedMessage.Received = true;
        return sender.QueuedMessage.Message;
    }
}
