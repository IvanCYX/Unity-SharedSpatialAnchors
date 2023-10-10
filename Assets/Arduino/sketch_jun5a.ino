#include <WiFiNINA.h>
#include <utility/wifi_drv.h>
#include <Servo.h>
#include "secrets.h";

char ssid[] = SECRET_SSID;
char pass[] = SECRET_PASS;
WiFiServer server(1234);
Servo myServo;

void setup() {
  myServo.attach(13);
  // WiFiDrv::pinMode(25, OUTPUT); //define green pin
  // WiFiDrv::pinMode(26, OUTPUT); //define red pin
  // WiFiDrv::pinMode(27, OUTPUT); //define blue pin
   WiFi.begin(ssid, pass);
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Connecting to WiFi...");
  }
  Serial.println("Connected to WiFi:");
  Serial.println(ssid);

  server.begin();
}

void loop() {
  WiFiClient client = server.available();
  if (client) {
    while (client.connected()) {
      if (client.available()) {
        String message = client.readStringUntil('\n');
        Serial.println("Received: " + message);
        Serial.println("I'll blink now!");
        blinkLED(); // Blink the onboard RGB LED
        }
      }
    }
    client.stop();
  }

void blinkLED() {
  for(int i = 0; i < 180; i++) {
    myServo.write(i);
    delay(15);
  }
  for(int i = 179; i > 0; i--) {
    myServo.write(i);
    delay(15);
  }
  // WiFiDrv::analogWrite(25, 255);
  // WiFiDrv::analogWrite(26, 0);
  // WiFiDrv::analogWrite(27, 0);

  // delay(1000);

  // WiFiDrv::analogWrite(25, 0);
  // WiFiDrv::analogWrite(26, 255);
  // WiFiDrv::analogWrite(27, 0);

  // delay(1000);

  // WiFiDrv::analogWrite(25, 0);
  // WiFiDrv::analogWrite(26, 0);
  // WiFiDrv::analogWrite(27, 255);

  // delay(1000);

  // WiFiDrv::analogWrite(25, 0);
  // WiFiDrv::analogWrite(26, 0);
  // WiFiDrv::analogWrite(27, 0);

  // delay(1000);
}