#include "Screen.h"

Screen::Screen(int RS, int E, int D4, int D5, int D6, int D7)
    : lcd(RS, E, D4, D5, D6, D7) {
    lcd.begin(16, 2);
    lcd.print("Loading...");
    lcd.display();
}

void Screen::write(String line0, String line1) {
    lcd.clear();
    lcd.setCursor(0, 0);
    lcd.print(line0);
    lcd.setCursor(0, 1);
    lcd.print(line1);
}