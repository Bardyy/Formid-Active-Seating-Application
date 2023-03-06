using System.Collections;
using System.Collections.Generic;
using System;

public class Recording {
    
    /* Core Protocol:
       A recording in the context of this application is a list
       of frame where each frame contains a position value and a 
       timestamp. The timestamp for a frame is the number of 
       seconds it has been since the start of the recording.

       The process of making the recording will involve checking
       for changed postures. If the posture is changed, the new
       posture is recorded along with the timestamp of when the
       change happened.

       Playback of recording will involve a similar process of
       setting the model's posture to the frame's value when
       the timestamp of said frame is reached.
    */

    // Singular frame of the recording
    public class Frame {
        public int Posture { get; set; }
        public float Timestamp { get; set; }

        public Frame(int posture, float timestamp) {
            this.Posture = posture;
            this.Timestamp = timestamp;
        }
    }

    // List of all frames in the recording
    public List<Frame> Frames { get; set; }
    public DateTime startTime;

    public Recording() {
        this.Frames = new List<Frame>();
        startTime = DateTime.Now; 
    }

    public Recording(string datetime) {
        this.Frames = new List<Frame>();
        startTime = DateTime.Parse(datetime); 
    }

    // Add a frame to recording
    public void AddFrame(int posture, float timestamp) {
        this.Frames.Add(new Frame(posture, timestamp));
    }
}
