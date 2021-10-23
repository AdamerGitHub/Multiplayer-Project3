using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [Header("Gun")]
    public int whatAWeaponInt;
    public GameObject[] weaponModels;
    public GameObject projectilePrefab;
    public CameraWeaponHolder cameraWeaponHolder;
    public GameObject currentPlayerWeaponGO;

    //On Future, will be hiden, for players
    [Header("Gun Parameters")]
    public int weaponDamage;
    public float weaponRateOfFire;
    public float weaponReloadTime;
    public float weaponAccuracy;

    [HideInInspector] public Transform gunHolder;
    Weapon playerWeapon;

    [Header("Stats")]
    public int health;
    public float moveSpeed = 500;
    public float maxMoveSpeed = 2000f;
    public float stoppingMoveSpeed = 50f;
    public float jumpPower = 500f;

    [Header("Camera")]
    public Vector2 minMaxClampRotY = new Vector2(-90, 90);
    public float mouseSensitivity;
    public GameObject playerCameraPrefab;
    //public GameObject myCameraGO;
    //public Camera myPlayerCamera;
    //public AudioListener myAudioListener;
    // GameObject testMyCameraGO;
    [HideInInspector]
    public GameObject playerCameraGO;
    Camera testMyPlayerCamera;
    AudioListener testMyAudioListener;

    Camera mainCamera;

    [Header("Another Components")]
    public GameObject deadPlayerGO;

    Rigidbody rb;

    ServerNetworkManagerMain serverNetManMain;
    Vector3 playerRotation = Vector3.zero;

    // IEnumerators

    IEnumerator movementCorotine;
    IEnumerator cameraCorotine;
    IEnumerator shootCorotine;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("НОВЫЕ БАГИ, ЛЮБЛЮ НОВЫЕ БАГИИИИИИИ");
        if (isLocalPlayer)
        {
            //Debug.Log("НОВЫЕ БАГИ, ЛЮБЛЮ НОВЫЕ БАГИИИИИИИ");

            playerCameraGO = Instantiate(playerCameraPrefab, transform.position, Quaternion.identity);

            testMyPlayerCamera = playerCameraGO.GetComponent<Camera>();
            testMyAudioListener = playerCameraGO.GetComponent<AudioListener>();
            cameraWeaponHolder = playerCameraGO.GetComponent<CameraWeaponHolder>();

            gunHolder = playerCameraGO.transform.GetChild(0);
            currentPlayerWeaponGO = Instantiate(weaponModels[whatAWeaponInt], gunHolder.position, gunHolder.rotation, gunHolder);
            playerWeapon = currentPlayerWeaponGO.GetComponent<Weapon>();

            rb = gameObject.GetComponent<Rigidbody>();
        }

        if (isLocalPlayer)
        {
            CmdSpawnCameraGO();
            //NetworkServer.Spawn(playerCameraGO);
            //NetworkServer.Spawn(currentPlayerWeaponGO);
        }

        if (isLocalPlayer)
        {
            if(Camera.main != null)
            {
                mainCamera = Camera.main;

                mainCamera.GetComponent<AudioListener>().enabled = false;
                mainCamera.enabled = false;
            }

            Cursor.lockState = CursorLockMode.Locked;

            testMyPlayerCamera.enabled = true;
            testMyAudioListener.enabled = true;

            movementCorotine = MovementCor();
            // cameraCorotine = CameraCor();
            shootCorotine = ShootCor();

            StartCoroutine(movementCorotine);
            // StartCoroutine(cameraCorotine);
            StartCoroutine(shootCorotine);
        }
    }

    /*
    [ClientRpc]
    void RpcMakeRed()
    {
        transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color = Color.red;
    }
    */

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            if (health <= 0)
            {
                print("DEAD");

                // TODO
                transform.GetChild(0).gameObject.GetComponent<MeshRenderer>().material.color = Color.red;

                //mainCamera.enabled = true;
                //mainCamera.GetComponent<AudioListener>().enabled = true;
                NetworkServer.Destroy(transform.GetComponent<Player>().playerCameraGO);
                CmdRespawnMyPlayer();

                //serverNetManMain.ReplacePlayer(connectionToServer, deadPlayerGO);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                health = 0;
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                CmdRespawnMyPlayer();
            }
        }

        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.Escape) && Cursor.lockState == CursorLockMode.None)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tilde))
            {
                Cursor.lockState = CursorLockMode.None;
            }

            Vector2 mouseRotation = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime, Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime);

            playerRotation.x -= mouseRotation.y;
            playerRotation.x = Mathf.Clamp(playerRotation.x, minMaxClampRotY.x, minMaxClampRotY.y);

            playerRotation.y += mouseRotation.x;

            transform.rotation = Quaternion.Euler(0, playerRotation.y, 0);
            //myCameraGO.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
            playerCameraGO.transform.rotation = Quaternion.Euler(playerRotation.x, playerRotation.y, 0);
            playerCameraGO.transform.position = rb.position;

            currentPlayerWeaponGO.transform.parent = playerCameraGO.transform;

            //MoveCamera(new Vector3(playerRotation.x, playerRotation.y, 0));
        }
    }

    IEnumerator MovementCor()
    {
        if (isLocalPlayer)
        {
            Vector3 input = new Vector3 { };
            Vector3 inputNorm = new Vector3 { };

            while (true)
            {
                input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
                inputNorm = input.normalized;

                //rb.velocity -= rb.velocity * 0.5f * Time.deltaTime;

                //rb.velocity = inputNorm.x * moveSpeed * Time.deltaTime * transform.right;
                //rb.velocity += inputNorm.z * moveSpeed * Time.deltaTime * transform.forward;

                //SetMaxPlayerSpeed();

                //if (input == Vector3.zero)
                {
                    //if(rb.velocity.magnitude >= 5)
                        //rb.velocity -= rb.velocity.normalized * stoppingMoveSpeed / rb.velocity.magnitude;
                }

                rb.position += inputNorm.x * moveSpeed * Time.deltaTime * transform.right;
                rb.position += inputNorm.z * moveSpeed * Time.deltaTime * transform.forward;

                // transform.position += inputNorm.x * moveSpeed * Time.deltaTime * transform.right;
                // transform.position += inputNorm.z * moveSpeed * Time.deltaTime * transform.forward;

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    rb.AddForce(Vector3.up * jumpPower);
                }

                yield return null;
            }
        }
    }

    void SetMaxPlayerSpeed()
    {
        if (rb.velocity.magnitude >= maxMoveSpeed)
            rb.velocity = rb.velocity.normalized * maxMoveSpeed;
    }

    // Just draw movement
    void OnDrawGizmos()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 rightStabilized = transform.right;
        Vector3 forwardStabilized = transform.forward;

        rightStabilized.y = 0; // Remove "flying up" by set zero y value (that control up and down)
        rightStabilized = rightStabilized.normalized;

        forwardStabilized.y = 0; // Remove "flying up" by set zero y value (that control up and down)
        forwardStabilized = forwardStabilized.normalized;

        // If double keys are pressed, make correction
        if (inputX != 0 && inputX != 0)
        {
            inputX *= 0.75f;
            inputY *= 0.75f;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + ((inputX * rightStabilized) + (inputY * forwardStabilized)) * 5);
    }

    [Command]
    void MoveCamera(Vector3 localRotation)
    {
        transform.rotation = Quaternion.Euler(0, localRotation.y, 0);
        playerCameraGO.transform.rotation = Quaternion.Euler(localRotation);
        playerCameraGO.transform.position = rb.position;

        currentPlayerWeaponGO.transform.rotation = gunHolder.rotation;
        currentPlayerWeaponGO.transform.position = gunHolder.position;
    }

    /* IEnumerator CameraCor()
    {
        if (isLocalPlayer)
        {
            //myPlayerCamera.enabled = true;
            //myAudioListener.enabled = true;

            testMyPlayerCamera.enabled = true;
            testMyAudioListener.enabled = true;

            Vector2 mouseRotation = Vector2.zero;
            Vector2 rotation = Vector2.zero;

            while (true)
            {
                if (Input.GetKeyDown(KeyCode.Escape) && Cursor.lockState == CursorLockMode.None)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tilde))
                {
                    Cursor.lockState = CursorLockMode.None;
                }

                mouseRotation = new Vector2(Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime, Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime);

                rotation.x -= mouseRotation.y;
                rotation.x = Mathf.Clamp(rotation.x, minMaxClampRotY.x, minMaxClampRotY.y);

                rotation.y += mouseRotation.x;

                transform.rotation = Quaternion.Euler(0, rotation.y, 0);
                //myCameraGO.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
                playerCameraGO.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
                playerCameraGO.transform.position = rb.position;

                MoveCamera(new Vector3(rotation.x, rotation.y, 0));

                yield return null;
            }
        }
    } */

    IEnumerator ShootCor()
    {
        if (isLocalPlayer)
        {
            while (true)
            {
                if (!isLocalPlayer)
                    yield return null;

                if (Input.GetMouseButton(0))
                {
                    CmdFire();
                    yield return new WaitForSeconds(0.2f);
                }
                yield return null;
            }
        }
    }

    [Command]
    void CmdSpawnCameraGO()
    {
        NetworkServer.Spawn(playerCameraGO);
        NetworkServer.Spawn(currentPlayerWeaponGO);

        currentPlayerWeaponGO.transform.parent = gunHolder;
    }

    [Command]
    void CmdRespawnMyPlayer()
    {
        // Cache a reference to the current player object
        GameObject oldPlayer = connectionToClient.identity.gameObject;

        // Instantiate the new player object and broadcast to clients
        NetworkServer.ReplacePlayerForConnection(connectionToClient, Instantiate(deadPlayerGO, transform.position, transform.rotation));

        // Remove the previous player object that's now been replaced
        NetworkServer.Destroy(oldPlayer.transform.GetComponent<Player>().playerCameraGO);
        NetworkServer.Destroy(oldPlayer);
    }

    [Command]
    void CmdFire()
    {
        GameObject myProjectile = Instantiate(projectilePrefab, transform.position + playerCameraGO.transform.forward, playerCameraGO.transform.rotation);
        Vector3 direction = playerCameraGO.transform.rotation * Vector3.forward;

        Projectile projectileScript = myProjectile.GetComponent<Projectile>();

        projectileScript.ForceDirection(direction);
        projectileScript.myPlayer = gameObject;
        projectileScript.direction = direction;

        NetworkServer.Spawn(myProjectile);
        // Debug.Log(playerCameraGO);
    }

}