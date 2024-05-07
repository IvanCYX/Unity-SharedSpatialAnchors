#include <Wire.h>
#include <SPI.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>
#include <WiFiNINA.h>
#include <WiFiUdp.h>
#include "secrets.h" // Include the secrets header file

#define BME_SCK 13
#define BME_MISO 12
#define BME_MOSI 11
#define BME_CS 10

#define SEALEVELPRESSURE_HPA (1013.25)

Adafruit_BME280 bme; // I2C
WiFiUDP udp;

const IPAddress udpServerIP(10, 72, 13, 237); // IP address of target device
const unsigned int udpPort = 8888; // UDP port number

unsigned long delayTime;

void setup() {
    Serial.begin(9600);
    Serial.println(F("BME280 test"));

    unsigned status;

    // Initialize Wi-Fi
    if (WiFi.status() == WL_NO_MODULE) {
        Serial.println("Communication with WiFi module failed!");
        while (true);
    }

    // Attempt to connect to WiFi network using secrets
    while (status != WL_CONNECTED) {
        Serial.print("Attempting to connect to SSID: ");
        Serial.println(SECRET_SSID);
        status = WiFi.begin(SECRET_SSID, SECRET_PASS);
        delay(5000); // Wait 5 seconds for connection
    }

    Serial.println("Connected to WiFi");

    // Initialize UDP communication
    udp.begin(udpPort);

    // default settings
    status = bme.begin();
    if (!status) {
        Serial.println("Could not find a valid BME280 sensor, check wiring, address, sensor ID!");
        Serial.print("SensorID was: 0x"); Serial.println(bme.sensorID(),16);
        Serial.print("        ID of 0xFF probably means a bad address, a BMP 180 or BMP 085\n");
        Serial.print("   ID of 0x56-0x58 represents a BMP 280,\n");
        Serial.print("        ID of 0x60 represents a BME 280.\n");
        Serial.print("        ID of 0x61 represents a BME 680.\n");
        while (1) delay(10);
    }

    Serial.println("-- Default Test --");
    delayTime = 5000;

    Serial.println();
}

void loop() {
    printValues();
    delay(delayTime);
}

void printValues() {
    Serial.print("Temperature = ");
    float temperature = bme.readTemperature();
    Serial.print(temperature);
    Serial.println(" °C");

    Serial.print("Pressure = ");
    Serial.print(bme.readPressure() / 100.0F);
    Serial.println(" hPa");

    Serial.print("Approx. Altitude = ");
    Serial.print(bme.readAltitude(SEALEVELPRESSURE_HPA));
    Serial.println(" m");

    Serial.print("Humidity = ");
    Serial.print(bme.readHumidity());
    Serial.println(" %");

    Serial.println();

    // Serialize temperature data into a byte array
    byte data[sizeof(float)];
    // memcpy (destination, ref source, size), referencing non-null temperature
    // reference does not allow for null
    memcpy(data, &temperature, sizeof(float));

    // Send UDP packet with temperature data
    udp.beginPacket(udpServerIP, udpPort);
    udp.write(data, sizeof(float));
    Serial.print("Temp data sent");
    udp.endPacket();
}
