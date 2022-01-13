using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float moveSpeed = 7f;
    public float smoothMoveTime = .1f;
    float smoothInputMagnitude;
    float smoothMoveVelocity;

    public float turnSpeed = 8f;
    float angle;

    private Rigidbody rb;
    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        // we'll use rb to set player rotation
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        float inputMagnitude = inputDirection.magnitude;
        smoothInputMagnitude = Mathf.SmoothDamp(smoothInputMagnitude, inputMagnitude, ref smoothMoveVelocity, smoothMoveTime);


        float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
        // using lerp to smooth the turning movement
        // multiplying to inputMagnitude so that when we stop moving the magnitude is 0 and the angle will not lerp
        angle = Mathf.LerpAngle(angle, targetAngle, Time.deltaTime * turnSpeed * inputMagnitude);

        /*
         // now using rigidbody 
         transform.eulerAngles = Vector3.up * angle; 
         transform.Translate(transform.forward * moveSpeed * Time.deltaTime * smoothInputMagnitude, Space.World);
        */

        velocity = transform.forward * moveSpeed * smoothInputMagnitude;
    }



    private void FixedUpdate()
    {
        // turning euler angle into a quaternian
        rb.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        rb.MovePosition(rb.position + velocity * Time.deltaTime);
    }

}
