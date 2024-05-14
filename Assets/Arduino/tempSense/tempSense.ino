#include <Wire.h>
#include <SPI.h>
#include <Adafruit_Sensor.h>
#include <Adafruit_BME280.h>
#include <WiFiNINA.h>
#include <WiFiUdp.h>
#include "secrets.h" // Include the secrets header file
#include <U8x8lib.h>//https://github.com/olikraus/u8glib

#define BME_SCK 13
#define BME_MISO 12
#define BME_MOSI 11
#define BME_CS 10

#define SEALEVELPRESSURE_HPA (1013.25)

Adafruit_BME280 bme; // I2C
WiFiUDP udp;

//Objects
U8X8_SH1106_128X64_NONAME_HW_I2C u8x8(U8X8_PIN_NONE);

const IPAddress udpServerIP(10, 65, 20, 51); // IP address of target device
const unsigned int udpPort = 8888; // UDP port number

unsigned long delayTime;

void setup() {
    Serial.begin(9600);
    Serial.println(F("BME280 test"));

    //Init OLED screen
 	  u8x8.begin();
 	  u8x8.setFont(u8x8_font_chroma48medium8_r);
 	  u8x8.setFlipMode(0);

    Serial.println("OLED begun");

    unsigned status;

    // Initialize Wi-Fi
    if (WiFi.status() == WL_NO_MODULE) {
        u8x8.drawString(0, 0, "WiFi Failed");
        while (true);
    }

    // Attempt to connect to WiFi network using secrets
    while (status != WL_CONNECTED) {
        u8x8.drawString(0, 0, "Connecting to: ");
        u8x8.drawString(0, 1, SECRET_SSID);
        status = WiFi.begin(SECRET_SSID, SECRET_PASS);
        delay(5000); // Wait 5 seconds for connection
    }

    u8x8.clearLine(0);
    u8x8.drawString(0, 0, "Connected to");

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
    delayTime = 2000;

    Serial.println();
}

void loop() {
    printValues();
    delay(delayTime);
}

void printValues() {
    // Read temperature and humidity
    float temperature = bme.readTemperature();
    float humidity = bme.readHumidity();

    // Round temperature and humidity to one decimal place
    temperature = round(temperature * 100.0) / 100.0;
    humidity = round(humidity * 100.0) / 100.0;
    
    // Display temperature
    u8x8.drawString(0, 4, "Temp= ");
    u8x8.setCursor(8, 4); // Set cursor position after "=" in "Temp ="
    u8x8.print(temperature, 1);
    u8x8.drawString(12, 4, " °C");

    // Display humidity
    u8x8.drawString(0, 5, "Humidity= ");
    u8x8.setCursor(10, 5); // Set cursor position after "=" in "Humidity ="
    u8x8.print(humidity, 1);
    u8x8.drawString(14, 5, " %");

    // Debug printing
    Serial.print("Temperature = ");
    Serial.print(temperature);
    Serial.println(" °C");
    Serial.print("Humidity = ");
    Serial.print(humidity);
    Serial.println(" %");
    
    //Serialize temperature data into a byte array
    byte data[sizeof(float)];
    memcpy(data, &temperature, sizeof(float));

    //Send UDP packet with temperature data
    udp.beginPacket(udpServerIP, udpPort);
    udp.write(data, sizeof(float));
    Serial.println("Temp data sent");
    udp.endPacket();
    u8x8.drawString(0, 7, "Sending data... ");
}

