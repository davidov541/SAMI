#include <Servo.h>
#include <SoftwareSerial.h>
#include <XBee.h>

SoftwareSerial xbeeSerial(2,3);

XBee xbee = XBee();
XBeeResponse response = XBeeResponse();
// create reusable response objects for responses we expect to handle 
Rx64Response rx64 = Rx64Response();

// constants won't change. Used here to 
// set pin numbers:
int pumpPins[] = {13, 12, 11, 10, 7, 6, 5, 4};
int pumpCount = 8;

void setup() {
  // Setup the serial port
  Serial.begin(9600);
  Serial.println("Starting up");
  // set the digital pin as output:
  for (int i = 0; i < pumpCount; i++){
    pinMode(pumpPins[i], OUTPUT);
  }
  // Setup the XBee
  xbeeSerial.begin(9600);
  xbee.setSerial(xbeeSerial);
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
      int pumpIndex = rx64.getData(0);
      unsigned long pumpTimeMs = 0;
      for (int i = 1; i <= 4; i++){
        pumpTimeMs = (pumpTimeMs << 8) | (rx64.getData(i) & 0xFF);
      }
      Serial.println("Pump index:");
      Serial.println(pumpIndex);
      Serial.println("Pump time:");
      Serial.println(pumpTimeMs);
      pumpLiquid(pumpIndex, pumpTimeMs);
    } else {
    	// not something we were expecting
      Serial.println("Recieved an unexpected packet");
    }
  } else if (xbee.getResponse().isError()) {
    Serial.print("Error reading packet.  Error code: ");  
    Serial.println(xbee.getResponse().getErrorCode());
  }
}

void pumpLiquid(int pumpIndex, unsigned long pumpTimeMs){
  digitalWrite(pumpPins[pumpIndex], HIGH);
  delay(pumpTimeMs);
  digitalWrite(pumpPins[pumpIndex], LOW);
}
