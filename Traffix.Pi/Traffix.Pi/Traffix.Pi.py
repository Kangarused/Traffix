import RPi.GPIO as GPIO
from datetime import datetime as TIME

GPIO.setmode(GPIO.BCM)

# White Data Input
GPIO.setup(17, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)

# Blue Data Input
GPIO.setup(12, GPIO.IN, pull_up_down=GPIO.PUD_DOWN)

# Ready Light Output
#GPIO.setwarnings(False)
#GPIO.setup(21, GPIO.OUT)

# Global Variables
firstTriggered = False
firstTimestamp = TIME.now()

secondTriggered = False
secondTimestamp = TIME.now()

hasVehicle = False
speed = 0
distance = 0.001

# Event for first trigger
def FirstTrigger(channel):
    # Declare globals
    global hasVehicle
    global firstTimestamp
    global firstTriggered

    if (hasVehicle == False):
        if (firstTriggered == False):
            firstTimestamp = TIME.now()
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
            secondTimestamp = TIME.now()
            secondTriggered = True

    if (firstTriggered == False):
        if (secondTriggered == True):
            secondTriggered = False
            hasVehicle = False

GPIO.add_event_detect(17, GPIO.RISING, callback=FirstTrigger, bouncetime=100)
GPIO.add_event_detect(12, GPIO.RISING, callback=SecondTrigger, bouncetime=100)
#GPIO.output(21, GPIO.HIGH)
while True:
    if (firstTriggered == True):
        if (secondTriggered == True):
            # Get time offset and convert to hours
            delta = secondTimestamp - firstTimestamp
            timeOffset = (float(float(delta.total_seconds() / 60) / 60))
            
            # Calculate and print speed
            speed = float(distance) / float(timeOffset)
            print("speed: " + str(speed))
            print("")

            hasVehicle = True
            firstTriggered = False

#Cleanup ports
GPIO.cleanup()
