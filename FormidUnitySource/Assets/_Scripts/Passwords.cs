using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Passwords : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField password;
    void Start()
    {
         password.contentType = TMP_InputField.ContentType.Password;
    }



}
