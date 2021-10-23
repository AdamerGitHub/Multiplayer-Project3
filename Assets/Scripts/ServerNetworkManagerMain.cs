using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ServerNetworkManagerMain : NetworkManager
{
    public void ReplacePlayer(NetworkConnection conn, GameObject newPrefab)
    {
        // Cache a reference to the current player object
        GameObject oldPlayer = conn.identity.gameObject;

        // Instantiate the new player object and broadcast to clients
        NetworkServer.ReplacePlayerForConnection(conn, Instantiate(newPrefab));

        // Remove the previous player object that's now been replaced
        NetworkServer.Destroy(oldPlayer);
    }

    /*
    public Transform spawnPoints;
    List<Player> serverPlayerPrefabsList = new List<Player>();

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        // add player at correct spawn position
        Transform randomSpawnPos = spawnPoints.GetChild(Random.Range(0, spawnPoints.childCount));
        GameObject player = Instantiate(playerPrefab, randomSpawnPos.position, randomSpawnPos.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    void RespawnLocalPlayer(NetworkConnection conn)
    {
        
    }
    */
}
