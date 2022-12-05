using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class PostureAlertController : MonoBehaviour {

    enum AlertState { Disabled, Enabled };

    // - - - - Public variables
    public BodyController postureControl;
    public AudioSource alertAudio;
    public GameObject alertBox;
    public GameObject toggleAlerts;
    public float alertBoxSpeed = 5.0f;
    
    // - - - - Private variables
    private AlertState _state;
    private float _previousAlertTime;
    private int _currentPosture = -1;
    private float _alertMessageMovement = 0.0f;

    void Awake() {
        this._state = AlertState.Disabled;
        HideAlert();
        ResetTimer();
        ToggleAlertState();
    }

    // Update method for input handling
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

    // LateUpdate for auto-adjustments handled by engine
    public void LateUpdate() {
        // Move alert box if movement is true
        if(_alertMessageMovement != 0) this.alertBox.GetComponent<RectTransform>().anchoredPosition += new Vector2(_alertMessageMovement, 0);

        // Stop movement when edge reached
        if(_alertMessageMovement < 0 && this.alertBox.GetComponent<RectTransform>().anchoredPosition.x <= -this.alertBox.GetComponent<RectTransform>().rect.width / 2.0f) {
            _alertMessageMovement = 0;
            this.alertBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(-this.alertBox.GetComponent<RectTransform>().rect.width / 2.0f, this.alertBox.GetComponent<RectTransform>().anchoredPosition.y);
        }
        else if (_alertMessageMovement > 0 && this.alertBox.GetComponent<RectTransform>().anchoredPosition.x >= this.alertBox.GetComponent<RectTransform>().rect.width / 2.0f) {
            _alertMessageMovement = 0;
            this.alertBox.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.alertBox.GetComponent<RectTransform>().rect.width / 2.0f, this.alertBox.GetComponent<RectTransform>().anchoredPosition.y);
        }
    }

    // ------------------------- Helper methods -------------------------

    // Reset timer to current time
    public void ResetTimer() {
        _previousAlertTime = Time.time;
    }

    // Toggle alert state
    public void ToggleAlertState() {
        if(this._state == AlertState.Disabled){
            this._state = AlertState.Enabled;
            this.toggleAlerts.GetComponentInChildren<TMP_Text>().text = "Disable Alerts";
        } 
        else if(this._state == AlertState.Enabled){
            this._state = AlertState.Disabled;
            this.toggleAlerts.GetComponentInChildren<TMP_Text>().text = "Enable Alerts";
        } 
        ResetTimer();
    }

    // Making alert box visible
    public void DisplayAlert(){
        _alertMessageMovement = -alertBoxSpeed;
        PlayAlert();
    }

    // Making alert box invisible
    public void HideAlert(){
        _alertMessageMovement = alertBoxSpeed;
    }
    
    // Play alert sound
    public void PlayAlert() {
        this.alertAudio.Play();
    }
}