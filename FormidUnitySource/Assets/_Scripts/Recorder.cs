using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    public GameObject recordingListView;
    public GameObject recordingListContent;
    
    // - - - - Private variables
    private RecorderState _state;
    private int _previousPosition = -1;         // Used for recording positions
    private int _playbackPosition = -1;         // Used for replaying positions
    private float _timeRelativeStart = 0.0f;    // Used for recording and replaying timestamps
    private int _playbackIndex = 0;             // Index in list of frames for playback

    private Recording _currentRecording;        // Tape that is being currently recorded
    private Recording _lastRecording;           // Tape that has finished being recorded (Can be played back, saved, or overwritten)

    private bool _showingOptions = false;

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
        this.recordingListView.SetActive(false);
        this._showingOptions = false;
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

    public void ToggleRecordingOptions() {
        if(this._showingOptions) HideRecordingOptions();
        else ShowRecordingOptions();
    }

    // Show buttons
    public void ShowRecordingOptions() {
        float sizeY = loadButton.GetComponent<RectTransform>().sizeDelta.y;
        Debug.Log(sizeY);
        int numButtons = 1;
        if(loadButton.activeSelf) {
            RectTransform r = loadButton.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(r.anchoredPosition.x, (sizeY / 2.0f) + sizeY * numButtons);
            numButtons++;
        }
        if(saveButton.activeSelf) {
            RectTransform r = saveButton.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(r.anchoredPosition.x, (sizeY / 2.0f) + sizeY * numButtons);
            numButtons++;
        }
        if(recordButton.activeSelf) {
            RectTransform r = recordButton.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(r.anchoredPosition.x, (sizeY / 2.0f) + sizeY * numButtons);
            numButtons++;
        }
        if(playButton.activeSelf) {
            RectTransform r = playButton.GetComponent<RectTransform>();
            r.anchoredPosition = new Vector2(r.anchoredPosition.x, (sizeY / 2.0f) + sizeY * numButtons);
            numButtons++;
        }
        this._showingOptions = true;
    }
    
    // Hide buttons
    public void HideRecordingOptions() {
        RectTransform r = loadButton.GetComponent<RectTransform>(); r.anchoredPosition = new Vector2(r.anchoredPosition.x, -50.0f);
        r = saveButton.GetComponent<RectTransform>(); r.anchoredPosition = new Vector2(r.anchoredPosition.x, -50.0f);
        r = recordButton.GetComponent<RectTransform>(); r.anchoredPosition = new Vector2(r.anchoredPosition.x, -50.0f);
        r = playButton.GetComponent<RectTransform>(); r.anchoredPosition = new Vector2(r.anchoredPosition.x, -50.0f);
        this._showingOptions = false;
    }

    // Enable recording list view
    public void EnableRecordingListView() {
        this.recordingListView.SetActive(true);
    }

    // Disable recording list view
    public void DisableRecordingListView() {
        this.recordingListView.SetActive(false);
    }

    // Return true if the current state is playback
    public bool InPlayback() {
        return this._state == RecorderState.Playback;
    }

    // Prompt the user for confirmation
    public bool PromptConfirmation(string title, string text, bool hasCancel) {
        return EditorUtility.DisplayDialog(title, text, "Ok", hasCancel ? "Cancel" : "");
    }

    // Return true if the current state is recording
    public bool InRecording() {
        return this._state == RecorderState.Recording;
    }

    // Toggle Recording
    public void ToggleRecording() {
        HideRecordingOptions();
        if(this._state == RecorderState.Standby) {
            StartRecording();
        }
        else if(this._state == RecorderState.Recording) {
            StopRecording();
        }
    }

    // Toggle Playback
    public void TogglePlayback() {
        HideRecordingOptions();
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
            if(this._lastRecording == null || PromptConfirmation("Overwrite Confirmation", "Would you like to overwrite the existing recording?", true)) {
                _timeRelativeStart = Time.time;
                this._lastRecording = null;
                this._currentRecording = new Recording();
                // Add first frame
                this._currentRecording.AddFrame(this.postureControl.Position, 0.0f);
                this._state = RecorderState.Recording;       // Set state to recording

                this.recordingSignifierUI.SetActive(true);
                this.playbackSignifierUI.SetActive(false);
                this.recordButton.GetComponentInChildren<TMP_Text>().text = "Stop";
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
            this.recordButton.GetComponentInChildren<TMP_Text>().text = "Record";
            this.playButton.GetComponentInChildren<TMP_Text>().text = "Play";
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
            this.playButton.GetComponentInChildren<TMP_Text>().text = "Stop";
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
            this.recordButton.GetComponentInChildren<TMP_Text>().text = "Record";
            this.playButton.GetComponentInChildren<TMP_Text>().text = "Play";
            this.loadButton.SetActive(true);
            this.saveButton.SetActive(true);
            this.recordButton.SetActive(true);
        }
    }

    // Save recording to file
    public void SaveRecording() {
        HideRecordingOptions();
        // Only perform if in standby state
        if(this._state == RecorderState.Standby && this._lastRecording != null && PromptConfirmation("Saving Alert", "This action will add the current recording to your user profile.", true)) {
            StartCoroutine(SavingData());
        }
    }

    // Get the list of recordings
    public void GetRecordingList() {
        HideRecordingOptions();
        // Only perform if in standby state
        if(this._state == RecorderState.Standby) {
            StartCoroutine(LoadingRecordingList());
        }
    }

    // Populate the recording list view
    public void PopulateRecordingListView(List<string> recordingList) {
        int offset = 0;
        float fontsize = 20.0f;
        foreach(string rec in recordingList) {
            string[] splitStr = rec.Split(',');
            string id = splitStr[0], name = splitStr[1];

            GameObject recEntry = new GameObject(name.Replace(" ", "-").Replace(":", "-"), typeof(RectTransform));
            TextMeshProUGUI recEntryText = recEntry.AddComponent<TextMeshProUGUI>();
            recEntryText.text = "Recording " + name;
            recEntryText.fontSize = fontsize;
            recEntry.transform.SetParent(recordingListContent.transform);

            RectTransform entryRectTransform = recEntry.GetComponent<RectTransform>();
            entryRectTransform.anchorMin = new Vector2(0, 1);
            entryRectTransform.anchorMax = new Vector2(0, 1);
            entryRectTransform.pivot = new Vector2(0, 1);
            entryRectTransform.anchoredPosition = new Vector2(5, -2.5f - fontsize * offset);
            entryRectTransform.sizeDelta = new Vector2(entryRectTransform.sizeDelta.x * 2, entryRectTransform.sizeDelta.y);
            offset++;
            
            Button button = recEntry.AddComponent<Button>();
            recEntry.GetComponent<Button>().onClick.AddListener(delegate() { 
                LoadRecording(id);
            });
        }

        RectTransform contentAreaRect = recordingListContent.transform.GetComponent<RectTransform>();
        contentAreaRect.sizeDelta = new Vector2(contentAreaRect.sizeDelta.x, 2.5f + fontsize * offset);
    }

    // Load recording from file
    public void LoadRecording(string recordingId) {
        // Only perform if in standby state
        if(this._state == RecorderState.Standby) {
            StartCoroutine(LoadRecordingData(recordingId));
        }
    }

    // - - - - Database co-routines - - - -

    // Save to database
    IEnumerator SavingData() {
        WWWForm form = new WWWForm();
        form.AddField("startTime",this._lastRecording.startTime.ToString("yyyy-MM-dd HH:mm:ss"));
        form.AddField("duration",this._lastRecording.Frames[this._lastRecording.Frames.Count-1].Timestamp.ToString());
        form.AddField("data",JsonConvert.SerializeObject(this._lastRecording));
        form.AddField("username", UserManager.username);

        WWW www = new WWW("http://localhost:8888/sqlconnect/saverecording.php", form);
        yield return www;

        Debug.Log(www.text);
    }

    // Load list from database
    IEnumerator LoadingRecordingList() {
        WWWForm form = new WWWForm();
        form.AddField("username", UserManager.username);
        WWW www = new WWW("http://localhost:8888/sqlconnect/getrecordinglist.php", form);
        yield return www;

        if(www.text != "No recording found") {
            List<string> list = new List<string>(www.text.Split('.'));
            list.RemoveAt(list.Count - 1);
            PopulateRecordingListView(list);
            if(list.Count > 0) EnableRecordingListView();
        }
        else PromptConfirmation("Loading Alert", "Could not find recordings for this user.", false);
    }

    // Load recording from database
    IEnumerator LoadRecordingData(string recordingId) {
        WWWForm form = new WWWForm();
        form.AddField("recordingId", recordingId);
        WWW www = new WWW("http://localhost:8888/sqlconnect/getrecordingdata.php", form);
        yield return www;

        if(www.text != "No recording found") {
            Recording loadedRecording = JsonConvert.DeserializeObject<Recording>(www.text);
            if(loadedRecording.Frames.Count > 0 && (this._lastRecording == null || PromptConfirmation("Overwrite Confirmation", "Would you like to overwrite the existing recording?", true))) {
                this._lastRecording = loadedRecording;
                DisableRecordingListView();
            }
        }
        else PromptConfirmation("Loading Alert", "An error has occured.", false);
    }
}
