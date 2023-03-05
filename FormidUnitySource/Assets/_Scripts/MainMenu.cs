using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_Text usernameInputField;
    public TMP_InputField passwordInputField;


    public void CallLogin()
    {
        StartCoroutine(LoginUser());
    }

    IEnumerator LoginUser()
    {
        WWWForm form1 = new WWWForm();
        passwordInputField.contentType = TMP_InputField.ContentType.Standard;
        form1.AddField("username",usernameInputField.text);
        form1.AddField("password",passwordInputField.text);
        
        string username = usernameInputField.text;
        PlayerPrefs.SetString("username", username);
        UserManager.username = username;

        WWW www = new WWW("http://localhost:8888/sqlconnect/login.php", form1);
        yield return www;
        if(www.text == "success")
        {
            Debug.Log("User Login Successful.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(2);
        }

        if(www.text == "invalid")
        {
            Debug.Log("Invalid username or pass");
    
        }
        if(www.text == "No user exists")
        {
            Debug.Log("No user exists");
    
        }


}
}
