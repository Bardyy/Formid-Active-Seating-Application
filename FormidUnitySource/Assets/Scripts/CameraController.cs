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
        
    }

    void Update()
    {
        RotateCamera();
        float fixedX = Mathf.Clamp(this.transform.eulerAngles.x, -30.0f, 30.0f);
        this.transform.eulerAngles = new Vector3(fixedX, this.transform.eulerAngles.y, 0);
        this.transform.LookAt(this.cameraPivotPoint.transform.position);
    }

    void RotateCamera()
    {
        if (Input.GetMouseButton(0))
        {
            this.transform.RotateAround(cameraPivotPoint.transform.position, Vector3.up, -Input.GetAxis("Mouse X") * speed);
            float mouseChangeY = -Input.GetAxis("Mouse Y");
            if (mouseChangeY < 0 && (mouseChangeY * speed) - this.transform.eulerAngles.x > -45 ) {
                this.transform.RotateAround(cameraPivotPoint.transform.position, this.transform.right, mouseChangeY * speed);
            }
        }

    }
}
