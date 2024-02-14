using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ElectronBeam : MonoBehaviour {
    ParticleSystem vectorFieldParticleSystem;
    ParticleSystem.EmissionModule emmission;
    ParticleSystem.Particle[] particles;

    [Range(0,15)]
    public float velocityMin;
    [Range(0,15)]
    public float velocityMax;

    public ComputeShader computeShader;
    private ComputeBuffer velocityBuffer;
    private ComputeBuffer positionBuffer;
    private int kernelID;
    private const int threadCount = 1024;

    private Vector3[] velocityArray;
    private Vector3[] positionArray;

    public List<Vector4> externalCharge;
    private ComputeBuffer externalChargeBuffer;

    void Update() {
        //Read-in alive particles
        InitializeParticleSystem();
        int aliveParticleCount = vectorFieldParticleSystem.GetParticles(particles);

        if(aliveParticleCount == 0) {return;}

        //Allocate position and field buffers
        positionBuffer = new ComputeBuffer(aliveParticleCount, sizeof(float) * 3);
        positionArray = new Vector3[aliveParticleCount];

        velocityBuffer = new ComputeBuffer(aliveParticleCount, sizeof(float) * 3);
        velocityArray = new Vector3[aliveParticleCount];

        externalChargeBuffer = new ComputeBuffer(externalCharge.Count, sizeof(float) * 4);

        //Read-in position data to position buffer
        for(int i = 0; i < aliveParticleCount; i++) {
            positionArray[i] = particles[i].position;
            velocityArray[i] = particles[i].velocity;

            if(checkIfOutsideBoundry(positionArray[i])) {particles[i].remainingLifetime = 0;}
        }
        positionBuffer.SetData(positionArray);
        velocityBuffer.SetData(velocityArray);
        externalChargeBuffer.SetData(externalCharge);

        //Set buffers and dispatch compute shader
        computeShader.SetBuffer(kernelID, "_Position", positionBuffer);
        computeShader.SetBuffer(kernelID, "_Velocity", velocityBuffer);
        computeShader.SetInt("_ParticleCount", aliveParticleCount);
        computeShader.SetInt("_ExternalChargeCount", externalCharge.Count);

        computeShader.SetBuffer(kernelID, "_ExternalCharge", externalChargeBuffer);

        computeShader.Dispatch(kernelID, (int)Mathf.Ceil(aliveParticleCount/(float)threadCount), 1, 1);

        velocityBuffer.GetData(velocityArray);
  
        for(int i = 0; i < aliveParticleCount; i++) {
            if(float.IsNaN(velocityArray[i].magnitude)) {particles[i].remainingLifetime = 0; continue;}
            particles[i].velocity = velocityArray[i];
            particles[i].startSize = 0.05f;
            particles[i].startColor = Color.yellow;
        }

        vectorFieldParticleSystem.SetParticles(particles, aliveParticleCount); 

        positionBuffer.Release();
        velocityBuffer.Release();
        externalChargeBuffer.Release();
    }

    void InitializeParticleSystem() {
        if (vectorFieldParticleSystem == null) {
            vectorFieldParticleSystem = GetComponent<ParticleSystem>();
            emmission = vectorFieldParticleSystem.emission;
        }

        if (particles == null || particles.Length < vectorFieldParticleSystem.main.maxParticles) {
            particles = new ParticleSystem.Particle[vectorFieldParticleSystem.main.maxParticles];
        }
    }

    private void OnEnable() {
        kernelID = computeShader.FindKernel("CSMain");
    }

    private void OnDisable() {
        if(velocityBuffer != null) {
            velocityBuffer.Release();
            velocityBuffer = null;
        }

        if(positionBuffer != null) {
            positionBuffer.Release();
            positionBuffer = null;
        }

        if(externalChargeBuffer != null) {
            externalChargeBuffer.Release();
            externalChargeBuffer = null;
        }
    }

    bool checkIfOutsideBoundry(Vector3 radialVector) {
        if(radialVector.magnitude > 4) {return true;}
        return false;
    }

    float Remap(float iMin, float iMax, float oMin, float oMax, float v) {
                    float t = Mathf.InverseLerp(iMin, iMax, v);
                    return Mathf.Lerp(oMin, oMax, t);
    }

    Color Remap(float iMin, float iMax, Color oMin, Color oMax, float v) {
                    float t = Mathf.InverseLerp(iMin, iMax, v);
                    return Color.Lerp(oMin, oMax, t);
    }
}