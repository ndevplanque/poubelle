#ifndef ACCELEROMETER_H
#define ACCELEROMETER_H

#include <mpu6500.h>
#include "AbstractTrashSensor.h"

class Accelerometer : public AbstractTrashSensor {
public:
    Accelerometer(char id, float threshold, bfs::Mpu6500::I2cAddr pin = bfs::Mpu6500::I2C_ADDR_PRIM);
    void updateState() override;

private:
    float threshold;
    bfs::Mpu6500 mpu6500;
    bfs::Mpu6500::I2cAddr pin;
};

#endif