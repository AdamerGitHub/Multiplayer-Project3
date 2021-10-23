using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{
    [Header("Stats")]
    public int damage = 20;
    public float speed = 5;
    public float destroyAfterTime = 15f;

    [Space]

    public Rigidbody rigidBody;

    [HideInInspector]
    public GameObject myPlayer;
    [HideInInspector]
    public Vector3 direction;

    bool isHitted;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, destroyAfterTime);
    }

    [Server]
    public void ForceDirection(Vector3 direction)
    {
        rigidBody.AddForce(direction * speed);
    }

    /*
    [Server]
    private void OnTriggerEnter(Collider col)
    {
        print("KEK");
        if (col.CompareTag("Player") && col.gameObject != myPlayer && !isHitted)
        {
            RpcDamageHit(col.transform);
            isHitted = true;
            rpcChangeColour(Color.red);
        }
        else if (col.gameObject != myPlayer)
        {
            isHitted = true;
            rpcChangeColour(Color.blue);
        }
    }
    */

    [ClientRpc]
    void RpcDamageHit(Transform damageHitTransform)
    {
        damageHitTransform.GetComponent<Player>().health -= damage;
    }
    
    [Server]
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player") && col.gameObject != myPlayer && !isHitted)
        {
            RpcDamageHit(col.transform);
            isHitted = true;
            rpcChangeColour(Color.blue);
        }
        else if (col.gameObject != myPlayer)
        {
            isHitted = true;
            rpcChangeColour(Color.blue);
        }
    }
    
    [ClientRpc]
    void rpcChangeColour(Color whatAColour)
    {
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color = whatAColour;
    }
}