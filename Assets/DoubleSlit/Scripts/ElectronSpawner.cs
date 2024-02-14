using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using UnityEngine.Video;

public class ElectronSpawner : MonoBehaviour {
    [Range(0.1f, 2)] public float electronSpeed = 2f;
    [Range(0.1f,20)] public float electronFrequency = 1;
    [Range(0,10)] public float maxElectronDeflection = 1;
    [Range(0,10)] public int shootingMode = 0;

    private int lastShootingMode = -1;
    private float lastElectronTime = 0;

    public float coordIncrement = 0.001f;
    public float rotIncrement = 0.01f;

    public uint electronCount;

    void StartMode0() {shootingMode = 0;} void StartMode1() {shootingMode = 1;} void StartMode2() {shootingMode = 2;}
    void StartMode3() {shootingMode = 3;} void StartMode4() {shootingMode = 4;} void StartMode5() {shootingMode = 5;}
    void StartMode6() {shootingMode = 6;} void StartMode7() {shootingMode = 7;}

    void DecreaseX() {DemoManager.vacuumTubeSetup.transform.position = new Vector3(DemoManager.vacuumTubeSetup.transform.position.x - coordIncrement, DemoManager.vacuumTubeSetup.transform.position.y, DemoManager.vacuumTubeSetup.transform.position.z);}
    void DecreaseY() {DemoManager.vacuumTubeSetup.transform.position = new Vector3(DemoManager.vacuumTubeSetup.transform.position.x, DemoManager.vacuumTubeSetup.transform.position.y  - coordIncrement, DemoManager.vacuumTubeSetup.transform.position.z);}
    void DecreaseZ() {DemoManager.vacuumTubeSetup.transform.position = new Vector3(DemoManager.vacuumTubeSetup.transform.position.x, DemoManager.vacuumTubeSetup.transform.position.y, DemoManager.vacuumTubeSetup.transform.position.z - coordIncrement);}
    
    //void DecreaseXRot() {DemoManager.vacuumTubeSetup.transform.rotation = DemoManager.vacuumTubeSetup.transform.rotation * Quaternion.AngleAxis(-rotIncrement, DemoManager.vacuumTubeSetup.transform.right);}
    //void DecreaseYRot() {DemoManager.vacuumTubeSetup.transform.rotation = DemoManager.vacuumTubeSetup.transform.rotation * Quaternion.AngleAxis(-rotIncrement, DemoManager.vacuumTubeSetup.transform.up);}
    //void DecreaseZRot() {DemoManager.vacuumTubeSetup.transform.rotation = DemoManager.vacuumTubeSetup.transform.rotation * Quaternion.AngleAxis(-rotIncrement, DemoManager.vacuumTubeSetup.transform.forward);}

    void DecreaseXRot() {DemoManager.vacuumTubeSetup.transform.Rotate(-rotIncrement,0,0);} 
    void DecreaseYRot() {DemoManager.vacuumTubeSetup.transform.Rotate(0,-rotIncrement,0);}
    void DecreaseZRot() {DemoManager.vacuumTubeSetup.transform.Rotate(0,0,-rotIncrement);}

    void IncreaseX() {DemoManager.vacuumTubeSetup.transform.position = new Vector3(DemoManager.vacuumTubeSetup.transform.position.x + coordIncrement, DemoManager.vacuumTubeSetup.transform.position.y, DemoManager.vacuumTubeSetup.transform.position.z);}
    void IncreaseY() {DemoManager.vacuumTubeSetup.transform.position = new Vector3(DemoManager.vacuumTubeSetup.transform.position.x, DemoManager.vacuumTubeSetup.transform.position.y  + coordIncrement, DemoManager.vacuumTubeSetup.transform.position.z);}
    void IncreaseZ() {DemoManager.vacuumTubeSetup.transform.position = new Vector3(DemoManager.vacuumTubeSetup.transform.position.x, DemoManager.vacuumTubeSetup.transform.position.y, DemoManager.vacuumTubeSetup.transform.position.z + coordIncrement);}
    
    //void IncreaseXRot() {DemoManager.vacuumTubeSetup.transform.rotation = DemoManager.vacuumTubeSetup.transform.rotation * Quaternion.AngleAxis(rotIncrement, DemoManager.vacuumTubeSetup.transform.right);}
    //void IncreaseYRot() {DemoManager.vacuumTubeSetup.transform.rotation = DemoManager.vacuumTubeSetup.transform.rotation * Quaternion.AngleAxis(rotIncrement, DemoManager.vacuumTubeSetup.transform.up);}
    //void IncreaseZRot() {DemoManager.vacuumTubeSetup.transform.rotation = DemoManager.vacuumTubeSetup.transform.rotation * Quaternion.AngleAxis(rotIncrement, DemoManager.vacuumTubeSetup.transform.forward);}

    void IncreaseXRot() {DemoManager.vacuumTubeSetup.transform.Rotate(rotIncrement,0,0);} 
    void IncreaseYRot() {DemoManager.vacuumTubeSetup.transform.Rotate(0,rotIncrement,0);}
    void IncreaseZRot() {DemoManager.vacuumTubeSetup.transform.Rotate(0,0,rotIncrement);}

    void Update() {
        CheckForInputs();

        if(shootingMode != lastShootingMode) {
            lastShootingMode = shootingMode;
            lastElectronTime = 0;
            DestroyBlips();
            DestroyElectrons();

            if(shootingMode != 0) {
                DemoManager.doubleSlit.SetActive(true);
                DemoManager.doubleSlitImage.SetActive(true);
            }

            if(shootingMode != 1) {

            }
            
            if(shootingMode != 2) {

            } 
            
            if(shootingMode != 3) {
                DemoManager.leftCover.SetActive(false);
            } 

            if(shootingMode != 4) {
                DemoManager.rightCover.SetActive(false);
            }

            if(shootingMode != 5) {
                DemoManager.interferenceWave.SetActive(false);
                DemoManager.screen.GetComponent<VideoPlayer>().Stop();
            }

            if(shootingMode != 6 && shootingMode != 7) {
                DemoManager.singleSlit.SetActive(false);
                DemoManager.screen.SetActive(true);
            }

            if(shootingMode != 7) {
                DemoManager.singleSlit.GetComponent<SingleSlit>().displacementToggle = false;
            }

            if(shootingMode == 0) {
                electronFrequency = 3;

                DemoManager.doubleSlit.SetActive(false);
                DemoManager.doubleSlitImage.SetActive(false);
                DemoManager.screen.GetComponent<Renderer>().sharedMaterial = DemoManager.screenWhite;
            } else if(shootingMode == 1) {
                electronFrequency = 14;

                DemoManager.screen.GetComponent<Renderer>().sharedMaterial = DemoManager.screenWhite;
            } else if(shootingMode == 2) {
                DemoManager.screen.GetComponent<Renderer>().sharedMaterial = DemoManager.interferenceBothOpen;
            } else if(shootingMode == 3) {
                DemoManager.leftCover.SetActive(true);
                DemoManager.screen.GetComponent<Renderer>().sharedMaterial = DemoManager.interferenceLeftClosed;
            } else if(shootingMode == 4) {
                DemoManager.rightCover.SetActive(true);
                DemoManager.screen.GetComponent<Renderer>().sharedMaterial = DemoManager.interferenceRightClosed;
            } else if(shootingMode == 5) {
                DemoManager.interferenceWave.SetActive(true);
                DemoManager.screen.GetComponent<VideoPlayer>().Play();
                DemoManager.screen.GetComponent<Renderer>().sharedMaterial = DemoManager.particleInterferencePattern;
            } else if(shootingMode == 6) {
                electronFrequency = 0.4f;

                DemoManager.singleSlit.SetActive(true);
                DemoManager.screen.SetActive(false);

                //DemoManager.singleSlit.GetComponent<SingleSlit>().SwitchRestPosition();
            } else if(shootingMode == 7) {
                electronFrequency = 0.3f;

                DemoManager.singleSlit.SetActive(true);
                DemoManager.screen.SetActive(false);

                DemoManager.singleSlit.GetComponent<SingleSlit>().displacementToggle = true;
            }
        }

        if(shootingMode == 0) {
            if(Time.time > lastElectronTime + 1 / electronFrequency) {
                lastElectronTime = Time.time;
                SpawnElectronBeam();
            }
        } else if(shootingMode == 1) {
            if(Time.time > lastElectronTime + 1 / electronFrequency) {
                lastElectronTime = Time.time;
                SpawnScatteredElectronBeam();
            }
        } else if(shootingMode == 6) {
            if(Time.time > lastElectronTime + 1 / electronFrequency) {
                lastElectronTime = Time.time;
                SpawnElectronBeam();
                ++electronCount;
            }
        } else if(shootingMode == 7) {
            if(Time.time > lastElectronTime + 1 / electronFrequency) {
                lastElectronTime = Time.time;
                Invoke("MoveSingleSlit", 0f);
                //DemoManager.singleSlit.GetComponent<SingleSlit>().SwitchRestPosition();
                SpawnElectronBeam();
            }
        }
    }

    void MoveSingleSlit() {
        DemoManager.singleSlit.GetComponent<SingleSlit>().SwitchRestPosition();
    }

    void SpawnElectronBeam() {
        GameObject electronGameObject = Instantiate(DemoManager.electronPrefab, gameObject.transform.position, Quaternion.identity);
        Electron electron = electronGameObject.GetComponent<Electron>();
        electron.direction = gameObject.transform.forward;
        electron.speed = electronSpeed;
        if(!DemoManager.singleSlit.GetComponent<SingleSlit>().displacementToggle && shootingMode == 7) {electron.transform.localPosition += DemoManager.singleSlitOffset;}

        electron.transform.parent = gameObject.transform;
    }

    void SpawnScatteredElectronBeam() {
        GameObject electronGameObject = Instantiate(DemoManager.electronPrefab, gameObject.transform.position, Quaternion.identity);
        Electron electron = electronGameObject.GetComponent<Electron>();

        electron.sourceToScreenDirection = gameObject.transform.forward;
        electron.deflectionDirection = gameObject.transform.right;

        float deflectionAngle1 = Random.Range(-maxElectronDeflection, maxElectronDeflection);
        float deflectionAngle2 = Random.Range(-maxElectronDeflection, maxElectronDeflection);
        electron.direction = Quaternion.AngleAxis(deflectionAngle1, gameObject.transform.right) * 
                             Quaternion.AngleAxis(deflectionAngle2, gameObject.transform.up) * 
                             gameObject.transform.forward;
        electron.speed = electronSpeed;

        electron.transform.parent = gameObject.transform;
    }

    void DestroyElectrons() {
        foreach(Transform electron in gameObject.transform) {
            Destroy(electron.gameObject);
        }
    }

    void DestroyBlips() {
        foreach(Transform blip in DemoManager.blips.transform) {
            Destroy(blip.gameObject);
        }
    }

    void CheckForInputs() {
        if(Input.GetKeyDown(KeyCode.Alpha1)) {
            StartMode0();
        } else if(Input.GetKeyDown(KeyCode.Alpha2)) {
            StartMode1();
        } else if(Input.GetKeyDown(KeyCode.Alpha3)) {
            StartMode2();
        } else if(Input.GetKeyDown(KeyCode.Alpha4)) {
            StartMode3();
        } else if(Input.GetKeyDown(KeyCode.Alpha5)) {
            StartMode4();
        } else if(Input.GetKeyDown(KeyCode.Alpha6)) {
            StartMode5();
        } else if(Input.GetKeyDown(KeyCode.Alpha7)) {
            StartMode6();
        } else if(Input.GetKeyDown(KeyCode.Alpha8)) {
            StartMode7();
        } 

        if(Input.GetKeyDown(KeyCode.Q)) {
            IncreaseX();
        } else if(Input.GetKeyDown(KeyCode.A)) {
            DecreaseX();
        } else if(Input.GetKeyDown(KeyCode.W)) {
            IncreaseY();
        } else if(Input.GetKeyDown(KeyCode.S)) {
            DecreaseY();
        } else if(Input.GetKeyDown(KeyCode.E)) {
            IncreaseZ();
        } else if(Input.GetKeyDown(KeyCode.D)) {
            DecreaseZ();
        } else if(Input.GetKeyDown(KeyCode.R)) {
            IncreaseXRot();
        } else if(Input.GetKeyDown(KeyCode.F)) {
            DecreaseXRot();
        } else if(Input.GetKeyDown(KeyCode.T)) {
            IncreaseYRot();
        } else if(Input.GetKeyDown(KeyCode.G)) {
            DecreaseYRot();
        } else if(Input.GetKeyDown(KeyCode.Y)) {
            IncreaseZRot();
        } else if(Input.GetKeyDown(KeyCode.H)) {
            DecreaseZRot();
        } 

    }
}