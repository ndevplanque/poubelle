#include "PhotoResistor.h"

PhotoResistor::PhotoResistor(char id, float threshold, uint8_t pin) {
    this->id = id;
    this->pin = pin;
    this->threshold = threshold;
    pinMode(this->pin, INPUT);
    this->started = true;
    Serial.println("PhotoResistor " + String(this->pin) + " started");
}

void PhotoResistor::updateState(bool debug) {
    if (!this->started) {
        Serial.println("PhotoResistor did not start");
        return;
    }

    float value = analogRead(this->pin);

    this->lastState = this->state;
    this->state = value < this->threshold;

    if (debug) {
      Serial.println(String(this->id) + ": " + String(value) + "(" + String(this->state) + ")");
    }
}