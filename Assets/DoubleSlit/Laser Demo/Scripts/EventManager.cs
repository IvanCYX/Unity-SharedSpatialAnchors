//Event Manager Class -- Manages event triggers from the UI menu

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour {
    //Define all action events 
    public event Action clearScreen;
    public event Action switchLaserPlacementMode;
    public event Action updateLasers;
    public event Action startSetup1;
    public event Action startSetup2;
    public event Action startSetup3;
    public event Action startSetup4;
    public event Action startSetup5;
    public event Action startSetup6;
    public event Action startSetup7;
    public event Action startSetup8;
    public event Action decreaseLaserRadius;
    public event Action increaseLaserRadius;

    //Define action triggering functions that can called through the UI interface
    void ClearScreen() {clearScreen();}
    void SwitchLaserPlacementMode() {switchLaserPlacementMode();}

    void StartSetup1() {clearScreen(); startSetup1(); updateLasers();}
    void StartSetup2() {clearScreen(); startSetup2(); updateLasers();}
    void StartSetup3() {clearScreen(); startSetup3(); updateLasers();}
    void StartSetup4() {clearScreen(); startSetup4(); updateLasers();}
    void StartSetup5() {clearScreen(); startSetup5(); updateLasers();}
    void StartSetup6() {clearScreen(); startSetup6(); updateLasers();}
    void StartSetup7() {clearScreen(); startSetup7(); updateLasers();}
    void StartSetup8() {clearScreen(); startSetup8(); updateLasers();}

    void DecreaseLaserRadius() {decreaseLaserRadius();}
    void IncreaseLaserRadius() {increaseLaserRadius();}
    
    //Testing script to allow event triggers through keyboard input
    void Update() {
        if(Input.GetKeyDown(KeyCode.C)) {
            ClearScreen();
        } else if(Input.GetKeyDown(KeyCode.Alpha1)) {
            StartSetup1();
        } else if(Input.GetKeyDown(KeyCode.Alpha2)) {
            StartSetup2();
        } else if(Input.GetKeyDown(KeyCode.Alpha3)) {
            StartSetup3();
        } else if(Input.GetKeyDown(KeyCode.Alpha4)) {
            StartSetup4();
        } else if(Input.GetKeyDown(KeyCode.Alpha5)) {
            StartSetup5();
        } else if(Input.GetKeyDown(KeyCode.Alpha6)) {
            StartSetup6();
        } else if(Input.GetKeyDown(KeyCode.Alpha7)) {
            StartSetup7();
        } else if(Input.GetKeyDown(KeyCode.Alpha8)) {
            StartSetup8();
        } else if(Input.GetKeyDown(KeyCode.O)) {
            DecreaseLaserRadius();
        } else if(Input.GetKeyDown(KeyCode.P)) {
            IncreaseLaserRadius();
        }
    }
    
}
