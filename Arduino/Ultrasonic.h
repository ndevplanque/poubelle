#ifndef ULTRASONIC_H
#define ULTRASONIC_H

#include "AbstractTrashSensor.h"

class Ultrasonic : public AbstractTrashSensor {
public:
    Ultrasonic(char id, float distanceThreshold, uint8_t trigPin, uint8_t echoPin);
    void updateState() override;

private:
    float distanceThreshold;
    uint8_t trigPin;
    uint8_t echoPin;
    float distance;
    float duration;
};

#endif