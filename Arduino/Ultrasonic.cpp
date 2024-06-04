#include "Ultrasonic.h"

Ultrasonic::Ultrasonic(char id, float distanceThreshold, uint8_t trigPin, uint8_t echoPin) {
    this->id = id;
    this->trigPin = trigPin;
    this->echoPin = echoPin;
    this->distanceThreshold = distanceThreshold;

    pinMode(trigPin, OUTPUT);
	  pinMode(echoPin, INPUT);

    this->started = true;
    Serial.println("Ultrasonic " + String(this->trigPin) + "/" + String(this->echoPin) + " started");
}

void Ultrasonic::updateState() {
    if (!this->started) {
        Serial.println("Ultrasonic did not start");
        return;
    }

    digitalWrite(this->trigPin, LOW);
    delayMicroseconds(2);
    digitalWrite(this->trigPin, HIGH);
    delayMicroseconds(10);
    digitalWrite(this->trigPin, LOW);

    this->duration = pulseIn(this->echoPin, HIGH);
    this->distance = (duration*.0343)/2;

    this->lastState = this->state;
    this->state = distance > this->distanceThreshold;
}