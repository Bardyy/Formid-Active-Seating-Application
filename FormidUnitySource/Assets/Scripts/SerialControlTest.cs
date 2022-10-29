using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;
using System.IO.Ports;

public class SerialControlTest : MonoBehaviour
{
    public string portValue;
    public int speed = 50;

    private SerialPort serialPort;

    // Input values
    private bool leftPressed;
    private bool rightPressed;

    void Start()
    {
        serialPort = new SerialPort(portValue, 9600);
        serialPort.Open();
        serialPort.ReadTimeout = 1;
    }

    void Update()
    {
        leftPressed = false;
        rightPressed = false;

        // Serial input
        int input = 0;
        if(serialPort.IsOpen)
        {
            try { input = serialPort.ReadByte(); }
            catch(System.Exception) { /* Nothing */ }
        }
        
        if(input == 1) leftPressed = true;
        if(input == 2) rightPressed = true;
    }

    void LateUpdate()
    {
        if (leftPressed) transform.Translate(-speed * Time.deltaTime, 0, 0);
        if (rightPressed) transform.Translate(speed * Time.deltaTime, 0, 0);
    }
}
