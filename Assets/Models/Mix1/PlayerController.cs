using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    Vector2 moveDirection;
    Vector2 lookDirection;
    float jumpDirection;
    public float moveSpeed = 2;
    public float maxForwardSpeed = 8;
    public float turnSpeed = 100;
    float desiredSpeed;
    float forwardSpeed;
    float jumpSpeed = 30000f;

    const float groudAccel = 5;
    const float groudDecel = 10;
    
    private Transform  cameraMainTransform;

    Animator anim;
    Rigidbody rb;

    bool onGround = true;
    bool isMoveInput{
        get  {return !Mathf.Approximately(moveDirection.sqrMagnitude, 0f);}
    }

    public void OnMove(InputAction.CallbackContext context){
        moveDirection = context.ReadValue<Vector2>();
        // Debug.Log("KUYYYY "+moveDirection);
    }

    public void OnJump(InputAction.CallbackContext context){
        jumpDirection = context.ReadValue<float>();
        Debug.Log("JUMP "+jumpDirection);
    }

    public void OnLook(InputAction.CallbackContext context){
        float criteria_rot_X = 20;
        Vector2 lookDirection_temp = context.ReadValue<Vector2>();
        if(lookDirection_temp.x<(-1*criteria_rot_X)){
            lookDirection_temp.x = -1*criteria_rot_X;
        }else if(lookDirection_temp.x>criteria_rot_X){
            lookDirection_temp.x = criteria_rot_X;
        }

        lookDirection = lookDirection_temp;
        Debug.Log("Look "+lookDirection);
        transform.Rotate(0, lookDirection.x*30*Time.deltaTime,0);
        // Debug.Log("Model "+ transform.rotation.eulerAngles.y);
        // Debug.Log("Camera "+ cameraMainTransform.rotation.eulerAngles.y);
        // float rotatediff = transform.rotation.eulerAngles.y-cameraMainTransform.rotation.eulerAngles.y;
        // transform.Rotate(0, cameraMainTransform.rotation.eulerAngles.y,0);
        
    }


    public void WarptoOrigin(){
        Debug.Log("P CLOCK  ");
        SceneManager.LoadScene("All");
    }

    void Move(Vector2 direction){
        float turnAmout = direction.x;
        float fDirection = direction.y;
        
        
        if(direction.sqrMagnitude > 1f){
            direction.Normalize();
        }

        desiredSpeed = direction.magnitude * maxForwardSpeed * Mathf.Sign(fDirection);
        float acceleration = isMoveInput ? groudAccel : groudDecel;

        forwardSpeed = Mathf.MoveTowards(forwardSpeed, desiredSpeed, acceleration*Time.deltaTime);
        anim.SetFloat("ForwardSpeed", forwardSpeed);
        // transform.Rotate(0, turnAmout*turnSpeed*Time.deltaTime,0);
        if(direction!= Vector2.zero)
        transform.Rotate(0, lookDirection.x*50*Time.deltaTime,0);
        // if(fDirection==-1){
            
        // }else{
            
            
        // }
        Vector3 movement_temp = new Vector3(direction.x,0,direction.y);
        movement_temp = cameraMainTransform.forward * movement_temp.z + cameraMainTransform.right * movement_temp.x;
        movement_temp.y = 0;
        // transform.Translate(movement_temp.x*moveSpeed*Time.deltaTime, 0, movement_temp.z*moveSpeed*Time.deltaTime);
        transform.Translate(direction.x*moveSpeed*Time.deltaTime, 0, direction.y*moveSpeed*Time.deltaTime);
    }

    bool readyJump = false;
    float jumpEffort = 0;
    void Jump(float direction){
        // Debug.Log(direction);
        if(direction>0 && onGround){
            anim.SetBool("ReadyJump", true);
            readyJump = true;
            jumpEffort += Time.deltaTime;
        }else if(readyJump){
            anim.SetBool("Launch", true);
            readyJump = false;
            anim.SetBool("ReadyJump", false);
        }
        
    }

    public void Launch(){
        rb.AddForce(0, jumpSpeed * Mathf.Clamp(jumpEffort,1,3), 0);
        anim.SetBool("Launch",false);
        anim.applyRootMotion = false;
        Debug.Log("KUYz");
    }

    public void Land(){
        anim.SetBool("Land", false);
        anim.applyRootMotion = true;
        anim.SetBool("Launch",false);
        jumpEffort = 0;
    }
    void OnCollisionEnter(Collision col){
        anim.SetBool("Land", true);
        Debug.Log("asdasdasd");
    }


        private void OnTriggerEnter(Collider target) {
            Debug.Log("collisio = "+target.gameObject.name);
        if(target.gameObject.name.Equals("TeleporterCliff")){
            SceneManager.LoadScene("Cliff");
            Debug.Log("Cliff");
        }else if(target.gameObject.name.Equals("TeleporterForest")){
            SceneManager.LoadScene("Forest");
            Debug.Log("Forest");
        }
        else if(target.gameObject.name.Equals("TeleporterWharf")){
            SceneManager.LoadScene("Wharf");
            Debug.Log("Wharf");
        }
        else if(target.gameObject.name.Equals("TeleporterHell")){
            SceneManager.LoadScene("Hell");
            Debug.Log("Hell");
        }
        else if(target.gameObject.name.Equals("TeleporterDesert")){
            SceneManager.LoadScene("Desert");
            Debug.Log("Desert");
        }
        else if(target.gameObject.name.Equals("TeleporterSea")){
            SceneManager.LoadScene("Sea");
            Debug.Log("Sea");
        }
        else if(target.gameObject.name.Equals("TeleporterSnow")){
            SceneManager.LoadScene("Snow");
            Debug.Log("Snow");
        }
        else if(target.gameObject.name.Equals("TeleporterGod")){
            SceneManager.LoadScene("Godz");
            Debug.Log("God");
        }
        else if(target.gameObject.name.Equals("TeleporterDebris")){
            SceneManager.LoadScene("destroyed_city");
            Debug.Log("Debris");
        }
        else if(target.gameObject.name.Equals("TeleporterLowpoly")){
            SceneManager.LoadScene("DemoDay");
            Debug.Log("Lowpoly");
        }
        else if(target.gameObject.name.Equals("TeleporterTemple")){
            SceneManager.LoadScene("DemoScene");
            Debug.Log("Temple");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
        cameraMainTransform = Camera.main.transform;
    }

    float groundRayDist = 2f;

    // Update is called once per frame
    void Update()
    {
        Move(moveDirection);
        Jump(jumpDirection);

        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * groundRayDist * 0.5f, -Vector3.up);
        if(Physics.Raycast(ray, out hit, groundRayDist)){
            // Debug.Log("Onground1 = "+onGround);
            if(!onGround){
                onGround = true;
                anim.SetBool("Land", true);
            }
        }else{
            // Debug.Log("Onground2 = "+onGround);
            anim.SetBool("Launch",false);
            onGround = false;
            anim.applyRootMotion = false;
            anim.SetBool("Land", false);
        }
        Debug.DrawRay(transform.position + Vector3.up * groundRayDist * 0.5f, -Vector3.up * groundRayDist, Color.red);
    }
}
