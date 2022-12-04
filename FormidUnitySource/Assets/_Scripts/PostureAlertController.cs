using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PostureAlertController : MonoBehaviour {

    enum AlertState { Disabled, Enabled };

    // - - - - Public variables
    public BodyController postureControl;
    public GameObject alertBox;
    
    // - - - - Private variables
    private AlertState _state;
    private float _previousAlertTime;
    private int _currentPosture = -1;

    void Awake() {
        this._state = AlertState.Disabled;
        HideAlert();
        ResetTimer();
        ToggleAlertState();
    }

    void Update() {
        // Only if the state is currently enabled
        if(this._state == AlertState.Enabled) {
            // Reset timer when posture changes
            if(this.postureControl.Position != _currentPosture) {
                _currentPosture = this.postureControl.Position;
                ResetTimer();
            }

            // Display alert when timer exceeds posture timeout
            if(Time.time - _previousAlertTime >= this.postureControl.Positions.Postures[this.postureControl.Position].AlertTimeout) {
                DisplayAlert();
                ResetTimer();
            }
        }
    }

    // ------------------------- Helper methods -------------------------

    // Reset timer to current time
    public void ResetTimer() {
        _previousAlertTime = Time.time;
    }

    // Toggle alert state
    public void ToggleAlertState() {
        if(this._state == AlertState.Disabled) this._state = AlertState.Enabled;
        else if(this._state == AlertState.Enabled) this._state = AlertState.Disabled;
        ResetTimer();
    }

    // Making alert box visible
    public void DisplayAlert(){
        this.alertBox.SetActive(true);
    }

    // Making alert box invisible
    public void HideAlert(){
        this.alertBox.SetActive(false);
    }
}