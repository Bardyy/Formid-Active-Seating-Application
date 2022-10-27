using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Ports;


public class test : MonoBehaviour
{
    public SerialPort serialPort; 
    // Start is called before the first frame update
    void Start()
    {
        serialPort = new SerialPort("COMPORT", 9600);
        serialPort.Open();
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
