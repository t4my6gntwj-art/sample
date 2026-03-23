using System;

namespace AvaloniaDiApp.Contracts;

public interface IConnectionStatusReceiver
{
    void Receive(byte[] data);
}
