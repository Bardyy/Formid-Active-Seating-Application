using System.IO;
using System.IO.Ports;

public class PostureInputInterface {

    public const int DATA_MASK = 0xFF;
    public const int INPUT_ERROR_MASK = 0x0100;

    // ----- Private field attributes -----
    private SerialPort _serialPort;
    private int _stableReading = 0;

    // Constructor
    public PostureInputInterface(string portName, int baudRate) {
        CreateConnection(portName, baudRate);
    }

    // Create a serial connection on port
    public void CreateConnection(string portName, int baudRate) {
        try {
            this._serialPort = new SerialPort(portName, baudRate);
            this._serialPort.Open();
            this._serialPort.ReadTimeout = 1;

            if(this._serialPort.IsOpen) _stableReading = this._serialPort.ReadByte();
        }
        catch(System.Exception) {
            _stableReading |= INPUT_ERROR_MASK;   // Add error if any
        }
    }

    // Read singular byte from serial port (Upper byte has error message bit mask, lower byte is the last stable reading)
    public int GetInput() {
        if(this._serialPort.IsOpen) {
            try { 
                int input = this._serialPort.ReadByte();
                if(input == 0) input = 1;               // TODO: Temporary adjustment
                if(_stableReading != input) _stableReading = (input - 1) & DATA_MASK;
            }
            catch(System.Exception) {
                _stableReading |= INPUT_ERROR_MASK;   // Add error if any
            }
        }
        return _stableReading;
    }

    // Get serial input data value
    public int GetSerialInputData(int reading) {
        return DATA_MASK & reading;
    }

    // Get input error flag
    public bool GetSerialInputError(int reading) {
        return (INPUT_ERROR_MASK & reading) != 0;
    }
}
