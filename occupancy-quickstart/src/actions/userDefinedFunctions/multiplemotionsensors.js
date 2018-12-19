// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Sample code for detecting motion in an area with multiple motion sensors.

var motionType = "Motion";
var spaceAvailFresh = "AvailableAndFresh";

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
       
       // Get all children sensors of the parent space
       var allSensors = parentSpace.ChildSensors();

       // Get all motion sensors of the parent space
       var motionSensors = allSensors.filter(function(item){
         return item.DataType === motionType;
       });

       // Is any of the motion sensors detecting motion?
       var motionDetected = motionSensors.find(function(element) {
         return element.Value().Value.toLowerCase() === "true";
       });

       // Set OccupancyStatus for the space
       var occupied = "Room is occupied";
       var empty = "Room is empty!";
                
       if (motionDetected != undefined)
       {
         // Motion is detected by at least one sensor
         log(`${occupied}. Motion Detected: ${motionDetected}. Sensors: ${motionSensors}.`);
         setSpaceValue(parentSpace.Id, spaceAvailFresh, occupied);
       }
       else {
         // No motion detected by any sensor
         log(`${empty}. Motion Not Detected: ${motionDetected}. Sensors: ${motionSensors}.`);
         setSpaceValue(parentSpace.Id, spaceAvailFresh, empty);
         parentSpace.Notify(JSON.stringify(empty));
       }
   }
   catch (error)
   {
      var errormsg = `An error has occurred processing the UDF Error: ${error.name} Message ${error.message}.`;
      log(errormsg);
      setSpaceValue(parentSpace.Id, spaceAvailFresh, errormsg);
   }
}
function getFloatValue(str) {
   if(!str) {
       return null;
   }
   return parseFloat(str);
}
