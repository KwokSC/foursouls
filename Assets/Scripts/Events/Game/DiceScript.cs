using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceScript : MonoBehaviour
{
    static Rigidbody rb;
    public static Vector3 diceVelocity;
    public GameObject diceArea;
    public Vector3 originalPosition;
    Vector3 gravityDirection;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        gravityDirection = diceArea.transform.TransformDirection(Vector3.forward).normalized;
        Debug.Log(transform.up);
        Debug.Log(gravityDirection);
    }

    // Update is called once per frame
    void Update()
    {
        diceVelocity = rb.velocity;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float dirX = Random.Range(0, 200);
            float dirY = Random.Range(0, 200);
            float dirZ = Random.Range(0, 200);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = originalPosition;
            transform.rotation = Quaternion.identity;
            rb.AddForce(gravityDirection * -500);
            rb.AddTorque(dirX, dirY, dirZ);
        }
    }
}
