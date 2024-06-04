#ifndef NETWORK_FACADE_H
#define NETWORK_FACADE_H

#include <Arduino.h>

class NetworkFacade {
public:
    static void sendSensorState(char sensorId, bool sensorState) {
      Serial.println("Sending " + String(sensorState) + " for sensor " + String(sensorId));
      // add communication
    }
};

#endif
