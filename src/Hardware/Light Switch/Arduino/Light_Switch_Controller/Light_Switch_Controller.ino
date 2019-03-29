#include <Servo.h>
#include <SoftwareSerial.h>
#include <XBee.h>

// Set up the serial connection to the XBee
SoftwareSerial xbeeSerial(2,3);

// create reusable response objects for responses we expect to handle 
XBee xbee = XBee();
XBeeResponse response = XBeeResponse();
Rx64Response rx64 = Rx64Response();

#define servoCount 3

Servo servos[servoCount];
int pinNumbers[servoCount];
int midPos[servoCount];
int onPos[servoCount];
int offPos[servoCount];
int currentAngle[servoCount];

void setup() 
{ 
  Serial.begin(9600);
  
  xbeeSerial.begin(9600);
  xbee.setSerial(xbeeSerial);

  Serial.println("Starting up");
  
  pinNumbers[0] = 9; // Switch 0 is connected to pin 9
  pinNumbers[1] = 6; // Switch 1 is connected to pin 6
  pinNumbers[2] = 5; // Switch 2 is connected to pin 5
  
  for (int i = 0; i < servoCount; i++){
    // Set default middle, on, and off angles.
    midPos[i] = 90;
    onPos[i] = 10;
    offPos[i] = 170;
    currentAngle[i] = midPos[i];
  }
} 
 
 
void loop() 
{
  
    xbee.readPacket();
    
    if (xbee.getResponse().isAvailable()) {
      // We received a packet.
      
      if (xbee.getResponse().getApiId() == RX_64_RESPONSE) {
        // The packet is a receive packet
        uint8_t cmd, arg0, arg1;
        xbee.getResponse().getRx64Response(rx64);
        cmd = rx64.getData(0);
        arg0 = rx64.getData(1);
        arg1 = rx64.getData(2);        
        
        Serial.println("Command Started");
        switch(cmd){
          case 0x02:
            Serial.println("Moving to angle" + arg1);
            // Move servo to angle
            // Arg 0: Servo number
            // Arg 1: Angle (0-180)
            moveSwitchTo(arg0, arg1);
            break;
          
          case 0x03:
            Serial.println("Setting midpoint");
            // Set midpoint of servo
            // Arg 1: Servo number
            // Arg 2: Angle (0-180)
            midPos[arg0] = arg1;
            break;
    
          case 0x04:
            Serial.println("Setting on point");
            // Set on point of servo
            // Arg 1: Servo number
            // Arg 2: Angle (0-180)
            onPos[arg0] = arg1;
            break;
    
          case 0x05:
            Serial.println("Setting off point");
            // Set off point of servo
            // Arg 1: Servo number
            // Arg 2: Angle (0-180)
            offPos[arg0] = arg1;
            break;
      
          case 0x06:
            Serial.println("Turning on");
            // Set off point of servo
            // Arg 1: Servo number
            turnOn(arg0);
            break;
      
          case 0x07:
            Serial.println("Turning off");
            // Set off point of servo
            // Arg 1: Servo number
            turnOff(arg0);
            break;
    
        }
        
      } else {
      	// not something we were expecting
        Serial.println("Recieved an unexpected packet");
      }
    } else if (xbee.getResponse().isError()) {
      Serial.print("Error reading packet.  Error code: ");  
      Serial.println(xbee.getResponse().getErrorCode());
    } 
    
}

void turnOn(int index){
  if (index < servoCount){
    moveSwitchTo(index, onPos[index]);
    moveSwitchTo(index, midPos[index]);
  }
}

void turnOff(int index){
  if (index < servoCount){
    moveSwitchTo(index, offPos[index]);
    moveSwitchTo(index, midPos[index]);
  }
}

void moveSwitchTo(int index, int angle){
  if (index < servoCount){
    // Turn on the servo
    servos[index].attach(pinNumbers[index]);
    
    // Step up the angle so it does not instantly jump
    // up to the desired angle
    while (angle != currentAngle[index]){
      int stepAngle = angle - currentAngle[index];
      if (stepAngle > 1){
        stepAngle = 1;
      } else if (stepAngle < -1){
        stepAngle = -1;
      }
      currentAngle[index] += stepAngle;
      servos[index].write(currentAngle[index]);
      delay(5);
    }
    
    // Turn off the servo to save energy
    servos[index].detach();
  }
}
