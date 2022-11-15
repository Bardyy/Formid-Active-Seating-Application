using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // - - - - Public variables

    public GameObject cameraPivotPoint;

    public const float MAX_DISTANCE = 10.0f;
    public const float MIN_DISTANCE = 1.0f;

    // - - - - Private variables

    private float _distance;

    void Start()
    {
        this._distance = (this.transform.position - this.cameraPivotPoint.transform.position).magnitude;
        this.transform.LookAt(this.cameraPivotPoint.transform.position);
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.A)) {
            this.transform.RotateAround(this.cameraPivotPoint.transform.position, Vector3.up, 100 * Time.deltaTime);
        }
        if(Input.GetKey(KeyCode.D)) {
            this.transform.RotateAround(this.cameraPivotPoint.transform.position, Vector3.up, -100 * Time.deltaTime);
        }

        // Vector3 direction = this.transform.position - this.cameraPivotPoint.transform.position;
        // direction.Normalize();

        // if(Input.GetKey(KeyCode.W)) {
        //     this.transform.Translate(direction * 100 * Time.deltaTime);
        // }
        // if(Input.GetKey(KeyCode.S)) {
        //     this.transform.Translate(direction * -100 * Time.deltaTime);
        // }

        this._distance = System.Math.Clamp(this._distance, MIN_DISTANCE, MAX_DISTANCE);
    }
}
