using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DeadPlayer : NetworkBehaviour
{
    public GameObject playerGO;
    //ServerNetworkManagerMain serverNetManMain;

    // Start is called before the first frame update
    void Start()
    {
        //serverNetManMain = GameObject.FindGameObjectWithTag("ServerMain").GetComponent<ServerNetworkManagerMain>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            print("GIVEMONEY");
            CmdRespawnPlayer();
        }
    }

    [Command]
    void CmdRespawnPlayer()
    {
        // Cache a reference to the current player object
        GameObject oldPlayer = connectionToClient.identity.gameObject;

        // Instantiate the new player object and broadcast to clients
        NetworkServer.ReplacePlayerForConnection(connectionToClient, Instantiate(playerGO));

        // Remove the previous player object that's now been replaced
        NetworkServer.Destroy(oldPlayer.transform.parent.gameObject);
    }
}
