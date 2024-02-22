using Fusion;

public struct PlayerData : INetworkStruct
{
    [Networked, Capacity(32)]public string Nickname { get => default; set { } }
    public PlayerRef OwnerPlayerPrefs;
    public int KillCount;
    public int DeathCount;
}
