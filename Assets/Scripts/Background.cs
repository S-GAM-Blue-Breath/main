using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    private float moveSpeed = 3f;

    void Start()
    {

    }

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        if (transform.position.x < -17.76) {
            transform.position += new Vector3(35.52f, 0, 0);
        }
    }
}
