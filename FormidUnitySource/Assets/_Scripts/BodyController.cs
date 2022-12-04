using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class BodyController : MonoBehaviour {

    public const int CHAIR_INPUT_BAUD_RATE = 9600;

    // - - - - Public variables
    public GameObject lowerTorso;
    public GameObject upperTorso;
    public GameObject leftUpperLeg;
    public GameObject rightUpperLeg;
    public GameObject neck;
    public GameObject chair;

    public Recorder recorder;       // Tape recorder
    
    public string configFilePath = "config.json";       // Config.JSON file path (including file name and extension)
    public string chairSerialPortName = "";             // COM port for the chair input
    public bool debugMode = false;                      // Whether to use serial input or to use Z/X to change posture

    // - - - - Private variables
    private PostureList _postures;
    private PostureInputInterface _sInput;
    private int _position = 0;

    // - - - - Properties
    public int Position {
        get { return _position; }
        set { return; }
    }
    public PostureList Positions {
        get { return _postures; }
        set { return; }
    }
    
    void Start() {
        // Set up serial input
        if(!debugMode) this._sInput = new PostureInputInterface(chairSerialPortName, CHAIR_INPUT_BAUD_RATE);
        
        // Read postures config file
        StreamReader streamReader = new StreamReader(configFilePath);
        string config = streamReader.ReadToEnd();
        this._postures = JsonConvert.DeserializeObject<PostureList>(config);
    }

    // Update method for input handling
    void Update() {
        // Get position from either recording or from chair input
        if(this.recorder.InPlayback()) {
            _position = this.recorder.PlaybackPosition;
        }
        else { 
            if(debugMode) {
                if(Input.GetKeyDown(KeyCode.Z)) this._position += 1;
                if(Input.GetKeyDown(KeyCode.X)) this._position -= 1;
            }
            else _position = this._sInput.GetInput();
        }

        // Maintain value within the number of postures available
        _position = Mathf.Clamp(_position, 0, this._postures.Size - 1);
    }

    // LateUpdate for auto-adjustments handled by engine
    void LateUpdate() {
        var posture = this._postures.Postures[_position];

        // Set the posture (Perform top to bottom by hierarchy to avoid artifacts)
        this.chair.transform.eulerAngles = posture.Chair.toVector3();
        this.lowerTorso.transform.eulerAngles = posture.LowerTorso.toVector3();
        this.upperTorso.transform.eulerAngles = posture.UpperTorso.toVector3();
        this.leftUpperLeg.transform.eulerAngles = posture.UpperLeftLeg.toVector3();
        this.rightUpperLeg.transform.eulerAngles = posture.UpperRightLeg.toVector3();
        this.neck.transform.eulerAngles = posture.Neck.toVector3();
    }
}
