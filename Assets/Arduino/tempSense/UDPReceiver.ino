// #include <SPI.h>
// #include <WiFiNINA.h>
// #include <WiFiUdp.h>
// #include <U8x8lib.h>
// #include "secrets.h";

// // Network credentials from secrets.h
// char ssid[] = SECRET_SSID;
// char pass[] = SECRET_PASS;

// // UDP settings
// WiFiUDP udp;
// const unsigned int localUdpPort = 8888;

// // OLED display
// U8X8_SH1106_128X64_NONAME_HW_I2C u8x8(U8X8_PIN_NONE);

// void setup() {
//     Serial.begin(9600);
//     while (!Serial) {
//         ; // Wait for serial port to connect. Needed for native USB port only
//     }

//     // Initialize OLED display
//     u8x8.begin();
//     u8x8.setFont(u8x8_font_chroma48medium8_r);
//     u8x8.setFlipMode(0);
//     u8x8.drawString(0, 0, "Initializing...");

//     // Connect to WiFi network
//     Serial.print("Connecting to ");
//     Serial.println(ssid);
//     WiFi.begin(ssid, pass);
//     while (WiFi.status() != WL_CONNECTED) {
//         delay(500);
//         Serial.print(".");
//     }
//     Serial.println("WiFi connected");
//     Serial.println("IP Address: ");
//     Serial.println(WiFi.localIP());

//      // Display WiFi connection status
//     if (WiFi.status() == WL_CONNECTED) {
//         u8x8.clear();
//         u8x8.drawString(0, 0, "WiFi connected");
//         u8x8.drawString(0, 2, "IP Address:");
//         u8x8.drawString(0, 3, WiFi.localIP().toString().c_str());
//     } else {
//         u8x8.clear();
//         u8x8.drawString(0, 0, "WiFi disconnected");
//     }

//     // Begin UDP communication
//     udp.begin(localUdpPort);
//     Serial.print("Listening for UDP packets on port ");
//     Serial.println(localUdpPort);
// }

// void loop() {
//     // If data is available, read it and print the temperature
//     int packetSize = udp.parsePacket();
//     if (packetSize) {
//         byte data[sizeof(float)];
//         udp.read(data, sizeof(float));
//         float temperature;
//         memcpy(&temperature, data, sizeof(float));
//         u8x8.clear();
//         u8x8.drawString(0, 0, "Temperature:");
//         u8x8.setCursor(0, 2);
//         u8x8.print(temperature);
//         u8x8.print(" C");
//     }

//     delay(1000); // Delay to avoid flooding the display
// }
