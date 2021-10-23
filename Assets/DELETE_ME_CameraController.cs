using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraController : NetworkBehaviour
{
    public Vector3 rotation = Vector3.zero;
    public float mouseSensitivity;
    public GameObject playerGO;

    public Vector2 minMaxClampRotY = new Vector2(-90, 90);

    [Command]
    void MoveCamera(Vector3 localRotation)
    {
        playerGO.transform.rotation = Quaternion.Euler(0, localRotation.y, 0);
        transform.rotation = Quaternion.Euler(localRotation);
    }

    private void OnPreRender()
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

        rotation.x -= mouseRotation.y;
        rotation.x = Mathf.Clamp(rotation.x, minMaxClampRotY.x, minMaxClampRotY.y);

        rotation.y += mouseRotation.x;

        playerGO.transform.rotation = Quaternion.Euler(0, rotation.y, 0);
        MoveCamera(rotation);
        //myCameraGO.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0);
    }
}
