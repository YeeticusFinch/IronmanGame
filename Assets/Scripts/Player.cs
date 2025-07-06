using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    Rigidbody rb;
    public Camera cam;

    public float maxSpeed = 5;
    public float moveForce = 4;

    public GameObject head;
    public GameObject rootBone;

    public GameObject rootPoseArmature; // This is a copy of the armature that is always in root pose

    bool thirdPerson = false;
    bool crouch = false;

    private PlayerInput playerInput;
    private InputAction freeLookAction;
    private InputAction thirdPersonAction;
    private InputAction crouchAction;
    private InputAction interactAction;

    public Vector3 camDir;
    public Vector3 moveDir;

    public Collider standCollider;
    public Collider crouchCollider;

    [System.NonSerialized]
    public Animator anim;

    public Bone[] bones;

    public static Interactable selectedInteraction;

    public Material invis;
    public SkinnedMeshRenderer meshRenderer;

    [System.NonSerialized]
    public Armor armor;

    [System.Serializable]
    public struct Bone
    {
        public string name;
        public GameObject bone;
        //public GameObject rootPosBone;
        public Bone(string name, GameObject bone/*, GameObject rootPosBone*/)
        {
            this.name = name;
            this.bone = bone;
            //this.rootPosBone = rootPosBone;
        }
    }
    
    private void Awake()
    {
        SetupActions();
        SetupAnimations();
    }

    private void SetupActions() // Set up all the input actions (keybind controls)
    {
        // Do we want to improve how we handle inputs? Or is this fine?
        playerInput = GetComponent<PlayerInput>();
        freeLookAction = playerInput.actions["FreeLook"]; // Default: Left Alt
        thirdPersonAction = playerInput.actions["3rdPerson"]; // Default: F5
        crouchAction = playerInput.actions["Crouch"]; // Default: C
        interactAction = playerInput.actions["Interact"]; // Default: F
    }

    private void SetupAnimations() // Sets up animation stuff
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        freeLookAction.Enable();
        thirdPersonAction.Enable();
    }

    private void OnDisable()
    {
        freeLookAction.Disable();
        thirdPersonAction.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        if (!thirdPerson)
        {
            cam.transform.parent = head.transform;
            cam.transform.localPosition = new Vector3(0, 0.00068f, 0.00107f);
        }
        camDir = head.transform.forward;
    }

    bool thirdPersonPressed = false;
    bool crouchPressed = false;
    private void Update()
    {
        //anim.SetBool("Root", true);
        if (thirdPersonAction.IsPressed())
        {
            if (!thirdPersonPressed) // This is to prevent it from being spammed if you hold F5
            {
                thirdPersonPressed = true;
                thirdPerson = !thirdPerson;
            }
        } else
        {
            thirdPersonPressed = false;
        }
        if (crouchAction.IsPressed())
        {
            if (!crouchPressed) // This is to prevent it from being spammed if you hold C
            {
                crouchPressed = true;
                crouch = !crouch;
                anim.SetBool("Crouch", crouch);
                crouchCollider.enabled = crouch; // Switch to the appropriate collider
                standCollider.enabled = !crouch; // If you're crouching, then your collider is smaller
                if (armor != null)
                {
                    armor.crouchCollider.enabled = crouch;
                    armor.standCollider.enabled = !crouch;
                }
            }
        }
        else
        {
            crouchPressed = false;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.togglePause();
        }
        if (!GameManager.instance.paused)
        {
            HandleMouse();
        }
        HandleCamera();
    }
    public void CheckInteractions()
    {
        bool interacting = false;
        RaycastHit hit;
        if (Physics.Raycast(head.transform.position, cam.transform.forward, out hit, 3))
        {
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.GetComponent<Interactable>() != null)
            {
                Interactable inter = hitObj.GetComponent<Interactable>();
                GameManager.instance.ui.interaction.SetActive(true);
                GameManager.instance.ui.interaction.GetComponent<TextMeshProUGUI>().text = "<  > " + inter.getText();
                //GameManager.instance.ui.interaction.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "  " + interactAction.bindings[0];
                interacting = true;
                selectedInteraction = inter;
            }
        }
        if (!interacting)
        {
            if (GameManager.instance.ui.interaction.activeSelf)
            {
                GameManager.instance.ui.interaction.SetActive(false);
                selectedInteraction = null;
            }
        }


        //GameManager.instance.ui.interaction;
    }
    public void HandleCamera()
    {
        if (thirdPerson) // Third person camera
        {
            if (cam.transform.parent != null) // If this is true, then third person camera hasn't been set up yet
            {
                // Set up third person camera
                cam.transform.parent = null;
            }
            // Place the third person camera into position
            Vector3 camOffset = (-cam.transform.forward * 8.5f + cam.transform.up * 2.9f) * 0.5f;
            RaycastHit hit; // Raycast to prevent the third person camera from clipping through walls
            //Debug.DrawRay(transform.position+transform.up*2, camOffset.normalized*9.81f, Color.green); // This was just to debug the raycast
            if (Physics.Raycast(transform.position+transform.up*2, camOffset.normalized, out hit, 8.9f * 0.5f, GameManager.instance.cameraMask))
            {
                cam.transform.position = hit.point; // If the raycast hits something, place the camera on the point it hit
            }
            else
            {
                // Raycast didn't hit anything, place the camera at the ideal position
                cam.transform.position = transform.position + transform.up*2 + camOffset;
            }
        } else // First person camera
        {
            if (cam.transform.parent == null) // If this is true, then first person camera hasn't been set up yet
            {
                // Set up first person camera
                cam.transform.parent = head.transform;
                cam.transform.localPosition = new Vector3(0, 0.00068f, 0.00107f);
                //cam.transform.rotation = Quaternion.identity;
            }
        }
    }

    void HandleMouse()
    {
        cam.transform.localEulerAngles -= new Vector3(Math.Clamp(Input.mousePositionDelta.y, -10, 10), 0, 0);
        if (!((cam.transform.localEulerAngles.x > -70 && cam.transform.localEulerAngles.x < 70) || (cam.transform.localEulerAngles.x > 290 && cam.transform.localEulerAngles.x < 361)))
        {
            cam.transform.localEulerAngles += new Vector3(Math.Clamp(Input.mousePositionDelta.y, -10, 10), 0, 0);
        }

        if (thirdPerson)
        {
            if (freeLookAction.IsPressed())
            {
                cam.transform.Rotate(transform.up * Math.Clamp(Input.mousePositionDelta.x, -10, 10), Space.World);
                float angle = Vector3.SignedAngle(horizontalComponent(transform.InverseTransformDirection(transform.forward)), horizontalComponent(transform.InverseTransformDirection(cam.transform.forward)), transform.up);
                if (Math.Abs(angle) > 80)
                {
                    cam.transform.Rotate(-transform.up * Math.Clamp(Input.mousePositionDelta.x, -10, 10), Space.World);
                }
                camDir = cam.transform.forward;
            }
            else
            {
                transform.Rotate(transform.up * Input.mousePositionDelta.x, Space.World);
                float angle = Vector3.SignedAngle(horizontalComponent(transform.InverseTransformDirection(transform.forward)), horizontalComponent(transform.InverseTransformDirection(cam.transform.forward)), transform.up);
                if (Math.Abs(angle) > 0.1)
                {
                    cam.transform.Rotate(transform.up * Math.Clamp(-angle, -4, 4), Space.World);
                }
                camDir = cam.transform.forward;
                moveDir = camDir;
            }
            head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, cam.transform.rotation, 4);
        } else
        {
            if (freeLookAction.IsPressed())
            {
                head.transform.Rotate(transform.up * Math.Clamp(Input.mousePositionDelta.x, -10, 10), Space.World);
                float angle = Vector3.SignedAngle(horizontalComponent(transform.InverseTransformDirection(transform.forward)), horizontalComponent(transform.InverseTransformDirection(head.transform.forward)), transform.up);
                if (Math.Abs(angle) > 80)
                {
                    head.transform.Rotate(-transform.up * Math.Clamp(Input.mousePositionDelta.x, -10, 10), Space.World);
                }
                camDir = head.transform.forward;
            }
            else
            {
                transform.Rotate(transform.up * Input.mousePositionDelta.x, Space.World);
                float angle = Vector3.SignedAngle(horizontalComponent(transform.InverseTransformDirection(transform.forward)), horizontalComponent(transform.InverseTransformDirection(head.transform.forward)), transform.up);
                if (Math.Abs(angle) > 0.1)
                {
                    head.transform.Rotate(transform.up * Math.Clamp(-angle, -4, 4), Space.World);
                }
                camDir = head.transform.forward;
                moveDir = camDir;
            }
        }
    }

    private void HandleControls()
    {
        // Movement
        Vector3 move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (crouch) move *= 0.8f;

        anim.SetFloat("Walk", move.magnitude);

        if (Vector2.Dot(transform.TransformDirection(move), horizontalComponent(rb.linearVelocity)) < maxSpeed)
        {
            rb.AddForce((transform.TransformDirection(move) * maxSpeed - horizontalComponent(rb.linearVelocity)) * moveForce);
            
        }

        if (interactAction.IsPressed() && selectedInteraction != null)
        {
            selectedInteraction.interact(this);
        }

    }

    int c = 0;
    private void FixedUpdate()
    {
        if (!GameManager.instance.paused)
        {
            HandleControls();

            if (c % 20 == 0)
            {
                CheckInteractions();
            }

            c++;
            if (c > 100000) c = 0;
        }
    }

    private Vector3 horizontalComponent(Vector3 v)
    {
        return new Vector3(v.x, 0, v.z);
    }
}
