#include "Accelerometer.h"

Accelerometer::Accelerometer(char id, float threshold, bfs::Mpu6500::I2cAddr pin) {
    this->id = id;
    this->threshold = threshold;
    this->pin = pin;

    Wire.begin();
    Wire.setClock(400000);
    mpu6500.Config(&Wire, pin);
    if (!mpu6500.Begin()) {
        Serial.println("Error initializing communication with IMU");
        while(1) {}
    }
    if (!mpu6500.ConfigSrd(19)) {
        Serial.println("Error configured SRD");
        while(1) {}
    }
    if (!mpu6500.Read()) {
        Serial.println("Error reading Accelerometer");
        while(1) {}
    }
    this->started = true;
    Serial.println("Accelerometer started");
}

void Accelerometer::updateState() {
    if (!this->started) {
        Serial.println("Accelerometer did not start");
        return;
    }
    if (!mpu6500.Read()) {
        Serial.println("Reading error for Accelerometer");
        return;
    }
    this->lastState = this->state;
    this->state = mpu6500.accel_z_mps2() >= this->threshold;
}

