var CarbonDioxideType = "CarbonDioxide";
var motionType = "Motion";
var spaceAirQuality = "AvailableAndFresh";

function process(telemetry, executionContext) {
  
    sendNotification(telemetry.SensorId, "CarbonDioxide", JSON.stringify("hello world")); 

    // Log SensorId and Message
    log(`Sensor ID: ${telemetry.SensorId}. `);
    log(`Sensor value: ${JSON.stringify(telemetry.Message)}.`);

    var sensor = getSensorMetadata(telemetry.SensorId);

    // Retrieve the sensor value
    var parseReading = JSON.parse(telemetry.Message);

    try {
        // Set the sensor reading as the current value for the sensor.
        setSensorValue(telemetry.SensorId, sensor.DataType, parseReading.SensorValue);

        // Get parent space
        var parentSpace = sensor.Space();

        // Get children sensors from the same space
        var otherSensors = parentSpace.ChildSensors();
        var carbonDioxideThreshold = 1000.0;

        // Retrieve C02, and motion sensors
        var CarbonDioxideSensor = otherSensors.find(function(element) {
            return element.DataType === CarbonDioxideType;
        });

        var motionSensor = otherSensors.find(function(element) {
            return element.DataType === motionType;
        });

        // get latest values for above sensors
        var motionVal = motionSensor.Value().Value;
        var presence = !!motionVal && motionVal.toLowerCase() === "true";
        var CarbonDioxide = getFloatValue(CarbonDioxideSensor.Value().Value);

        // Return if no motion or C02 found
        if(motionVal === null || CarbonDioxide === null) {
            return;
        }

        // If C02 greater than threshold and presence in the room => log, notify and set parent space computed value 
        if(CarbonDioxide < carbonDioxideThreshold && !presence) {
            log(`Room is available and air quality is good. Carbon Dioxide: ${CarbonDioxideSensor.Value().Value}. Presence: ${presence}.`);
            setSpaceValue(parentSpace.Id, spaceAirQuality, "Room is available and air quality is good.");

            // Set up custom notification for air quality
            parentSpace.Notify(JSON.stringify("Room not available or air quality is poor.")); 
        }
        else {
            log(`Room is not available or air quality is poor. Carbon Dioxide: ${CarbonDioxideSensor.Value().Value}. Presence: ${presence}.`);
            setSpaceValue(parentSpace.Id, spaceAirQuality, "Room is not available or air quality is poor.");

            // Set up custom notification for air quality
            parentSpace.Notify(JSON.stringify("Room not available or air quality is poor.")); 
        }
    }
    catch (error)
    {
        log(`An error has occured processing the UDF Error: ${error.name} Message ${error.message}.`);
    }
}


function getFloatValue(str) {
  if(!str) {
      return null;
  }

  return parseFloat(str);
}  