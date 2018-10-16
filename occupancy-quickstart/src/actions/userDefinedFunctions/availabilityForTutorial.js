// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Sample code for the "Monitor a building with Digital Twins" tutorials:

var carbonDioxideType = "CarbonDioxide";
var motionType = "Motion";
var spaceAvailFresh = "AvailableAndFresh";
var carbonDioxideThreshold = 1000.0;
var temperatureType = "Temperature";
var temperatureThreshold = 78;

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
       
       // Retrieve carbonDioxide, temperature, and motion sensors
       var carbonDioxideSensor = otherSensors.find(function(element) {
           return element.DataType === carbonDioxideType;
       });
        var motionSensor = otherSensors.find(function(element) {
           return element.DataType === motionType;
       });
       var temperatureSensor = otherSensors.find(function(element) {
           return element.DataType === temperatureType;
       });
 
       // get latest values for above sensors
       var motionValue = motionSensor.Value().Value;
       var presence = !!motionValue && motionValue.toLowerCase() === "true";
       var carbonDioxideValue = getFloatValue(carbonDioxideSensor.Value().Value);
       var temperatureValue = getFloatValue(temperatureSensor.Value().Value);
       
       // Return if no motion, temperature, or carbonDioxide found
       if(carbonDioxideValue === null || motionValue === null || temperatureValue === null){
           sendNotification(telemetry.SensorId, "Sensor", "Error: Carbon dioxide, motion, or temperature are null, returning");
           return;
       }
        
       var alert = "Room with fresh air and comfortable temperature is available.";
       var noAlert = "Either room is occupied, or working conditions are not right.";
    
       // If sensor values are within range and room is available
       if(carbonDioxideValue < carbonDioxideThreshold && temperatureValue < temperatureThreshold && !presence) {
           log(`${alert}. Carbon Dioxide: ${carbonDioxideValue}. Temperature: ${temperatureValue}. Presence: ${presence}.`);
           
           // log, notify and set parent space computed value
           setSpaceValue(parentSpace.Id, spaceAvailFresh, alert);
           
           // Set up notification for this alert
           parentSpace.Notify(JSON.stringify(alert));
       }
       else {
           log(`${noAlert}. Carbon Dioxide: ${carbonDioxideValue}. Temperature: ${temperatureValue}. Presence: ${presence}.`);
           
           // log, notify and set parent space computed value
           setSpaceValue(parentSpace.Id, spaceAvailFresh, noAlert);
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
