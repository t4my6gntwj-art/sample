using System;

namespace AvaloniaDiApp.Contracts;

// 入力ポート：外の世界 (通信部など) から生のバイナリを受け取る
public interface IPowerStatusReceiver
{
    // 生のバイナリデータを受信し、リポジトリ内で解析・変換を行う
    void Receive(byte[] data);
}
