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

//     // Connect to WiFi network
//     WiFi.begin(ssid, pass);
//     while (WiFi.status() != WL_CONNECTED) {
//         u8x8.drawString(0, 0, "Connecting to: ");
//         u8x8.drawString(0, 1, SECRET_SSID);
//         delay(500);
//         Serial.print(".");
//     }
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
//     int connected = udp.begin(localUdpPort);
//     if (!connected) {
//       Serial.print("No udp port");
//     } else {
//       Serial.print("Listening for UDP packets on port ");
//     }
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
//         u8x8.drawString(0, 5, "Temp= ");
//         u8x8.setCursor(8, 5); // Set cursor position after "=" in "Temp ="
//         u8x8.print(temperature, 1);
//         u8x8.drawString(12, 4, " °C");
//         Serial.print(temperature);
//     } else {
//       u8x8.drawString(0, 5, "No packet");
//     }
// }
