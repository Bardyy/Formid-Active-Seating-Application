using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public const int LMB_CODE = 0;
    public const float MAX_DIST = 20.0f;
    public const float MIN_DIST = 2.5f;
    public const float ANGLE_CLAMP_OFFSET = 0.2f;

    // - - - - Public variables

    public GameObject pivotPoint;
    public GameObject floor;
    public float mouseSens = 10.0f;
    public float distance = 3.0f;

    // - - - - Private variables
    
    private float _floorHeight = -Mathf.Infinity;
    private float _ceilingHeight = 0;
    private float _scrollSpeed = 0.5f;

    void Start() {
        if(this.floor != null) {
            _floorHeight = this.floor.transform.position.y + this.floor.transform.localScale.y / 2.0f + ANGLE_CLAMP_OFFSET;
        }
    }

    void Update() {
        // Zooming in and out
        if(Input.GetAxis("Mouse ScrollWheel") > 0.0f) distance -= _scrollSpeed;
        else if(Input.GetAxis("Mouse ScrollWheel") < 0.0f) distance += _scrollSpeed;
        distance = Mathf.Clamp(distance, MIN_DIST, MAX_DIST);

        // Holding left mouse button
        if(Input.GetMouseButton(LMB_CODE)) {
            this.transform.RotateAround(pivotPoint.transform.position, Vector3.up, Input.GetAxis("Mouse X") * mouseSens);
            this.transform.RotateAround(pivotPoint.transform.position, this.transform.right, -Input.GetAxis("Mouse Y") * mouseSens);
        }

        // Set position relative to pivot
        Vector3 displacement = this.transform.position - this.pivotPoint.transform.position;
        displacement.Normalize();
        displacement *= distance;
        this.transform.position = this.pivotPoint.transform.position + displacement;

        // Clamp Y-axis position between floor and ceiling
        _ceilingHeight = distance - distance / 2.0f - ANGLE_CLAMP_OFFSET;
        if(this.transform.position.y < _floorHeight) this.transform.position = new Vector3(this.transform.position.x, _floorHeight, this.transform.position.z);
        if(this.transform.position.y > _ceilingHeight) this.transform.position = new Vector3(this.transform.position.x, _ceilingHeight, this.transform.position.z);

        // Keep looking at pivot
        this.transform.LookAt(this.pivotPoint.transform.position);
    }
}
