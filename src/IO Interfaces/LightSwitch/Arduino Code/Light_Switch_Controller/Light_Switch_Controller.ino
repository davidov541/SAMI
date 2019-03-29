#include <Servo.h>
#include <SoftwareSerial.h>
#include <XBee.h>

SoftwareSerial xbeeSerial(2,3);

XBee xbee = XBee();
XBeeResponse response = XBeeResponse();
// create reusable response objects for responses we expect to handle 
Rx64Response rx64 = Rx64Response();

Servo servos[4];
int pinNumbers[4];
int midPos[4];
int onPos[4];
int offPos[4];
int currentAngle[4];
int servoCount;

void setup() 
{ 
  Serial.begin(9600);
  
  xbeeSerial.begin(9600);
  xbee.setSerial(xbeeSerial);

  servoCount = 0;
  Serial.println("Starting up");
  addSwitch(9);
  addSwitch(6);
  addSwitch(5);
  for (int i = 0; i < 4; i++){
    midPos[i] = 90;
    onPos[i] = 10;
    offPos[i] = 170;
  }
} 
 
 
void loop() 
{
  
  
    xbee.readPacket();
    
    if (xbee.getResponse().isAvailable()) {
      // got something
      
      if (xbee.getResponse().getApiId() == RX_64_RESPONSE) {
        // got a rx packet
        uint8_t cmd, arg0, arg1;
        xbee.getResponse().getRx64Response(rx64);
        cmd = rx64.getData(0);
        arg0 = rx64.getData(1);
        arg1 = rx64.getData(2);        
        
        Serial.println("Command Started");
        switch(cmd){
          case 0x01:
            Serial.println("Adding a switch");
            // Add switch
            // Arg 0: Pin number
            addSwitch(arg0);
            break;
    
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

void addSwitch(int pinNumber){
  pinNumbers[servoCount] = pinNumber;
  currentAngle[servoCount] = 90;
  servoCount++;
}

void turnOn(int index){
  moveSwitchTo(index, onPos[index]);
  moveSwitchTo(index, midPos[index]);
}

void turnOff(int index){
  moveSwitchTo(index, offPos[index]);
  moveSwitchTo(index, midPos[index]);
}



void moveSwitchTo(int index, int angle){
  servos[index].attach(pinNumbers[index]);
  
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
  
  servos[index].detach();
  
}
