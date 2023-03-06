using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Newtonsoft.Json;

public class Recorder : MonoBehaviour {

    enum RecorderState { Standby, Recording, Playback }

    // - - - - Public variables
    public BodyController postureControl;
    public GameObject recordingSignifierUI;
    public GameObject playbackSignifierUI;
    public GameObject loadButton;
    public GameObject saveButton;
    public GameObject recordButton;
    public GameObject playButton;
    
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
    void Awake() {
        this._state = RecorderState.Standby;
        this.recordingSignifierUI.SetActive(false);
        this.playbackSignifierUI.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        switch(this._state) {
        case RecorderState.Standby:
            // Set save and play buttons to show as disabled (transparent) when there is no video to save or play
            if(this._lastRecording == null) {
                Image imgSave = this.saveButton.GetComponentInChildren<Image>();
                imgSave.color = new Color(imgSave.color.r, imgSave.color.g, imgSave.color.b, 0.5f);
                Image imgPlay = this.playButton.GetComponentInChildren<Image>();
                imgPlay.color = new Color(imgPlay.color.r, imgPlay.color.g, imgPlay.color.b, 0.5f);
            }
            else {
                Image imgSave = this.saveButton.GetComponentInChildren<Image>();
                imgSave.color = new Color(imgSave.color.r, imgSave.color.g, imgSave.color.b, 1f);
                Image imgPlay = this.playButton.GetComponentInChildren<Image>();
                imgPlay.color = new Color(imgPlay.color.r, imgPlay.color.g, imgPlay.color.b, 1f);
            }
            break;
        case RecorderState.Recording:
            // Each update: If the posture has changed, add the new frame with timestamp to the recording
            if(this.postureControl.Position != _previousPosition && this._currentRecording != null) {
                _previousPosition = this.postureControl.Position;
                this._currentRecording.AddFrame(_previousPosition, Time.time - _timeRelativeStart);
            }
            break;
        case RecorderState.Playback:
            if(Time.time - _timeRelativeStart >= this._lastRecording.Frames[this._playbackIndex].Timestamp) {
                this._playbackPosition = this._lastRecording.Frames[this._playbackIndex++].Posture;
                if(this._playbackIndex == this._lastRecording.Frames.Count) StopPlayback();
            }
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
        if(this._state == RecorderState.Standby) {
            StartRecording();
        }
        else if(this._state == RecorderState.Recording) {
            StopRecording();
        }
    }

    // Toggle Playback
    public void TogglePlayback() {
        if(this._state == RecorderState.Standby) {
            StartPlayback();
        }
        else if(this._state == RecorderState.Playback) {
            StopPlayback();
        }
    }

    // Event for starting the recording
    public void StartRecording() {
        // Only perform if in standby state
        if(this._state == RecorderState.Standby) {
            
            if(PromptOverwrite()) {
                _timeRelativeStart = Time.time;
                this._lastRecording = null;
                this._currentRecording = new Recording();
                // Add first frame
                this._currentRecording.AddFrame(this.postureControl.Position, 0.0f);
                this._state = RecorderState.Recording;       // Set state to recording

                this.recordingSignifierUI.SetActive(true);
                this.playbackSignifierUI.SetActive(false);
                this.recordButton.GetComponentInChildren<Text>().text = "Stop";
                this.loadButton.SetActive(false);
                this.saveButton.SetActive(false);
                this.playButton.SetActive(false);
            }
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

            this.recordingSignifierUI.SetActive(false);
            this.playbackSignifierUI.SetActive(false);
            this.recordButton.GetComponentInChildren<Text>().text = "Record";
            this.playButton.GetComponentInChildren<Text>().text = "Play";
            this.loadButton.SetActive(true);
            this.saveButton.SetActive(true);
            this.playButton.SetActive(true);
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

            this.recordingSignifierUI.SetActive(false);
            this.playbackSignifierUI.SetActive(true);
            this.playButton.GetComponentInChildren<Text>().text = "Stop";
            this.loadButton.SetActive(false);
            this.saveButton.SetActive(false);
            this.recordButton.SetActive(false);
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

            this.recordingSignifierUI.SetActive(false);
            this.playbackSignifierUI.SetActive(false);
            this.recordButton.GetComponentInChildren<Text>().text = "Record";
            this.playButton.GetComponentInChildren<Text>().text = "Play";
            this.loadButton.SetActive(true);
            this.saveButton.SetActive(true);
            this.recordButton.SetActive(true);
        }
    }

    // Save recording to file
    public void SaveRecording() {
        // Only perform if in standby state
        // if(this._state == RecorderState.Standby && this._lastRecording != null) {
        //     string path = EditorUtility.SaveFilePanel("Save Recording", "", "recording.fasa", "fasa");
        //     if (path.Length != 0) {
        //         string output = JsonConvert.SerializeObject(this._lastRecording);   
        //         if (output != null) using (StreamWriter outputFile = new StreamWriter(path)) outputFile.WriteLine(output);
        //     }
        // }
        if(this._state == RecorderState.Standby && this._lastRecording != null) {
            StartCoroutine(SavingData());

        }

    }

    IEnumerator SavingData()
    {
        // this._lastRecording.Frames[this._lastRecording.Frames.Count-1].Timestamp.ToString()
        WWWForm form = new WWWForm();
        form.AddField("startTime",this._lastRecording.startTime.ToString("yyyy-MM-dd HH:mm:ss"));
        form.AddField("duration",this._lastRecording.Frames[this._lastRecording.Frames.Count-1].Timestamp.ToString());
        form.AddField("data",JsonConvert.SerializeObject(this._lastRecording));
        form.AddField("username", UserManager.username);

        WWW www = new WWW("http://localhost:8888/sqlconnect/saverecording.php", form);
        yield return www;

        Debug.Log(www.text);

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
