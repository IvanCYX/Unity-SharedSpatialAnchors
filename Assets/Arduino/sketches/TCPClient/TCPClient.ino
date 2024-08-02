#include <SPI.h>
#include <WiFiNINA.h>
#include <U8x8lib.h>
#include <Wire.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_LSM9DS1.h>
#include "secrets.h"

// Network credentials from secrets.h
char ssid[] = SECRET_SSID;
char pass[] = SECRET_PASS;

// TCP server settings
WiFiServer server(8888);  // Create a server that listens on port 8888

// OLED display
U8X8_SH1106_128X64_NONAME_HW_I2C u8x8(U8X8_PIN_NONE);

// LSM9DS1 settings
#define LSM9DS1_SCK A5
#define LSM9DS1_MISO 12
#define LSM9DS1_MOSI A4
#define LSM9DS1_XGCS 6
#define LSM9DS1_MCS 5

Adafruit_LSM9DS1 lsm = Adafruit_LSM9DS1();

void setupSensor() {
  lsm.setupAccel(lsm.LSM9DS1_ACCELRANGE_2G, lsm.LSM9DS1_ACCELDATARATE_10HZ);
  lsm.setupGyro(lsm.LSM9DS1_GYROSCALE_245DPS);
}

void setup() {
  Serial.begin(9600);
  while (!Serial) {
    ;  // Wait for serial port to connect. Needed for native USB port only
  }

  // Initialize OLED display
  u8x8.begin();
  u8x8.setFont(u8x8_font_5x7_f);
  u8x8.setFlipMode(0);

  // Initialize LSM9DS1 sensor
  if (!lsm.begin()) {
    Serial.println("Oops ... unable to initialize the LSM9DS1. Check your wiring!");
    while (1);
  }
  setupSensor();

  // Connect to WiFi network
  while (WiFi.status() != WL_CONNECTED) {
    u8x8.drawString(0, 0, "Connecting to: ");
    u8x8.drawString(0, 1, ssid);
    WiFi.begin(ssid, pass);
    delay(500);
  }

  // Display WiFi connection status
  if (WiFi.status() == WL_CONNECTED) {
    u8x8.clear();
    u8x8.drawString(0, 0, "WiFi connected");
    u8x8.drawString(0, 2, "IP Address:");
    u8x8.drawString(0, 3, WiFi.localIP().toString().c_str());
    Serial.print("Arduino IP Address: ");
    Serial.println(WiFi.localIP());

    // Start the TCP server
    server.begin();
    Serial.println("Server started, waiting for clients...");
    u8x8.drawString(0, 4, "Server started");
  } else {
    u8x8.clear();
    u8x8.drawString(0, 0, "WiFi disconnected");
    Serial.println("WiFi disconnected");
    while (true)
      ;  // Halt execution if WiFi connection fails
  }
}

void loop() {
  // Check if a client has connected
  WiFiClient client = server.available();

  if (client) {
    Serial.println("Client connected");
    u8x8.drawString(0, 5, "Client connected");

    while (client.connected()) {
      lsm.read(); // Read the data from the sensor

      // Get a new sensor event
      sensors_event_t a, g;
      lsm.getEvent(&a, nullptr, &g, nullptr);

      // Extract accelerometer data
      float accelX = a.acceleration.x;
      float accelY = a.acceleration.y;
      float accelZ = a.acceleration.z;

      // Extract gyroscope data
      float gyroX = g.gyro.x;
      float gyroY = g.gyro.y;
      float gyroZ = g.gyro.z;

      // Format data as a string
      String data = String("AccelX=") + accelX + "&AccelY=" + accelY + "&AccelZ=" + accelZ +
                    "&GyroX=" + gyroX + "&GyroY=" + gyroY + "&GyroZ=" + gyroZ;

      // Send data to the connected client
      client.println(data);

      // Display data on OLED
      displaySensorData(accelX, accelY, accelZ, gyroX, gyroY, gyroZ);

      Serial.println("Data sent: " + data);

      delay(500);  // Delay to avoid flooding the client
    }

    client.stop();
    Serial.println("Client disconnected");
    u8x8.drawString(0, 5, "Client disconnected");
  }
}

void displaySensorData(float accelX, float accelY, float accelZ, float gyroX, float gyroY, float gyroZ) {
  // Display accelerometer data
  u8x8.clearLine(0);
  u8x8.drawString(0, 0, "Accel X: ");
  u8x8.setCursor(9, 0);
  u8x8.print(accelX);

  u8x8.clearLine(1);
  u8x8.drawString(0, 1, "Accel Y: ");
  u8x8.setCursor(9, 1);
  u8x8.print(accelY);

  u8x8.clearLine(2);
  u8x8.drawString(0, 2, "Accel Z: ");
  u8x8.setCursor(9, 2);
  u8x8.print(accelZ);

  // Display gyroscope data
  u8x8.clearLine(3);
  u8x8.drawString(0, 3, "Gyro X: ");
  u8x8.setCursor(8, 3);
  u8x8.print(gyroX);

  u8x8.clearLine(4);
  u8x8.drawString(0, 4, "Gyro Y: ");
  u8x8.setCursor(8, 4);
  u8x8.print(gyroY);

  u8x8.clearLine(5);
  u8x8.drawString(0, 5, "Gyro Z: ");
  u8x8.setCursor(8, 5);
  u8x8.print(gyroZ);
}
