#ifndef ABSTRACT_TRASH_SENSOR_H
#define ABSTRACT_TRASH_SENSOR_H

#include <Arduino.h>
#include <Wire.h>
#include "NetworkFacade.h"

class AbstractTrashSensor {
public:
  virtual void updateState() = 0;
  
  bool getState() {
    return this->state;
  }
  bool getLastState() {
    return this->lastState;
  }
  bool stateChanged() {
    return this->state != this->lastState;
  }
  String toString() {
    return String(this->id) + ":" + String(this->state) + (this->alertSent ? "!" : " ");
  };

protected:
  AbstractTrashSensor() {}
  char id = '/';
  bool started = false;
  bool state = false;
  bool lastState = false;
  bool alertSent = false;
};

#endif
