using UnityEngine;

[ExecuteInEditMode]
public class SolarWindController : MonoBehaviour {
    ParticleSystem vectorFieldParticleSystem;
    ParticleSystem.EmissionModule emmission;
    ParticleSystem.Particle[] particles;

    [Range(0,100)]
    public float velocityMax;

    public float charge;
    public float earthMagneticFieldMagnitude;
    public float earthRadius;
    public Vector4 earthDipole;

    public ComputeShader computeShader;
    private ComputeBuffer velocityBuffer;
    private ComputeBuffer positionBuffer;
    private int kernelID;

    private Vector3[] velocityArray;
    private Vector3[] positionArray;

    private const int threadCount = 1024;

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

        //Read-in position data to position buffer
        for(int i = 0; i < aliveParticleCount; i++) {
            positionArray[i] = particles[i].position;
            velocityArray[i] = particles[i].velocity;
        }
        positionBuffer.SetData(positionArray);
        velocityBuffer.SetData(velocityArray);

        //Set buffers and dispatch compute shader
        computeShader.SetBuffer(kernelID, "_Position", positionBuffer);
        computeShader.SetBuffer(kernelID, "_Velocity", velocityBuffer);
        computeShader.SetInt("_ParticleCount", aliveParticleCount);

        computeShader.SetFloat("_charge", charge);
        computeShader.SetFloat("_earthMagneticFieldMagnitude", earthMagneticFieldMagnitude);
        computeShader.SetFloat("_earthRadius", earthRadius);
        computeShader.SetVector("_earthDipole", earthDipole);

        computeShader.Dispatch(kernelID, (int)Mathf.Ceil(aliveParticleCount/(float)threadCount), 1, 1);

        velocityBuffer.GetData(velocityArray);
  
        for(int i = 0; i < aliveParticleCount; i++) {
            particles[i].velocity = velocityArray[i];
            particles[i].startColor = Remap(0, velocityMax, Color.blue, Color.red, velocityArray[i].magnitude);

            if(particles[i].velocity.magnitude > 100) {particles[i].remainingLifetime = 0;}
            //particles[i].startSize = Remap(0,velocityMax, 1f, 3f, velocityArray[i].magnitude);
        }

        vectorFieldParticleSystem.SetParticles(particles, aliveParticleCount); 

        positionBuffer.Release();
        velocityBuffer.Release();
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
    }

    bool checkIfOutsideBoundry(Vector3 radialVector) {
        if(radialVector.magnitude > 6) {return true;}
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