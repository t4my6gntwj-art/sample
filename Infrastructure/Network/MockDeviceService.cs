using System;
using System.Threading.Tasks;
using AvaloniaDiApp.Contracts;

namespace AvaloniaDiApp.Infrastructure.Network;

// 外部機器をシミュレートするクラス
public class MockDeviceService
{
    private readonly INetworkTransport _transport;

    public MockDeviceService(INetworkTransport transport, NetworkMessageDispatcher dispatcher)
    {
        _transport = transport;
        
        // 【一本化】配送センター経由でデータを受け取る
        dispatcher.MessageReady += async (data) => await HandleRequest(data);
    }

    private async Task HandleRequest(byte[] data)
    {
        if (data == null || data.Length == 0) return;

        byte cmdId = data[0];

        // 特定のコマンドに対して、少し遅れてレスポンスを返す（本物っぽく）
        switch (cmdId)
        {
            case 0x10: // 「初期化しろ」
                await Task.Delay(500);
                _transport.Send(new byte[] { 0x11, 0x00 }); // 「初期化完了(0x11)」を返す
                break;
                
            case 0x20: // 「設定しろ」
                await Task.Delay(800);
                _transport.Send(new byte[] { 0x21, 0xFF }); // 「設定完了(0x21)」を返す
                break;
        }
    }
}
