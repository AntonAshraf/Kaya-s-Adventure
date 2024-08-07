using System.Collections;
using System.Collections.Generic;
// using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

// for particle system Rain https://www.youtube.com/watch?app=desktop&v=MBVGUD5nZeA

public class PlayerMove : MonoBehaviour
{
    public CharacterController charController;
    public float maxSpeed = 1.0f;

    private float gravityY = 0.0f;
    public bool isRunning = false;
    public Material sunnySkybox;
    public Material nightSkybox;
    public bool isSunny = true;
    public bool isJumping, isGrounded;
    public Camera minimapCamera;
    public MeshRenderer arrowMeshRenderer;
    public Camera camera;
    public Animator anim;
    public GameObject myHands; //reference to your hands/the position where you want your object to go
    bool canpickup; //a bool to see if you can or cant pick up the item
    GameObject ObjectIwantToPickUp; // the gameobject onwhich you collided with
    public bool hasItem = false; // a bool to see if you have an item in your hand
    private Quaternion lastParentRotation;
    public AudioSource winAudio;
    public AudioSource nextLevelAudio;
    public AudioSource actionAudio;

    void Start()
    {
        isGrounded = true;
        charController = GetComponent<CharacterController>();
        SunnyMode();
        canpickup = false;    //setting both to false
        minimapCamera.enabled = false;
        arrowMeshRenderer.enabled = false;
        lastParentRotation = myHands.transform.parent.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        float dX = Input.GetAxis("Horizontal");
        float dY = Input.GetAxis("Vertical");

        Vector3 nonNormalizedMovementVector = new Vector3(dX, 0, dY);

        Vector3 movementVector = nonNormalizedMovementVector;
        movementVector = Quaternion.AngleAxis(camera.transform.eulerAngles.y, Vector3.up) * movementVector;

        movementVector.Normalize();


        gravityY += Physics.gravity.y * Time.deltaTime;

        // to lock the rotation of the hands that hold the staff
        // https://stackoverflow.com/questions/52179975/make-child-unaffected-by-parents-rotation-unity#:~:text=An%20alternative%20might%20be%20to,kinematic%20and%20lock%20the%20rotation..&text=You%20could%20apply%20the%20opposite,the%20child%20is%20not%20rotating. 
        myHands.transform.localRotation = Quaternion.Inverse(myHands.transform.parent.localRotation) * lastParentRotation * myHands.transform.localRotation;
        lastParentRotation = myHands.transform.parent.localRotation;

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (minimapCamera.enabled)
                minimapCamera.enabled = false;
            else
                minimapCamera.enabled = true;
        }

        if (Input.GetKey(KeyCode.Tab))
        {
            arrowMeshRenderer.enabled = true;
        }
        else
            arrowMeshRenderer.enabled = false;

        if (GameObject.Find("Time").GetComponent<TimeCountdown>().timeLeft == 0)
        {
            Debug.Log("You lose");
            anim.SetBool("Lose", true);
            this.enabled = false;
        }

        if (charController.isGrounded)
        {

            gravityY = -0.5f;

            isGrounded = true;
            anim.SetBool("Grounded", true);

            isJumping = false;
            anim.SetBool("Jumping", false);

            anim.SetBool("Falling", false);


            if (Input.GetKeyDown(KeyCode.Space))
            {
                gravityY = 7.0f;
                isJumping = true;
                anim.SetBool("Jumping", true);
            }


        }
        else
        {
            isGrounded = false;
            anim.SetBool("Grounded", false);

            if (isJumping && gravityY < 0 || gravityY < -5)
            {
                anim.SetBool("Falling", true);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && canpickup && !hasItem)
        {
            transform.LookAt(ObjectIwantToPickUp.transform);
            nextLevelAudio.Play();
            anim.Play("Pickup");
            hasItem = true;
        }

        if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Pickup")) {
            // stop the player from moving
            movementVector = Vector3.zero;
        }
            


        if (movementVector != Vector3.zero)
        {
            Quaternion rotationDirection = Quaternion.LookRotation(movementVector, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationDirection, 360 * Time.deltaTime);


            if (Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetFloat("IWR", 1.0f, 0.5f, Time.deltaTime);
            }
            else
                anim.SetFloat("IWR", 0.5f, 0.5f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("IWR", 0.0f, 0.5f, Time.deltaTime);
        }

        if (isGrounded == false)
        {
            Vector3 airMove = movementVector * anim.GetFloat("IWR") * Random.Range(2.0f, 4.0f);
            airMove.y = gravityY;
            charController.Move(airMove * Time.deltaTime);
        }

    }
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        else
            UnityEngine.Cursor.lockState = CursorLockMode.None;
    }

    private void OnAnimatorMove()
    {
        if (isGrounded)
        {
            Vector3 _move = anim.deltaPosition;
            _move.y = gravityY * Time.deltaTime;
            charController.Move(_move);
        }
    }
    private void SunnyMode()
    {
        isSunny = true;
        RenderSettings.skybox = sunnySkybox;
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 5);
        DynamicGI.UpdateEnvironment();
    }
    private void NightMode()
    {
        isSunny = false;
        RenderSettings.skybox = nightSkybox;
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * 5);
        DynamicGI.UpdateEnvironment();
    }

    private void PickupStaff()
    {
        // https://forum.unity.com/threads/3d-character-picking-up-item.900098/ 
        if (canpickup)
        {
            actionAudio.Play();

            ObjectIwantToPickUp.transform.parent = myHands.transform; //set the parent of the object to your hands
            ObjectIwantToPickUp.transform.position = myHands.transform.position; //set the position of the object to your hands
            ObjectIwantToPickUp.transform.rotation = myHands.transform.rotation; //set the rotation of the object to your hands
            ObjectIwantToPickUp.GetComponent<Rigidbody>().isKinematic = true;   //set the rigidbody to kinematic so it doesnt fall out of your hands
            NightMode();
            // activate the point light on the staff
            ObjectIwantToPickUp.transform.GetChild(0).gameObject.SetActive(true);
            print(ObjectIwantToPickUp.transform.GetChild(0).gameObject.name);
        }
    }
    private void OnTriggerEnter(Collider other) // to see when the player enters the collider
    {
        if (other.gameObject.tag == "staff") //on the object you want to pick up set the tag to be anything, in this case "object"
        {
            canpickup = true;  //set the pick up bool to true
            ObjectIwantToPickUp = other.gameObject; //set the gameobject you collided with to one you can reference
        }
        if (other.gameObject.tag == "sacredTree")
        {
            actionAudio.Stop();
            anim.SetBool("Win", true);
            this.enabled = false;
            winAudio.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        canpickup = false;
    }
}
