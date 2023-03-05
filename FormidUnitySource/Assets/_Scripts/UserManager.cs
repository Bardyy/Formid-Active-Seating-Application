using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserManager : MonoBehaviour
{
    public static string username;
    public TMP_Text usernameUI;
    public TMP_Text logoutButton;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        usernameUI.text = username;
    }

    public void LogOut(){
        username = "";
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
