using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DocOck : MonoBehaviour
{
    [SerializeField] public float speed = 5f;
    [SerializeField] public float rotationSpeed = 100f;
    [SerializeField] public float jumpForce = 1f;
    [SerializeField] private float jumpCooldown = 1f;
    [SerializeField] private ClawController[] takeSpiderman;
    [SerializeField] private float mouseSensitivity = 100f;
    private bool canJump = true;

    private Vector3 movement;
    private Rigidbody rb;

    private float xRotation = 0f;
    private Vector3 dragOrigin;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        movement = (transform.right * moveX + transform.forward * moveZ).normalized;
        rb.AddForce(movement * speed * Time.deltaTime, ForceMode.Impulse);

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            canJump = false;
            Invoke("OctaviusCanJump", jumpCooldown);
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            takeSpiderman[0].SetStartAnimation();
        }
        if( Input.GetKeyDown(KeyCode.Escape))
        {
            #if UNITY_STANDALONE
                Application.Quit();
            #endif
            #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
            #endif
        }

        Mouse();
    }

    private void Mouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, transform.localRotation.eulerAngles.y + mouseX, 0f);
    }

    void OctaviusCanJump()
    {
        canJump = true;
    }
}