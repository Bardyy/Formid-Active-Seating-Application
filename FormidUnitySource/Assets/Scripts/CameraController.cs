using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // - - - - Public variables

    public GameObject cameraPivotPoint;
    public float speed = 50.0f;

    // - - - - Private variables

    private float _distance;
    

    void Start()
    {
        this._distance = (this.transform.position - this.cameraPivotPoint.transform.position).magnitude;
        this.transform.LookAt(this.cameraPivotPoint.transform.position);
    }

    void Update()
    {
        RotateCamera();

    }

    void RotateCamera()
    {
        if (Input.GetMouseButton(0))
        {
            this.transform.RotateAround(cameraPivotPoint.transform.position, Vector3.up, -Input.GetAxis("Mouse X") * speed);

            this.transform.RotateAround(cameraPivotPoint.transform.position, this.transform.right, -Input.GetAxis("Mouse Y") * speed);
        }

    }
}
