using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyController : MonoBehaviour {

    // - - - - Public variables

    public GameObject torso;
    public GameObject upperTorso;
    public GameObject leftThigh;
    public GameObject rightThigh;
    
    public int position = 0;

    // - - - - Private variables

    private Vector3 rotTorso;
    private Vector3 rotUpperTorso;
    private Vector3 rotLeftThigh;
    private Vector3 rotRightThigh;

    private float[,] positions;

    void Awake() {
        // this.rotTorso = new Vector3(0, 0, 0);
        // this.rotUpperTorso = new Vector3(0, 0, 0);
        // this.rotLeftThigh = new Vector3(0, 0, 0);
        // this.rotRightThigh = new Vector3(0, 0, 0);
    }

    void Start() {
        this.positions = new float[,] {
            { 0, 0, 0, 0, 0, 0, 0, -5, 0, 0, 5, 0 },
            { 0, 0, 0, 0, 0, 0, 10, -5, 0, 10, 5, 0 },
            { -10, 0, 0, 10, 0, 0, 10, -5, 0, 10, 5, 0 },
            { 0, 0, 0, 0, 0, 0, 20, -20, -10, 10, 5, -5 },
            { 0, 0, 20, 0, 0, 20, 20, -20, -10, 10, 5, -5 },
            { 0, 0, 0, 0, 0, 0, 10, -5, 5, 20, 20, 10 },
            { 0, 0, -20, 0, 0, -20, 10, -5, 5, 20, 20, 10 }
        };
    }

    void Update() {
        // Test values
        if(Input.GetKeyDown(KeyCode.Z)) this.position += 1;
        if(Input.GetKeyDown(KeyCode.X)) this.position -= 1;
        
        if(this.position < 0) this.position = 0;
        if(this.position > 6) this.position = 6;
        

        Vector3[] rotations = new Vector3[4];
        for(int i = 0; i <= 9; i += 3) {
            rotations[i / 3] = new Vector3(this.positions[this.position, i],
                                           this.positions[this.position, i + 1], 
                                           this.positions[this.position, i + 2]);
        }

        this.torso.transform.eulerAngles = rotations[0];
        this.upperTorso.transform.eulerAngles = rotations[1];
        this.leftThigh.transform.eulerAngles = rotations[2];
        this.rightThigh.transform.eulerAngles = rotations[3];
    }
}
