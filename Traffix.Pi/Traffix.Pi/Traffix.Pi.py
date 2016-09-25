import RPi.GPIO as GPIO
import requests
import random
import time
import threading
from datetime import datetime

GPIO.setmode(GPIO.BCM)

# White Data Input
GPIO.setup(17, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)
# Blue Data Input
GPIO.setup(12, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)

# Ready Light Output
GPIO.setwarnings(False)
GPIO.setup(21, GPIO.OUT)

class MeterData:
    def __init__(self, firstTime, secondTime):
        self.firstTime = firstTime
        self.secondTime = secondTime
        

# Global Variables
meterList = range(1, 8)
key = '#123Traffix654@'
firstTriggered = False
firstTimestamp = datetime.now()

secondTriggered = False
secondTimestamp = datetime.now()

hasVehicle = False
distance = 0.001
requestList = []

# Event for first trigger
def FirstTrigger(channel):
    # Declare globals
    global hasVehicle
    global firstTimestamp
    global firstTriggered

    if (firstTriggered == False):
        firstTimestamp = datetime.now()
        firstTriggered = True

# Event for second trigger
def SecondTrigger(channel):
    # Declare globals
    global hasVehicle
    global firstTriggered
    global secondTimestamp
    global secondTriggered
    
    if (firstTriggered == True):
        if (secondTriggered == False):
            secondTimestamp = datetime.now()
            secondTriggered = True

def ConnectSession():
    global session
    try:
        session.get('https://www.austraffix.com/', verify=False)
        return True
    except Exception as ex:
        return False

def SendDataToServer():
    global requestList
    global meterList
    global distance
    global session

    while True:
        if (len(requestList) > 0):
            # Pop the first request out of the list
            data = requestList.pop(0)
            
            # Get time offset and convert to hours
            delta = data.secondTime - data.firstTime
            timeOffset = (float(float(delta.total_seconds() / 60) / 60))
                    
            # Calculate and print speed
            speed = float(distance) / float(timeOffset)
            print(speed)

            # Send data to server
            meterId = random.choice(meterList)
            print('Sending Request => ' + str(datetime.now()))
            r = session.post('https://www.austraffix.com/api/update/insertlog',data = {'Key':key, 'MeterId':meterId, 'Timestamp':secondTimestamp, 'Speed':speed}, verify=False)
            print('Request Sent => ' + str(datetime.now()))
            

def AddEventToQueue():
    global firstTimestamp
    global secondTimestamp
    print('Added Request to Queue')
    requestList.append(MeterData(firstTimestamp, secondTimestamp))

GPIO.add_event_detect(17, GPIO.RISING, callback=FirstTrigger, bouncetime=200)
GPIO.add_event_detect(12, GPIO.RISING, callback=SecondTrigger, bouncetime=200)

session = requests.Session()
while (ConnectSession() == False):
    print('Attempting to connect to server')
    time.sleep(2)
    continue;

# Start Thread that will send requests
t = threading.Thread(target=SendDataToServer)
t.start()

# LED Blink twice to show that the program has started and has connected to the server
GPIO.output(21, GPIO.LOW)
time.sleep(0.2)
GPIO.output(21, GPIO.HIGH)
time.sleep(0.2)
GPIO.output(21, GPIO.LOW)
time.sleep(0.2)
GPIO.output(21, GPIO.HIGH)
try:
    while True:
        #if (hasVehicle == False):
        if (firstTriggered == True):
            if (secondTriggered == True):
                AddEventToQueue()
                # Set active vehicle (to ignore rear wheels crossing)
                #hasVehicle = True
                firstTriggered = False
                secondTriggered = False
except KeyboardInterrupt:
    GPIO.output(21, GPIO.LOW)
    GPIO.cleanup()

#Cleanup ports
GPIO.cleanup()
