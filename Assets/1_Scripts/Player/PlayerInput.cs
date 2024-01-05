using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;

public class PlayerInput : NetworkBehaviour, INetworkRunnerCallbacks
{
    public override void Spawned()
    {
        Runner.AddCallbacks(this);
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (Object.HasInputAuthority == false)
            return;

        PlayerInputData inputData = UpdateInputData();

        input.Set(inputData);
    }

    private PlayerInputData UpdateInputData()
    {
        PlayerInputData input = default;

        if (Input.GetKey(KeyCode.W))
        {
            input.buttons |= PlayerInputData.BUTTON_FORWARD;
        }

        if (Input.GetKey(KeyCode.S))
        {
            input.buttons |= PlayerInputData.BUTTON_BACKWARD;
        }

        if (Input.GetKey(KeyCode.A))
        {
            input.buttons |= PlayerInputData.BUTTON_LEFT;
        }

        if (Input.GetKey(KeyCode.D))
        {
            input.buttons |= PlayerInputData.BUTTON_RIGHT;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            input.buttons |= PlayerInputData.BUTTON_JUMP;
        }

        Vector2 inputRotateDelta = new Vector2(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"));

        if(inputRotateDelta.magnitude != 0)
        {
            input.RotationDelta = inputRotateDelta;
        }

        return input;
    }

    #region Unused Network Callback Method
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    #endregion
}
