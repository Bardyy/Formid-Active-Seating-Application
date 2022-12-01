using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdder : MonoBehaviour {

    public const int MAX_ADDITONAL_CAMERAS = 3;
    
    // - - - - Public variables
    public GameObject additionalCameraPrefab;
    public GameObject addButton;
    public GameObject removeButton;

    // - - - - Private variables
    private List<GameObject> _additionalCameraList;

    void Start() {
        this._additionalCameraList = new List<GameObject>();        
    }

    // Update method for input handling
    void Update() {
        // Remove add button if maximum cameras reached
        if (this._additionalCameraList.Count == MAX_ADDITONAL_CAMERAS) {
            this.addButton.SetActive(false);
        }
        else {
            this.addButton.SetActive(true);
        }
        // Hide remove button if minimum cameras reached
        if (this._additionalCameraList.Count == 0) {
            this.removeButton.SetActive(false);
        }
        else {
            this.removeButton.SetActive(true);
        }
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
        lastCamera.GetComponent<CameraController>().DestroyPresetViewsUI();

        Destroy(lastCamera);
        
        AdjustAllViewPorts();
    }

    // Adjust the view ports for all cameras in list
    void AdjustAllViewPorts() {
        int numCameras = 1 + this._additionalCameraList.Count;
        float width = 1.0f / numCameras;
        this.GetComponent<Camera>().rect = new Rect(0, 0, width, this.GetComponent<Camera>().rect.height);
        this.GetComponent<CameraController>().SetPresetViewsUIXOffset(width);
        for(int i = 1; i < numCameras; i++) {
            Camera cam = this._additionalCameraList[i - 1].GetComponent<Camera>();
            cam.rect = new Rect(i * width, 0, width, cam.rect.height);
            cam.GetComponent<CameraController>().SetPresetViewsUIXOffset((i + 1) * width);
        }
    }
}
