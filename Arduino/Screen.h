#ifndef SCREEN_H
#define SCREEN_H

#include <LiquidCrystal.h>

class Screen {
public:
    Screen(int RS, int E, int D4, int D5, int D6, int D7);
    void write(String line0, String line1);

private:
    LiquidCrystal lcd;
};

#endif
