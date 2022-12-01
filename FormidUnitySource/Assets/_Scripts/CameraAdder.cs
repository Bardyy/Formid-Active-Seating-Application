using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdder : MonoBehaviour {

    public const int MAX_ADDITONAL_CAMERAS = 2;
    
    // - - - - Public variables
    public GameObject additionalCameraPrefab;

    // - - - - Private variables
    private List<GameObject> _additionalCameraList;

    void Start() {
        this._additionalCameraList = new List<GameObject>();        
    }

    // Update method for input handling
    void Update() {
        // Add additional camera
        if(Input.GetKeyDown(KeyCode.P)) AddCamera();
        else if(Input.GetKeyDown(KeyCode.O)) RemoveCamera();
    }

    // ------------------------- Helper methods -------------------------

    // Get a new camera from original camera
    public void AddCamera() {
        if(this._additionalCameraList.Count >= MAX_ADDITONAL_CAMERAS) return;

        GameObject newCamera = Instantiate(additionalCameraPrefab) as GameObject;
        newCamera.transform.position = this.transform.position;
        newCamera.transform.eulerAngles = this.transform.eulerAngles;
        newCamera.GetComponent<CameraController>().distance = this.gameObject.GetComponent<CameraController>().distance;
        this._additionalCameraList.Add(newCamera);

        AdjustAllViewPorts();
    }

    // Remove a camera from the end of the list
    public void RemoveCamera() {
        if(this._additionalCameraList.Count <= 0) return;

        // Remove the last camera
        GameObject lastCamera = this._additionalCameraList[this._additionalCameraList.Count - 1];
        this._additionalCameraList.RemoveAt(this._additionalCameraList.Count - 1);
        Destroy(lastCamera);
        
        AdjustAllViewPorts();
    }

    // Adjust the view ports for all cameras in list
    void AdjustAllViewPorts() {
        int numCameras = 1 + this._additionalCameraList.Count;
        float width = 1.0f / numCameras;
        this.GetComponent<Camera>().rect = new Rect(0, 0, width, this.GetComponent<Camera>().rect.height);
        for(int i = 1; i < numCameras; i++) {
            Camera cam = this._additionalCameraList[i - 1].GetComponent<Camera>();
            cam.rect = new Rect(i * width, 0, width, cam.rect.height);
        }
    }
}