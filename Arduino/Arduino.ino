#include "Screen.h"
#include "Ultrasonic.h"
#include "Accelerometer.h"
#include "PhotoResistor.h"

Screen *screen;
int RS = 12;
int E = 11;
int D4 = 5;
int D5 = 4;
int D6 = 3;
int D7 = 2;

Ultrasonic *sonic;
const int ULTRASONIC_TRIG_PIN = 8;
const int ULTRASONIC_ECHO_PIN = 9;
const float ULTRASONIC_DISTANCE_THRESHOLD = 10;

Accelerometer *accel;
const float ACCELEROMETER_THRESHOLD = -10;

PhotoResistor *light;
const int PHOTO_RESISTOR_PIN = A4;
const int PHOTO_RESISTOR_THRESHOLD = 900;

NetworkFacade *network;

void setup() {
  Serial.begin(115200);
  while(!Serial) {}
  screen = new Screen(RS, E, D4, D5, D6, D7);
  sonic = new Ultrasonic('S', ULTRASONIC_DISTANCE_THRESHOLD, ULTRASONIC_TRIG_PIN, ULTRASONIC_ECHO_PIN);
  accel = new Accelerometer('A', ACCELEROMETER_THRESHOLD);
  light = new PhotoResistor('L', PHOTO_RESISTOR_THRESHOLD, PHOTO_RESISTOR_PIN);
}

void loop() {
  static unsigned long lastTime = 0;
  unsigned long currentTime = millis();

  if (currentTime - lastTime >= 500) {
    processSensor(sonic);
    processSensor(accel);
    processSensor(light);

    screen->write(
      sonic->toString() + "  " + accel->toString() + "  " + light->toString(),
      "Todo"
    );

    lastTime = currentTime;
  }
}

void processSensor(AbstractTrashSensor* sensor) {
  sensor->updateState();

  if(sensor->stateChanged()) {
    NetworkFacade::sendSensorState(this->id, this->state);
  }
}