/*
 * IRrecord: record and play back IR signals as a minimal 
 * An IR detector/demodulator must be connected to the input RECV_PIN.
 * An IR LED must be connected to the output PWM pin 3.
 * A button must be connected to the input BUTTON_PIN; this is the
 * send button.
 * A visible LED can be connected to STATUS_PIN to provide status.
 *
 * The logic is:
 * If the button is pressed, send the IR code.
 * If an IR code is received, record it.
 *
 * Version 0.11 September, 2009
 * Copyright 2009 Ken Shirriff
 * http://arcfn.com
 */

#include <IRremote.h>

int RECV_PIN = 11;
int BUTTON_PIN = 12;
int STATUS_PIN = 13;

IRrecv irrecv(RECV_PIN);
IRsend irsend;

decode_results results;

void setup()
{
  Serial.begin(9600);
  //irrecv.enableIRIn(); // Start the receiver
  pinMode(BUTTON_PIN, INPUT);
  pinMode(STATUS_PIN, OUTPUT);
}

// Storage for the recorded code
int toggle = 0; // The RC5/6 toggle state

int lastButtonState;

int readByte(){
  while (Serial.available() == 0);
  return Serial.read();
}

void cmdSendCode(){
  //boolean repeat = false;
  decode_results results;
  results.decode_type = readByte();
  results.value = readByte();
  results.value = (results.value << 8) | readByte();
  results.value = (results.value << 8) | readByte();
  results.value = (results.value << 8) | readByte();
  results.value = (results.value << 8) | readByte();
  results.value = (results.value << 8) | readByte();
  results.value = (results.value << 8) | readByte();
  results.value = (results.value << 8) | readByte();
  results.bits = readByte();
  
  if (results.decode_type == NEC) {
    //if (repeat) {
    //   irsend.sendNEC(REPEAT, results.bits);
    //} 
    //else {
      for (int i = 0; i < 4; i++){
       irsend.sendNEC(results.value, results.bits);
      }
    //}
  } 
  else if (results.decode_type == SONY) {
    for (int i = 0; i < 4; i++){
      irsend.sendSony(results.value, results.bits);
    }
  } 
  else if (results.decode_type == RC5) {
    toggle = 1 - toggle;
    // Put the toggle bit into the code to send
    results.value = results.value & ~(1 << (results.bits - 1));
    results.value = results.value | (toggle << (results.bits - 1));
    for (int i = 0; i < 4; i++){
      irsend.sendRC5(results.value, results.bits);
    }
  }
  else if (results.decode_type == RC6) {
    toggle = 1 - toggle;
    
    if (toggle == 0) {
      irsend.sendRC6(results.value, results.bits);
    } else {
      irsend.sendRC6(results.value ^ 0x8000, results.bits);
    }
  }
  else if (results.decode_type == RCMM) {
    
    toggle = 1 - toggle;
    
    for (int i = 0; i < 2; i++){
      if (toggle == 0) {
        irsend.sendRCMM(results.value, results.bits);
      } else {
        irsend.sendRCMM(results.value ^ 0x8000, results.bits);
      }
    }
  }
  
  // Wait 300ms as a pause
  delay(300);
  // Tell sami it's done sending
  Serial.write(0xBB);
}

void cmdReceiveCode(){
  decode_results results;
  results.decode_type = 0;
  results.value = 0LL;
  results.bits = 0;
  
  irrecv.resume(); // resume receiver
  
  digitalWrite(STATUS_PIN, HIGH);
  while (!irrecv.decode(&results));
  
  Serial.write(results.decode_type & 0x00FF);
  
  Serial.write((results.value >> 56) & 0x000000FFLL);
  Serial.write((results.value >> 48) & 0x000000FFLL);
  Serial.write((results.value >> 40) & 0x000000FFLL);
  Serial.write((results.value >> 32) & 0x000000FFLL);
  Serial.write((results.value >> 24) & 0x000000FFLL);
  Serial.write((results.value >> 16) & 0x000000FFLL);
  Serial.write((results.value >> 8) & 0x000000FFLL);
  Serial.write(results.value & 0x000000FFLL);
  
  Serial.write(results.bits & 0x00FF);
  
  digitalWrite(STATUS_PIN, LOW);
  
}

void loop() {
  
  int command;
  command = readByte();
  switch (command) {
    case 0x20:
      Serial.write(0xBB);
      break;
    case 0x30:
      cmdSendCode();
      break;
    case 0x31:
      cmdReceiveCode();
      break;
  }
  
}
