using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class BodyController : MonoBehaviour {

    // - - - - Public variables

    public GameObject lowerTorso;
    public GameObject upperTorso;
    public GameObject leftUpperLeg;
    public GameObject rightUpperLeg;
    public GameObject neck;
    public GameObject chair;
    
    public int position = 0;
    
    public string configFilePath = "config.json";       // Config.JSON file path (including file name and extension)

    PostureList postures;

    void Start() {
        StreamReader streamReader = new StreamReader(configFilePath);
        string config = streamReader.ReadToEnd();
        
        this.postures = JsonConvert.DeserializeObject<PostureList>(config);

        // this.positions = new float[,] {
        //     { 0, 0, 0, 0, 0, 0, 0, -5, 0, 0, 5, 0 },
        //     { 0, 0, 0, 0, 0, 0, 10, -5, 0, 10, 5, 0 },
        //     { -10, 0, 0, 10, 0, 0, 10, -5, 0, 10, 5, 0 },
        //     { 0, 0, 0, 0, 0, 0, 20, -20, -10, 10, 5, -5 },
        //     { 0, 0, 20, 0, 0, 20, 20, -20, -10, 10, 5, -5 },
        //     { 0, 0, 0, 0, 0, 0, 10, -5, 5, 20, 20, 10 },
        //     { 0, 0, -20, 0, 0, -20, 10, -5, 5, 20, 20, 10 }
        // };
    }

    void Update() {
        // Test values
        if(Input.GetKeyDown(KeyCode.Z)) this.position += 1;
        if(Input.GetKeyDown(KeyCode.X)) this.position -= 1;
        
        this.position = Mathf.Clamp(this.position, 0, this.postures.Size - 1);
        var posture = this.postures.Postures[this.position];

        // Set the posture (Perform top to bottom by hierarchy to avoid artifacts)
        this.chair.transform.eulerAngles = posture.Chair.toVector3();
        this.lowerTorso.transform.eulerAngles = posture.LowerTorso.toVector3();
        this.upperTorso.transform.eulerAngles = posture.UpperTorso.toVector3();
        this.leftUpperLeg.transform.eulerAngles = posture.UpperLeftLeg.toVector3();
        this.rightUpperLeg.transform.eulerAngles = posture.UpperRightLeg.toVector3();
        this.neck.transform.eulerAngles = posture.Neck.toVector3();
    }
}
