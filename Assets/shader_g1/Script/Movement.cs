using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 5.0f;

    void Update()
    {
       var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
       transform.position += move * speed * Time.deltaTime;
    }
}
