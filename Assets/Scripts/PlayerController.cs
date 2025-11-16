using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform viewPoint; // where the eyes are
    public float mouseSensitivity;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;  //how you want the mouse to work (like a plane or regular)
    //speeds and movement
    public float walkSpeed = 5f, runSpeed = 8f, activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCon; //character controller instead of collider 
    /// </summary>

    private Camera cam;

    public float jumpForce = 12f, gravityMod= 2.5f;  //jump force and extra gravity force fir a nice fall 

    public Transform groundCheckpoint;// extra check that is grounded using a raycast
    private bool isGrounded;
    public LayerMask groundLayers; //using the ground layer as a target for the raycast

    void Start()
    {
        //locking the curser 
        Cursor.lockState = CursorLockMode.Locked;
        //so the camera will respawn
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * mouseSensitivity;   //getting input from player 
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z); // looking to the side you move the player
        verticalRotStore += mouseInput.y; // because of the Quaternions you need to limit and edit the looking up down
        verticalRotStore = Mathf.Clamp(verticalRotStore, -60f, 60f);

        if (invertLook)
        {
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z); // looking up down
        }
        else
        {
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z); // looking up down
        }
        //setting the  movement of the player
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = walkSpeed;
        }
        float yVel = movement.y; //value for gravity

        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
        movement.y = yVel;

        if (charCon.isGrounded)
        {
            movement.y = 0f;
        }

        isGrounded = Physics.Raycast(groundCheckpoint.position, Vector3.down, 0.25f, groundLayers);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpForce;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;//adding gravity

        charCon.Move(movement * activeMoveSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.Escape))// to unlock the cursor when you press escape key
        {
            Cursor.lockState = CursorLockMode.None;
        }else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

    }

    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }
}
