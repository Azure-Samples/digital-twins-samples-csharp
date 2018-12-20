// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// Sample code for detecting motion in an area with multiple motion sensors:
// This code is designed to work with the existing sample code. 
// To test this end to end, make the following changes:
// 1) In the provisionSample.yaml, change the "script" key to point to this file (multiplemotionsensors.js).
// 2) In the provisionSample.yaml, add a couple more motion sensors. For example, the sample "sensors" node could look like this:
//        sensors:
//        - dataType: Motion
//          hardwareId: SAMPLE_SENSOR_MOTION_ONE
//        - dataType: CarbonDioxide
//          hardwareId: SAMPLE_SENSOR_CARBONDIOXIDE
//        - dataType: Motion
//          hardwareId: SAMPLE_SENSOR_MOTION_TWO
//        - dataType: Motion
//          hardwareId: SAMPLE_SENSOR_MOTION_THREE
// 3) Add these additional motion sensors to the device-connectivity project's appSettings.json. For example:
//    "Sensors": [{
//      "DataType": "Motion",
//      "HardwareId": "SAMPLE_SENSOR_MOTION_ONE"
//    },{
//      "DataType": "CarbonDioxide",
//      "HardwareId": "SAMPLE_SENSOR_CARBONDIOXIDE"
//    },{
//      "DataType": "Motion",
//      "HardwareId": "SAMPLE_SENSOR_MOTION_TWO"
//    },{
//      "DataType": "Motion",
//      "HardwareId": "SAMPLE_SENSOR_MOTION_THREE"
//    }]
// 
// 


var motionType = "Motion";
var occupancyStatus = "AvailableAndFresh";

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
         setSpaceValue(parentSpace.Id, occupancyStatus, occupied);
       }
       else {
         // No motion detected by any sensor
         log(`${empty}. Motion Not Detected: ${motionDetected}. Sensors: ${motionSensors}.`);
         setSpaceValue(parentSpace.Id, occupancyStatus, empty);
         // You could try creating a Logic App mail notification in case none of the motion sensors
         // detect anything. In that case, uncomment the following line:
         //  parentSpace.Notify(JSON.stringify(empty));
       }
   }
   catch (error)
   {
      var errormsg = `An error has occurred processing the UDF Error: ${error.name} Message ${error.message}.`;
      log(errormsg);
      setSpaceValue(parentSpace.Id, occupancyStatus, errormsg);
   }
}
