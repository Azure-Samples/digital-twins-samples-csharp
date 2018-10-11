// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

var carbonDioxideType = "CarbonDioxide";
var motionType = "Motion";
var spaceAvailFresh = "AvailableAndFresh";
var carbonDioxideThreshold = 1000.0;
// Add your sensor type here

function process(telemetry, executionContext) {

    try {
        // Log SensorId and Message
        log(`Sensor ID: ${telemetry.SensorId}. `);
        log(`Sensor value: ${JSON.stringify(telemetry.Message)}.`);

        // Get sensor metadata
        var sensor = getSensorMetadata(telemetry.SensorId);

        // Retrieve the sensor reading
        var parseReading = JSON.parse(telemetry.Message);

        // Set the sensor reading as the current value for the sensor.
        setSensorValue(telemetry.SensorId, sensor.DataType, parseReading.SensorValue);

        // Get parent space
        var parentSpace = sensor.Space();

        // Get children sensors from the same space
        var otherSensors = parentSpace.ChildSensors();

        // Retrieve carbonDioxide, and motion sensors
        var carbonDioxideSensor = otherSensors.find(function(element) {
            return element.DataType === carbonDioxideType;
        });

        var motionSensor = otherSensors.find(function(element) {
            return element.DataType === motionType;
        });
        
        // Add your sensor variable here

        // get latest values for above sensors
        var motionValue = motionSensor.Value().Value;
        var presence = !!motionValue && motionValue.toLowerCase() === "true";
        var carbonDioxideValue = getFloatValue(carbonDioxideSensor.Value().Value);

        // Add your sensor latest value here
        
        // Return if no motion or carbonDioxide found return
        // Modify this line to monitor your sensor value
        if(carbonDioxideValue === null || motionValue === null) {
            sendNotification(telemetry.SensorId, "Sensor", "Error: Carbon dioxide or motion are null, returning");
            return;
        }

        // Modify these lines as per your sensor
        var availableFresh = "Room is available and air is fresh";
        var noAvailableOrFresh = "Room is not available or air quality is poor";

        // Modify this code block for your sensor
        // If carbonDioxide less than threshold and no presence in the room => log, notify and set parent space computed value
        if(carbonDioxideValue < carbonDioxideThreshold && !presence) {
            log(`${availableFresh}. Carbon Dioxide: ${carbonDioxideValue}. Presence: ${presence}.`);
            setSpaceValue(parentSpace.Id, spaceAvailFresh, availableFresh);
        }
        else {
            log(`${noAvailableOrFresh}. Carbon Dioxide: ${carbonDioxideValue}. Presence: ${presence}.`);
            setSpaceValue(parentSpace.Id, spaceAvailFresh, noAvailableOrFresh);

            // Set up custom notification for poor air quality
            parentSpace.Notify(JSON.stringify(noAvailableOrFresh));
        }
    }
    catch (error)
    {
        log(`An error has occurred processing the UDF Error: ${error.name} Message ${error.message}.`);
    }
}

function getFloatValue(str) {
  if(!str) {
      return null;
  }

  return parseFloat(str);
}
