using UnityEngine;

[ExecuteInEditMode]
public class SolenoidField : MonoBehaviour {
    ParticleSystem vectorFieldParticleSystem;
    ParticleSystem.EmissionModule emmission;
    ParticleSystem.Particle[] particles;

    [Range(-10,10)]
    public float current = 1;
    [Range(0,15)]
    public float velocityMax;

    public ComputeShader computeShader;
    private ComputeBuffer fieldBuffer;
    private ComputeBuffer positionBuffer;
    private int kernelID;

    private Vector3[] fieldArray;
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

        fieldBuffer = new ComputeBuffer(aliveParticleCount, sizeof(float) * 3);
        fieldArray = new Vector3[aliveParticleCount];

        //Read-in position data to position buffer
        for(int i = 0; i < aliveParticleCount; i++) {
            positionArray[i] = particles[i].position;
        }
        positionBuffer.SetData(positionArray);

        //Set buffers and dispatch compute shader
        computeShader.SetBuffer(kernelID, "_Position", positionBuffer);
        computeShader.SetBuffer(kernelID, "_Field", fieldBuffer);
        computeShader.SetInt("_ParticleCount", aliveParticleCount);
        computeShader.SetFloat("_Current", current);

        computeShader.Dispatch(kernelID, (int)Mathf.Ceil(aliveParticleCount/(float)threadCount), 1, 1);

        fieldBuffer.GetData(fieldArray);
  
        for(int i = 0; i < aliveParticleCount; i++) {
            particles[i].velocity = fieldArray[i];
            particles[i].startColor = Remap(0, velocityMax, Color.blue, Color.red, fieldArray[i].magnitude);
            particles[i].startSize = Remap(0,velocityMax, 0.1f, 0.3f, fieldArray[i].magnitude);
        }

        vectorFieldParticleSystem.SetParticles(particles, aliveParticleCount); 

        positionBuffer.Release();
        fieldBuffer.Release();
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
        if(fieldBuffer != null) {
            fieldBuffer.Release();
            fieldBuffer = null;
        }
    }

    Vector3 getMagneticFieldFromLineWire(Vector3 radialVector) {
        radialVector = new Vector3(0, radialVector.y, radialVector.z).normalized;
        Vector3 direction = Vector3.Cross(radialVector, Vector3.right).normalized;
        float magnitude = current / radialVector.magnitude;
        return magnitude * direction;
    }

    Vector3 getMagneticFieldFromDiscreteLineWire(Vector3 radialVector) {
        Vector3 magneticField = Vector3.zero;

        for(int i = 0; i <= 100; i++) {
            Vector3 chargePosition = Vector3.right * Remap(0,100,-2,2,i);
            Vector3 r = radialVector - chargePosition;
            Vector3 v = Vector3.right * current;
            magneticField += 0.01f * Vector3.Cross(v,-r.normalized) / (r.magnitude * r.magnitude);
        }

        return magneticField;
    }

    Vector3 getMagneticFieldFromDiscreteLoopWire(Vector3 radialVector) {
        Vector3 magneticField = Vector3.zero;

        for(int i = 0; i <= 100; i++) {
            Vector3 chargePosition = Vector3.forward * Mathf.Cos(Remap(0, 100, 0, Mathf.PI * 2, i)) + Vector3.up * Mathf.Sin(Remap(0, 100, 0, Mathf.PI * 2, i));
            Vector3 r = radialVector - chargePosition;
            Vector3 v = Vector3.Cross(Vector3.right,chargePosition).normalized * current;
            magneticField += 0.01f * Vector3.Cross(v,-r.normalized) / (r.magnitude * r.magnitude);
        }

        return magneticField;
    }

    Vector3 getMagneticFieldFromSolenoidWire(Vector3 radialVector) {
        Vector3 magneticField = Vector3.zero;

        for(int j = 0; j <= 1; j++) {
            for(int i = 0; i <= 100; i++) {
                Vector3 chargePosition = Vector3.forward * Mathf.Cos(Remap(0, 100, 0, Mathf.PI * 2, i)) + Vector3.up * Mathf.Sin(Remap(0, 100, 0, Mathf.PI * 2, i)) + Vector3.forward * Remap(0,50,-2,2,j);
                Vector3 r = radialVector - chargePosition;
                Vector3 v = Vector3.Cross(Vector3.right,chargePosition).normalized * current;
                magneticField += 0.01f * Vector3.Cross(v,-r.normalized) / (r.magnitude * r.magnitude);
            }
        }

        return magneticField;
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