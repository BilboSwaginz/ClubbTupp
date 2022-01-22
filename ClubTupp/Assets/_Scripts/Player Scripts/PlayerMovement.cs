using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerMovement : NetworkBehaviour
{
    public float playerSpeed = 3f;

    public float horizantal;
    public float vertical;
    public Camera mainCamera;
    public Vector3 localTarget;
    public Vector3 localMouseTarget;
    public NetworkVariable<Vector3> target = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> mouseTarget = new NetworkVariable<Vector3>();
    public float angle;
    public Vector2 direction;
    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    private void Update()
    {
        //PlayerMoveKeyboard();
        PlayerMoveMouse();
    }

    public void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, mouseTarget.Value, Time.deltaTime * playerSpeed);
        
    }


    public void PlayerMoveKeyboard()
    {   
        if (IsOwner)
        {
            horizantal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
            localTarget = playerSpeed * Time.deltaTime * new Vector3(horizantal, vertical, 0);
            MoveRequestServerRpc(localTarget);
        }    
        if (NetworkManager.Singleton.IsServer)
        {
            transform.position += target.Value;
        }
    }

    public void PlayerMoveMouse()
    {
        if (IsOwner)
        {
            if (Input.GetMouseButtonDown(1))
            {
                localMouseTarget = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                MouseMoveRequestServerRpc(localMouseTarget);
                SetRotate();
                
            }
           
        }
    }

    public void SetRotate()
    {
        direction = localMouseTarget - transform.position;
        angle = Vector2.Angle(direction, Vector2.down);
        
        if (angle < 22.5)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (angle < 67.5 && angle >= 22.5 && direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(22.5f, 45f, -22.5f);
        }
        if (angle < 67.5 && angle > 22.5 && direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(22.5f, -45f, 22.5f);
        }

        if (angle >= 67.5 && angle < 112.5 && direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(30f, 60f, -45f);
        }
        if (angle >= 67.5 && angle < 112.5 && direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(30f, -60f, 45f);
        }

        if (angle >= 112.5 && angle < 157.5 && direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(60f, 135f, -20f);
        }
        if (angle >= 112.5 && angle < 157.5 && direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(60f, -135f, 20f);
        }
        if (angle >= 157.5)
        {
            transform.rotation = Quaternion.Euler(70f, 180f, 0f);
        }

    }




    [ServerRpc(RequireOwnership = false)]
    void MoveRequestServerRpc(Vector3 newTarget)
    {
        target.Value = newTarget;
    }

    [ServerRpc(RequireOwnership = false)]
    void MouseMoveRequestServerRpc(Vector3 newTarget)
    {
        mouseTarget.Value = newTarget;
    }
}
