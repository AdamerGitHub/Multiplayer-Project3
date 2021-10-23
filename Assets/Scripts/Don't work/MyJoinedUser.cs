using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MyJoinedUser : NetworkBehaviour
{
    public Transform mySpawnPointsHolder;
    public GameObject myPlayerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        mySpawnPointsHolder = GameObject.FindGameObjectWithTag("SpawnPointsHolder").gameObject.transform;

        Transform randomSpawnPoint = mySpawnPointsHolder.GetChild(Random.Range(0, mySpawnPointsHolder.childCount));
        GameObject mySpawnedPlayer = Instantiate(myPlayerPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation, transform);

        NetworkServer.Spawn(mySpawnedPlayer, gameObject);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
