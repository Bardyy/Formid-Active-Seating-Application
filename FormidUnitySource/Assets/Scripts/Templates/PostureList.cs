using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostureList {

    // Posture class for each individual poster
    public class Posture {

        // Pivot class with angles
        public class Pivot {
            // Properties for X, Y, Z Euler angles
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }

            // Convert pivot to Unity 3D vector
            public Vector3 toVector3() {
                return new Vector3(X, Y, Z);
            }
        }

        // Pivot properties for each given posture
        public Pivot UpperLeftLeg { get; set; }
        public Pivot UpperRightLeg { get; set; }
        public Pivot LowerTorso { get; set; }
        public Pivot UpperTorso { get; set; }
        public Pivot Neck { get; set; }
        public Pivot Chair { get; set; }
    }

    // List of postures
    public List<Posture> Postures { get; set; }
    public int Size {
        get { return Postures.Count; }
        set { return; }
    }
}
