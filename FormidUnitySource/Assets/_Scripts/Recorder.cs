using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;

public class Recorder : MonoBehaviour {

    enum RecorderState { Standby, Recording, Playback }

    // - - - - Public variables
    public BodyController postureControl;
    public GameObject recordingSignifierUI;
    public GameObject playbackSignifierUI;
    
    // - - - - Private variables
    private RecorderState _state;
    private int _previousPosition = -1;         // Used for recording positions
    private int _playbackPosition = -1;         // Used for replaying positions
    private float _timeRelativeStart = 0.0f;    // Used for recording and replaying timestamps
    private int _playbackIndex = 0;             // Index in list of frames for playback

    private Recording _currentRecording;        // Tape that is being currently recorded
    private Recording _lastRecording;           // Tape that has finished being recorded (Can be played back, saved, or overwritten)

    // - - - - Properties
    public int PlaybackPosition {
        get { return _playbackPosition; }
        set { return; }
    }

    // Start is called before the first frame update
    void Start() {
        this._state = RecorderState.Standby;
        this.recordingSignifierUI.SetActive(false);
        this.playbackSignifierUI.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        switch(this._state) {
        case RecorderState.Standby:
            this.recordingSignifierUI.SetActive(false);
            this.playbackSignifierUI.SetActive(false);
            break;
        case RecorderState.Recording:
            // Each update: If the posture has changed, add the new frame with timestamp to the recording
            if(this.postureControl.Position != _previousPosition && this._currentRecording != null) {
                _previousPosition = this.postureControl.Position;
                this._currentRecording.AddFrame(_previousPosition, Time.time - _timeRelativeStart);
            }
            this.recordingSignifierUI.SetActive(true);
            this.playbackSignifierUI.SetActive(false);
            break;
        case RecorderState.Playback:
            if(Time.time - _timeRelativeStart >= this._lastRecording.Frames[this._playbackIndex].Timestamp) {
                this._playbackPosition = this._lastRecording.Frames[this._playbackIndex++].Posture;
                if(this._playbackIndex == this._lastRecording.Frames.Count) StopPlayback();
            }
            this.recordingSignifierUI.SetActive(false);
            this.playbackSignifierUI.SetActive(true);
            break;
        default:    // Nothing in default state
            break;
        }
    }

    // ------------------------- Helper methods -------------------------

    // Return true if the current state is playback
    public bool InPlayback() {
        return this._state == RecorderState.Playback;
    }

    // Prompt the user for recording overwrite
    public bool PromptOverwrite() {
        if(this._lastRecording == null) return true;
        return EditorUtility.DisplayDialog("Overwrite recording", "This action will overwrite the existing recording. Would you like to continue?", "Yes", "No");
    }

    // Return true if the current state is recording
    public bool InRecording() {
        return this._state == RecorderState.Recording;
    }

    // Toggle Recording
    public void ToggleRecording() {
        if(this._state == RecorderState.Standby) StartRecording();
        else if(this._state == RecorderState.Recording) StopRecording();
    }

    // Toggle Playback
    public void TogglePlayback() {
        if(this._state == RecorderState.Standby) StartPlayback();
        else if(this._state == RecorderState.Playback) StopPlayback();
    }

    // Event for starting the recording
    public void StartRecording() {
        // Only perform if in standby state
        if(this._state == RecorderState.Standby) {
            // TODO: Prompt user if they would like to overwrite the last recording
            
            _timeRelativeStart = Time.time;
            this._lastRecording = null;
            this._currentRecording = new Recording();
            // Add first frame
            this._currentRecording.AddFrame(this.postureControl.Position, 0.0f);
            this._state = RecorderState.Recording;       // Set state to recording
        }
    }

    // Event for stopping a stated recording
    public void StopRecording() {
        // Only perform if in recording state
        if(this._state == RecorderState.Recording) {
            // Add the final frame and end recording
            if(this._currentRecording != null) this._currentRecording.AddFrame(this.postureControl.Position, Time.time - _timeRelativeStart);
            this._lastRecording = this._currentRecording;
            this._currentRecording = null;

            // Reset field values
            this._previousPosition = -1;
            this._playbackIndex = 0;
            this._playbackPosition = -1;
            this._timeRelativeStart = 0.0f;
            this._state = RecorderState.Standby;        // Reset state
        }
    }

    // Event for starting the playback
    public void StartPlayback() {
        // Only perform if in standby state and there is a recording to be played
        if(this._state == RecorderState.Standby && this._lastRecording != null) {
            this._timeRelativeStart = Time.time;
            this._playbackIndex = 0;
            this._playbackPosition = this._lastRecording.Frames[this._playbackIndex++].Posture;
            this._state = RecorderState.Playback;       // Set state to playback
        }
    }

    // Event for stpping a started playback
    public void StopPlayback() {
        // Only perform if in playback state
        if(this._state == RecorderState.Playback) {
            // Reset field values
            this._previousPosition = -1;
            this._playbackIndex = 0;
            this._playbackPosition = -1;
            this._timeRelativeStart = 0.0f;
            this._state = RecorderState.Standby;        // Reset state
        }
    }

    // Save recording to file
    public void SaveRecording() {
        // Only perform if in standby state
        if(this._state == RecorderState.Standby && this._lastRecording != null) {
            string path = EditorUtility.SaveFilePanel("Save Recording", "", "recording.fasa", "fasa");
            if (path.Length != 0) {
                string output = JsonConvert.SerializeObject(this._lastRecording, Formatting.Indented);   
                if (output != null) using (StreamWriter outputFile = new StreamWriter(path)) outputFile.WriteLine(output);
            }
        }
    }

    // Load recording from file
    public void LoadRecording() {
        // Only perform if in standby state
        if(this._state == RecorderState.Standby) {
            string path = EditorUtility.OpenFilePanel("Open Recording", "", "fasa");
            if (path.Length != 0) {
                StreamReader streamReader = new StreamReader(path);
                string input = streamReader.ReadToEnd();
                Recording loadedRecording = JsonConvert.DeserializeObject<Recording>(input);
                if(loadedRecording.Frames.Count > 0 && PromptOverwrite()) this._lastRecording = JsonConvert.DeserializeObject<Recording>(input);
            }
        }
    }
}
