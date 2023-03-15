using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class Register : MonoBehaviour
{
    public TMP_Text emailInputField;
    public TMP_Text firstNameInputField;
    public TMP_Text lastNameInputField;
    public TMP_Text userNameInputField;
    public TMP_InputField passwordInputField;

    public Button RegisterSubmitButton;

    public void CallRegister()
    {
        StartCoroutine(Registering());
    }

    IEnumerator Registering()
    {
        WWWForm form = new WWWForm();
        passwordInputField.contentType = TMP_InputField.ContentType.Standard;
        form.AddField("firstname",firstNameInputField.text);
        form.AddField("lastname",lastNameInputField.text);
        form.AddField("email",emailInputField.text);
        form.AddField("password",passwordInputField.text);
        form.AddField("username",userNameInputField.text);
        WWW www = new WWW("http://localhost:8888/sqlconnect/register.php", form);
        yield return www;

        if(www.text == "0")
        {
            Debug.Log("User created successfully.");
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        if(www.text == "4: Insert user query failed")
        {
            Debug.Log("User not created.");
    
        }
        if(www.text == "User already exists!"){
            Debug.Log("User already exists, try another username!");
        }

    }

    public void BackButton(){
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
