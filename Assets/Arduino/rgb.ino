// #include <WiFiNINA.h>
// #include <utility/wifi_drv.h>

// void setup() {
//   WiFiDrv::pinMode(25, OUTPUT); //define green pin
//   WiFiDrv::pinMode(26, OUTPUT); //define red pin
//   WiFiDrv::pinMode(27, OUTPUT); //define blue pin
// }

// void loop() {
//   WiFiDrv::analogWrite(25, 255);
//   WiFiDrv::analogWrite(26, 0);
//   WiFiDrv::analogWrite(27, 0);

//   delay(1000);

//   WiFiDrv::analogWrite(25, 0);
//   WiFiDrv::analogWrite(26, 255);
//   WiFiDrv::analogWrite(27, 0);

//   delay(1000);

//   WiFiDrv::analogWrite(25, 0);
//   WiFiDrv::analogWrite(26, 0);
//   WiFiDrv::analogWrite(27, 255);

//   delay(1000);

//   WiFiDrv::analogWrite(25, 0);
//   WiFiDrv::analogWrite(26, 0);
//   WiFiDrv::analogWrite(27, 0);

//   delay(1000);
// }