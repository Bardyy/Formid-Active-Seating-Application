using System.IO;
using System.IO.Ports;

public class PostureInputInterface {

    // ----- Private field attributes -----
    private SerialPort _serialPort;
    private int _stableReading = 0;

    // Constructor
    public PostureInputInterface(string portName, int baudRate) {
        try {
            this._serialPort = new SerialPort(portName, baudRate);
            this._serialPort.Open();
            this._serialPort.ReadTimeout = 1;

            if(this._serialPort.IsOpen) _stableReading = this._serialPort.ReadByte();
        }
        catch(System.Exception e) {
            /* Error handling */
        }
    }

    // Read singular byte from serial port
    public int GetInput() {
        if(this._serialPort.IsOpen) {
            try { 
                int input = this._serialPort.ReadByte();
                if(_stableReading != input) _stableReading = input;
            }
            catch(System.Exception) { 
                /* Error handling */ 
            }
        }
        return _stableReading;
    }
}
