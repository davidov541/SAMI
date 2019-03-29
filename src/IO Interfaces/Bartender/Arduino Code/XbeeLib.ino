#include <SoftwareSerial.h>

SoftwareSerial xbeeSerial(2,3);
int xbeeDeviceNumber;
int xbeeBuffer[50];
int xbeeBufferCount;
int xbeeError = 0;

void startXbee(int deviceNum){
  xbeeDeviceNumber = deviceNum;
  xbeeSerial.begin(9600);
  xbeeSerial.listen();
}

int getXbeeBufferCount(){
  return xbeeBufferCount;
}

int getXbeeBuffer(int index){
  return xbeeBuffer[index];
}

int XbeeReadByte(){
  while (xbeeSerial.available() == 0);
  char returned = xbeeSerial.read();
  ///////// DEBUG!
  //Serial.println(returned);
  /////////
  //xbeeSerial.write(returned);
  return returned;
}

int XbeeReadHex(){
  int byte1, byte2;
  byte1 = XbeeReadByte();
  if (byte1 == '\r' || byte1 == '\n'){
    return -1;
  }
  byte2 = XbeeReadByte();
  
  if ((byte1 >= '0') && (byte1 <= '9')) {
    byte1 = byte1 - '0';
  } else if ((byte1 >= 'A') && (byte1 <= 'F')) {
    byte1 = (byte1 - 'A') + 10;
  } else if ((byte1 >= 'a') && (byte1 <= 'f')) {
    byte1 = (byte1 - 'a') + 10;
  } else {
    return -2;
  }
  
  if ((byte2 >= '0') && (byte2 <= '9')) {
    byte2 = byte2 - '0';
  } else if ((byte2 >= 'A') && (byte2 <= 'F')) {
    byte2 = (byte2 - 'A') + 10;
  } else if ((byte2 >= 'a') && (byte2 <= 'f')) {
    byte2 = (byte2 - 'a') + 10;
  } else {
    return -2;
  }
  
  return (byte1 << 4) | byte2;
  
}

void XbeeWaitForCommand(){
  int incomingByte;
  
  // Wait for the correct device
  boolean foundDevice = false;
  boolean foundCmd = false;
  do {
    // Loop until a complete command has been found
    do {
      // Wait for the start of a command directed at
      // this device
      // Command starts with S, then a 2 byte
      // ASCII encoded hex
      while ((incomingByte = XbeeReadByte()) != 'S');
      foundDevice = XbeeReadHex() == xbeeDeviceNumber;
    } while (foundDevice == false);
    // Get the payload
    xbeeBufferCount = 0;
    do {
      incomingByte = XbeeReadHex();
      xbeeBuffer[xbeeBufferCount] = incomingByte;
      xbeeBufferCount++;
      if (incomingByte == -1) {
        foundCmd = true;
      }
    } while ((incomingByte >= 0) &&
             (xbeeBufferCount != 50));
    
  } while (foundCmd == false);
  
}
