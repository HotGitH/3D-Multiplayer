using UnityEngine;
//using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    //---------inputs------///

    public Transform viewPoint; // where the eyes are
    public float mouseSensitivity = 1f;
    private float verticalRotStore;
    private Vector2 mouseInput;

    public bool invertLook;  //how you want the mouse to work (like a plane or regular)
    //---speeds and movement---//
    public float walkSpeed = 5f, runSpeed = 8f, activeMoveSpeed;
    //private float activeMoveSpeed;
    private Vector3 moveDir, movement;

    public CharacterController charCon; //character controller instead of collider 
    /// </summary>

    private Camera cam;

    public float jumpForce = 12f, gravityMod = 2.5f;  //jump force and extra gravity force fir a nice fall 

    //----ground check---//
    public Transform groundCheckpoint;// extra check that is grounded using a raycast
    private bool isGrounded;
    public LayerMask groundLayers; //using the ground layer as a target for the raycast

    //----bullets  ----//
    public GameObject bulletImpact; //the quads of the bullet hit prefab
    private float shotCounter;

    //----muzzle slow show---//
    public float muzzleDisplayTime;
    private float muzzleCounter;


    //----gun settings---//
    public Gun[] allGuns;
    private int selectedGun;

    //----emmo use ---// unused yet  
    private int bullets, bulletUsed;

    //--------- end inputs------///

    void Start()
    {
        //locking the curser 
        Cursor.lockState = CursorLockMode.Locked;
        //so the camera will respawn
        cam = Camera.main;
        SwitchGun();

        //----spawning a player at a random spawn point from the spawn manager---//
        Transform newPlayerSpawnPoint = SpawnManager.instance.GetRandomSpawnPoint();
        transform.position = newPlayerSpawnPoint.position;
        transform.rotation = newPlayerSpawnPoint.rotation;
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
            viewPoint.rotation = Quaternion.Euler(verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z); // looking up down
        }
        else
        {
            viewPoint.rotation = Quaternion.Euler(-verticalRotStore, viewPoint.rotation.eulerAngles.y, viewPoint.rotation.eulerAngles.z); // looking up down
        }
        //setting the  movement of the player
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        //----walking/running----//
        if (Input.GetKey(KeyCode.LeftShift))
        {
            activeMoveSpeed = runSpeed;
        }
        else
        {
            activeMoveSpeed = walkSpeed;
        }
        //----end walking/running----//

        //----jumping----//
        float yVel = movement.y; //value for gravity

        movement = ((transform.forward * moveDir.z) + (transform.right * moveDir.x)).normalized * activeMoveSpeed;
        movement.y = yVel;

        if (charCon.isGrounded)
        {
            movement.y = 0f;
        }

        isGrounded = Physics.Raycast(groundCheckpoint.position, Vector3.down, .25f, groundLayers);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            movement.y = jumpForce;
        }

        movement.y += Physics.gravity.y * Time.deltaTime * gravityMod;//adding gravity////////

        charCon.Move(movement * activeMoveSpeed * Time.deltaTime);

        // ---deactivate muzzle flash----//
        for (int i = 0; i < allGuns[selectedGun].muzzleFlash.Length; i++)
        {
            if(allGuns[selectedGun].muzzleFlash[i].activeInHierarchy)
            {
                muzzleCounter -= Time.deltaTime;
                if (muzzleCounter <=0)
                    allGuns[selectedGun].muzzleFlash[i].SetActive(false);
            }
        }

        if (Input.GetMouseButtonDown(0)) //one shot
        {
            Shoot();
        }
        //if you keep pressing andd the gun is automatic then you can multishoot  
        if (Input.GetMouseButton(0) && allGuns[selectedGun].isAotumatic)  
        {
            shotCounter -= Time.deltaTime;
            if (shotCounter <= 0)
            {
                Shoot();
            }
        }

        //----choosing guns ----//
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            selectedGun++;
            if (selectedGun >= allGuns.Length) selectedGun = 0;// roll to begining of the list
            SwitchGun();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            selectedGun--;
            if (selectedGun < 0) selectedGun = allGuns.Length -1;// roll to end of the list
            SwitchGun();
        }
        


        //   ----unlock curser usidng escape----//
        if (Input.GetKeyDown(KeyCode.Escape))// to unlock the cursor when you press escape key
        {
            Cursor.lockState = CursorLockMode.None;
        } else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        //   ----end unlock curser----//
    }

    private void Shoot()  // the shooting fuction
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));// a ray that shoots from the camera to the middle of the screen
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // create the prefab on the outside surface of the thing that the raycast hits
            GameObject gameHitObj = Instantiate(bulletImpact, hit.point + (hit.normal * 0.02f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(gameHitObj, 10f); // destroy the hit points after 10 sec so you do not burden the memory 
        }
        // using the time between shots in the gun itself depending on what we select
        shotCounter = allGuns[selectedGun].timeBetweenShots;
        
        // ---activate muzzle flash----//
        for (int i = 0; i < allGuns[selectedGun].muzzleFlash.Length; i++)
        {
            allGuns[selectedGun].muzzleFlash[i].SetActive(true);
        }
        muzzleCounter = muzzleDisplayTime;
    }

    private void LateUpdate()
    {
        cam.transform.position = viewPoint.position;
        cam.transform.rotation = viewPoint.rotation;
    }

    //-----function for actual switch gun----//
    void SwitchGun()
    {
        foreach(Gun gun in allGuns)
        {
            gun.gameObject.SetActive(false);
        }
        allGuns[selectedGun].gameObject.SetActive(true);
        for (int i = 0; i < allGuns[selectedGun].muzzleFlash.Length; i++)
        {
           allGuns[selectedGun].muzzleFlash[i].SetActive(false);
        }
    }
}
