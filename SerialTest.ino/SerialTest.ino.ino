#define PIN_NUMBER 2
#define MAX_POSTURE 7
#define BAUD_RATE 115200

int posture = 0;
int lastState = 0;

void setup() {
  Serial.begin(BAUD_RATE);
  pinMode(PIN_NUMBER, INPUT);
}

void loop() {
  // Read the input pin
  int currentState = digitalRead(PIN_NUMBER);
  
  // Increment posture on rising edge
  if(lastState == 0 && currentState == 1) posture ++;
  if(posture > MAX_POSTURE) posture = 0;

  // Set previous state
  lastState = currentState;

  // Write posture value to serial port
  Serial.write(posture);
//  Serial.println(posture);
  delay(20);
}
