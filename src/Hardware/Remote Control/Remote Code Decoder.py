import time
import serial
from array import array

def setupSerial():
    global ser
    # Set up the serial connection
    ser = serial.Serial()
    ser.baudrate = 9600
    ser.parity = serial.PARITY_NONE
    # Change the port below to the port used by the remote.
    ser.port = 'COM20'
    ser.open()

def closeSerial():
    ser.close()

def receive():
    print "Receiving"
    # Send the start read command
    ser.write("1")
    # Read the results
    values = array("B", ser.read(10))
    print "Receive done."
    return values

def runTest():
    setupSerial()
    time.sleep(2)
    for i in range(20):
        values = receive()
        valuesString = []
        for value in values:
            valuesString.append('{:02x}'.format(value))
        print "CODE:",
        print '-'.join(valuesString)
    closeSerial()

runTest()
