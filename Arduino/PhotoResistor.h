#ifndef PHOTO_RESISTOR_H
#define PHOTO_RESISTOR_H

#include "AbstractTrashSensor.h"

class PhotoResistor : public AbstractTrashSensor {
public:
    PhotoResistor(char id, float threshold, uint8_t pin);
    void updateState() override;

private:
    float threshold;
    uint8_t pin;
};

#endif