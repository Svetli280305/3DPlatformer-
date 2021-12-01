using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    float rotation = 0.0f;
    public float rotationSpeed = 2.0f;
    float camRotation = 0.0f;
    public float camRotationSpeed = 1.5f;
    GameObject cam;
    Rigidbody myRigidbody;

    bool isOnGround;
    public GameObject groundChecker;
    public LayerMask groundLayer;
    //public float jumpForce = 300.0f;

    public float maxSpeed;
    public float normalSpeed = 8.0f;
    public float sprintSpeed = 15.0f;

    float jumpPressure = 0f;
    public float minJump = 6f;
    public float maxJumpPressure = 30f;

    public AudioClip jump;
    public AudioClip backgroundMusic;

    public AudioSource sfxPlayer;
    public AudioSource musicPlayer;

    Animator myAnim;

    AudioSource myAudioSource;

    [SerializeField] GameObject playerVisuals;

    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponentInChildren<Animator>();

        cam = GameObject.Find("Main Camera");
        myRigidbody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        myAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        musicPlayer.clip = backgroundMusic;
        musicPlayer.loop = true;
        musicPlayer.Play();
        //transform.position = transform.position + transform.forward * Input.GetAxis("Vertical") * maxSpeed;
        //transform.position = transform.position + transform.right * Input.GetAxis("Horizontal") * maxSpeed;

        isOnGround = Physics.CheckSphere(groundChecker.transform.position, 0.1f, groundLayer);

        //if (isOnGround == true && Input.GetKeyDown(KeyCode.Space))
        //{
        //myRigidbody.AddForce(transform.up * jumpForce);
        //}

        Vector3 newVelocity = transform.forward * Input.GetAxis("Vertical") * maxSpeed;
        Vector3 newVelocity2 = transform.right * Input.GetAxis("Horizontal") * maxSpeed;

        Vector3 finalVelocity = newVelocity + newVelocity2;

        myRigidbody.velocity = new Vector3(finalVelocity.x, myRigidbody.velocity.y, finalVelocity.z);

        if ((myRigidbody.velocity.magnitude > 1.0f) && (myAudioSource.isPlaying == false) && (isOnGround == true))
        {
            myAudioSource.volume = Random.Range(0.8f, 1.0f);
            myAudioSource.pitch = Random.Range(0.8f, 1.0f);
            myAudioSource.Play();
        }

        rotation = rotation + Input.GetAxis("Mouse X") * rotationSpeed;
        transform.rotation = Quaternion.Euler(new Vector3(0.0f, rotation, 0.0f));

        camRotation = camRotation + Input.GetAxis("Mouse Y") * camRotationSpeed;
        camRotation = Mathf.Clamp(camRotation, -40.0f, 40.0f);
        cam.transform.localRotation = Quaternion.Euler(new Vector3(-camRotation, 0.0f, 0.0f));

        myAnim.SetFloat("speed", newVelocity.magnitude);
        myAnim.SetBool("isOnGround", isOnGround);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            maxSpeed = sprintSpeed;
        }
        else
        {
            maxSpeed = normalSpeed;
        }

        if (isOnGround)
        {
            //holding jump button//
            if (Input.GetButton("Jump"))
            {
                if (jumpPressure < maxJumpPressure)
                {
                    jumpPressure += Time.deltaTime * 10f;
                }
                else
                {
                    jumpPressure = maxJumpPressure;
                }
            }
            else
            {
                //jump//
                if (jumpPressure > 0f)
                {
                    jumpPressure = jumpPressure + minJump;
                    myRigidbody.velocity = new Vector3(jumpPressure / 10f, jumpPressure, 3f);
                    jumpPressure = 0f;
                    sfxPlayer.clip = jump;
                    sfxPlayer.Play();
                    myAnim.SetTrigger("jumped");
                }
            }
        }
        //not holding jump button//
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }
    }

    //void DoVisuals()
    //{
        //Vector3 inputVector = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //playerVisuals.transform.LookAt(transform.position + inputVector);
    //}
}