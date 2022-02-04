using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerMovement : NetworkBehaviour
{
    public float playerSpeed = 4f;

    public Camera mainCamera;
    public Vector3 localMouseTarget;

    public NetworkVariable<Vector3> mouseTarget = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> playerRotation = new NetworkVariable<Vector3>();
    public LayerMask terrain;
    public float angle;
    public Vector2 direction;
    public float angleLook;
    public Vector2 directionLook;

    public Animator playerAnimator;
    public bool isWalking;
    

    private void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        localMouseTarget = transform.position + new Vector3(0, 0, -10);
        if (IsServer)
        {
            mouseTarget.Value = transform.position + new Vector3(0, 0, -10);
        }
        if (!IsOwner)
        {
            transform.position = mouseTarget.Value;
        }
    }

    private void Update()
    {
        PlayerMoveMouse();
        
        directionLook = mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        angleLook = Vector2.Angle(directionLook, Vector2.down);
        if (IsOwner && !isWalking)
        {
            SetRotateServerRpc(angleLook, directionLook);
        }
        if (Physics2D.Raycast(transform.position, mouseTarget.Value - transform.position + new Vector3(0, 0, 10), 0.2f, terrain))
        {
            localMouseTarget = transform.position + new Vector3(0, 0, -10);
            MouseMoveRequestServerRpc(transform.position + new Vector3(0, 0, -10));
            playerAnimator.SetBool("IsWalking", false);
            isWalking = false;
        }
    }

    public void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, mouseTarget.Value, Time.deltaTime * playerSpeed);
        transform.rotation = Quaternion.Euler(playerRotation.Value);
        
    }


    

    public void PlayerMoveMouse()
    {
        if (IsOwner)
        {
            if ((Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0)) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && 
                !Physics2D.Raycast(transform.position, mainCamera.ScreenToWorldPoint(Input.mousePosition) - transform.position + new Vector3(0, 0, 10), 0.2f, terrain))
            {
                localMouseTarget = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                MouseMoveRequestServerRpc(localMouseTarget);
                direction = localMouseTarget - transform.position;
                angle = Vector2.Angle(direction, Vector2.down);
                SetRotateServerRpc(angle,direction);
                
            }
           
        }
        if ((transform.position-(mouseTarget.Value + new Vector3(0,0,10))).magnitude > 0.1)
        {
            playerAnimator.SetBool("IsWalking", true);
            isWalking = true;
        }
        else
        {
            playerAnimator.SetBool("IsWalking", false);
            isWalking = false;
        }
    }

    [ServerRpc]
    public void SetRotateServerRpc(float angle, Vector2 direction)
    {
        
        if (angle < 22.5)
        {
            playerRotation.Value = new Vector3(0, 0, 0);
        }

        if (angle < 67.5 && angle >= 22.5 && direction.x < 0)
        {
            playerRotation.Value = new Vector3(22.5f, 45f, -22.5f);
        }
        if (angle < 67.5 && angle > 22.5 && direction.x > 0)
        {
            playerRotation.Value = new Vector3(22.5f, -45f, 22.5f);
        }

        if (angle >= 67.5 && angle < 112.5 && direction.x < 0)
        {
            playerRotation.Value = new Vector3(30f, 60f, -45f);
        }
        if (angle >= 67.5 && angle < 112.5 && direction.x > 0)
        {
            playerRotation.Value = new Vector3(30f, -60f, 45f);
        }

        if (angle >= 112.5 && angle < 157.5 && direction.x < 0)
        {
            playerRotation.Value = new Vector3(60f, 135f, -20f);
        }
        if (angle >= 112.5 && angle < 157.5 && direction.x > 0)
        {
            playerRotation.Value = new Vector3(60f, -135f, 20f);
        }
        if (angle >= 157.5)
        {
            playerRotation.Value = new Vector3(70f, 180f, 0f);
        }

    }


    [ServerRpc(RequireOwnership = false)]
    void MouseMoveRequestServerRpc(Vector3 newTarget)
    {
        mouseTarget.Value = newTarget;
    }
}
