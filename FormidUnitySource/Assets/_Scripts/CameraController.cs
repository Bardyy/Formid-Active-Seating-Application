using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    public const int LEFT_MOUSE_BUTTON = 0;
    public const int MIDDLE_MOUSE_BUTTON = 2;
    public const int RIGHT_MOUSE_BUTTON = 1;
    public const float DOUBLE_CLICK_DELAY = 0.2f;

    public const float MAX_DIST = 20.0f;
    public const float MIN_DIST = 2.5f;
    public const float ANGLE_CLAMP_OFFSET = 0.2f;

    // - - - - Public variables
    public GameObject defaultpivotPoint;
    public GameObject floor;
    public float mouseSens = 10.0f;
    public float distance = 3.0f;
    public GameObject presetViewsPrefab;

    // - - - - Private variables

    private GameObject _pivotPoint;
    private GameObject _presetViews;


    private float _floorHeight = -Mathf.Infinity;
    private float _ceilingHeight = 0;
    private float _scrollSpeed = 0.5f;
    private bool _isRotating = false;

    void Awake() {
        if(defaultpivotPoint == null) defaultpivotPoint = GameObject.Find("Controller");    // If default pivot isn't defined, find it
        this._pivotPoint = defaultpivotPoint;   // Set pivot point to default

        // Create new preset views game object on instantiation
        this._presetViews = Instantiate(this.presetViewsPrefab) as GameObject;
        Transform canvasTransform = GameObject.Find("Canvas").transform;
        this._presetViews.transform.localScale = canvasTransform.localScale;
        this._presetViews.transform.SetParent(canvasTransform);
        Rect r = this._presetViews.GetComponent<RectTransform>().rect;
        this._presetViews.GetComponent<RectTransform>().anchoredPosition = new Vector2(-r.width / 2.0f, -r.height / 2.0f);


        for (int i=0; i<this._presetViews.transform.childCount; i++) {

            Transform child = this._presetViews.transform.GetChild(i);
            if(child.name == "FrontView") {
                child.GetComponent<Button>().onClick.AddListener(delegate() { 
                    this.transform.position = this._pivotPoint.transform.position + (new Vector3(0, 0, 1)); 
                });
            }
            else if(child.name == "LeftSideView") {
                child.GetComponent<Button>().onClick.AddListener(delegate() { 
                    this.transform.position = this._pivotPoint.transform.position + (new Vector3(-1, 0, 0)); 
                });
            }
            else if(child.name == "RightSideView") {
                child.GetComponent<Button>().onClick.AddListener(delegate() { 
                    this.transform.position = this._pivotPoint.transform.position + (new Vector3(1, 0, 0)); 
                });
            }
            else if(child.name == "DiagonalView") {
                child.GetComponent<Button>().onClick.AddListener(delegate() { 
                    this.transform.position = this._pivotPoint.transform.position + (new Vector3(1, 1, 1)); 
                });
            }
            else if(child.name == "BackView") {
                child.GetComponent<Button>().onClick.AddListener(delegate() { 
                    this.transform.position = this._pivotPoint.transform.position + (new Vector3(0, 0, -1)); 
                });
            }
        }
        
        // If default floor isn't defined, find it
        if(floor == null) floor = GameObject.Find("Floor");                                     
        if(this.floor != null) {
            _floorHeight = this.floor.transform.position.y + this.floor.transform.localScale.y / 2.0f + ANGLE_CLAMP_OFFSET;
        }
    }

    // Update method for input handling
    void Update() {
        // Controls that require mouse to be within view port
        if(MouseWithinViewPort()) {
            // Zooming in and out (Disable while rotating)
            if(!_isRotating) {
                if(Input.GetAxis("Mouse ScrollWheel") > 0.0f) distance -= _scrollSpeed;
                else if(Input.GetAxis("Mouse ScrollWheel") < 0.0f) distance += _scrollSpeed;
                distance = Mathf.Clamp(distance, MIN_DIST, MAX_DIST);
            }    

            // Enable dragging rotation (ONLY FROM WITHIN VIEW PORT)
            if(Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON)) _isRotating = true;

            // Selecting new pivot point (Disable while rotating)
            if(Input.GetMouseButtonDown(RIGHT_MOUSE_BUTTON) && !_isRotating) {
                RaycastHit hit;
                Ray ray = this.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit)) {
                    this._pivotPoint = hit.transform.gameObject;
                }
            }

            // Deselecting pivot point
            if(Input.GetKeyDown(KeyCode.Escape) && !_isRotating) this._pivotPoint = this.defaultpivotPoint;
        }

        // Holding RIGHT mouse button to DRAG
        if(_isRotating) {
            this.transform.RotateAround(_pivotPoint.transform.position, Vector3.up, Input.GetAxis("Mouse X") * mouseSens);
            this.transform.RotateAround(_pivotPoint.transform.position, this.transform.right, -Input.GetAxis("Mouse Y") * mouseSens);
        }

        // Disable dragging rotation (ONLY FROM WITHIN VIEW PORT)
        if(Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON)) _isRotating = false;
    }

    // LateUpdate for auto-adjustments handled by engine
    void LateUpdate() {
        // Set position relative to pivot
        Vector3 displacement = this.transform.position - this._pivotPoint.transform.position;
        displacement.Normalize();
        displacement *= distance;
        this.transform.position = this._pivotPoint.transform.position + displacement;

        // Clamp Y-axis position between floor and ceiling
        _ceilingHeight = this._pivotPoint.transform.position.y + distance - distance / 2.0f - ANGLE_CLAMP_OFFSET;
        if(this.transform.position.y < _floorHeight) this.transform.position = new Vector3(this.transform.position.x, _floorHeight, this.transform.position.z);
        if(this.transform.position.y > _ceilingHeight) this.transform.position = new Vector3(this.transform.position.x, _ceilingHeight, this.transform.position.z);

        // Keep looking at pivot
        this.transform.LookAt(this._pivotPoint.transform.position);
    }

    // ------------------------- Helper methods -------------------------

    // Check if the mouse is within view port
    bool MouseWithinViewPort() {
        Vector3 pos = Input.mousePosition;
        pos.x /= Screen.width;
        pos.y = 1.0f - (pos.y / Screen.height);
        return this.GetComponent<Camera>().rect.Contains(pos);
    }

    // Destroy the prefab preset views object
    public void DestroyPresetViewsUI() {
        Destroy(this._presetViews);
    }

    // Set the x offset (from the left) of the preset views object attributed to this camera. 
    public void SetPresetViewsUIXOffset(float xOffsetNormal) {
        float canvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().rect.width;
        Rect r = this._presetViews.GetComponent<RectTransform>().rect;
        this._presetViews.GetComponent<RectTransform>().anchoredPosition = new Vector2(-(1.0f - xOffsetNormal) * canvasWidth - r.width / 2.0f, -r.height / 2.0f);
    }
}