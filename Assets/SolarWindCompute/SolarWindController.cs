using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

[ExecuteInEditMode]
public class SolarWindController : MonoBehaviour
{
    ParticleSystem vectorFieldParticleSystem;
    ParticleSystem.EmissionModule emmission;
    ParticleSystem.Particle[] particles;

    [Range(0, 1000)]
    public float lerpMax;

    public float particleCharge;
    public float particleMass;
    public float earthMagneticFieldMagnitude;
    public float earthRadius;

    [SerializeField]
    public Transform testPoint;
    public GameObject testPointObj;
    public GameObject EarthObj;

    public Vector3 testVelocity = new Vector3(1, 0, 0);

    public Vector3 earthDipole = new Vector4(0, Mathf.Cos(0.204203522f), Mathf.Sin(0.204203522f));

    public ComputeShader computeShader;
    private ComputeBuffer velocityBuffer;
    private ComputeBuffer positionBuffer;
    private ComputeBuffer bfieldBuffer;
    private int kernelID;

    private Vector3[] velocityArray;
    private Vector3[] positionArray;
    private Vector3[] bfieldValArray;

    public int counter = 0;

    private const int threadCount = 1024;

    void Update()
    {
        float UUScaleFactor = 12756000 / EarthObj.transform.localScale.x;

        // Test Point Position (Probing Magnetic Field Values)
        Vector3 test = new Vector4(testPointObj.transform.position.x, testPointObj.transform.position.y, testPointObj.transform.position.z);

        counter++;

        //Read-in alive particles
        InitializeParticleSystem();
        int aliveParticleCount = vectorFieldParticleSystem.GetParticles(particles);

        if (aliveParticleCount == 0 || vectorFieldParticleSystem.isPaused) { return; }

        //Allocate position and field buffers
        positionBuffer = new ComputeBuffer(aliveParticleCount, sizeof(float) * 3);
        positionArray = new Vector3[aliveParticleCount];

        velocityBuffer = new ComputeBuffer(aliveParticleCount, sizeof(float) * 3);
        velocityArray = new Vector3[aliveParticleCount];

        bfieldBuffer = new ComputeBuffer(1, sizeof(float) * 3);
        bfieldValArray = new Vector3[1];

        //Read-in position data to position buffer
        for (int i = 0; i < aliveParticleCount; i++)
        {
            positionArray[i] = particles[i].position - particles[i].velocity * Time.deltaTime;
            velocityArray[i] = particles[i].velocity;
        }

        positionBuffer.SetData(positionArray);
        velocityBuffer.SetData(velocityArray);
        bfieldBuffer.SetData(bfieldValArray);

        //Set buffers and dispatch compute shader
        computeShader.SetBuffer(kernelID, "_Position", positionBuffer);
        computeShader.SetBuffer(kernelID, "_Velocity", velocityBuffer);

        computeShader.SetBuffer(kernelID, "_BField", bfieldBuffer);

        computeShader.SetInt("_ParticleCount", aliveParticleCount);

        //Particle Parameters
        computeShader.SetFloat("_particleCharge", particleCharge);
        computeShader.SetFloat("_particleMass", particleMass);
        computeShader.SetFloat("_initialSpeed", vectorFieldParticleSystem.main.startSpeed.constant);
        //Earth Parameters
        computeShader.SetFloat("_earthMagneticFieldMagnitude", earthMagneticFieldMagnitude);
        computeShader.SetFloat("_earthRadius", EarthObj.transform.localScale.x / 2);
        computeShader.SetFloat("_scaleFactor", UUScaleFactor);
        computeShader.SetVector("_earthDipole", earthDipole);
        computeShader.SetVector("_earthPos", EarthObj.transform.position);

        computeShader.SetFloat("_dt", Time.deltaTime);
        computeShader.SetVector("_testPointTransform", test);

        computeShader.Dispatch(kernelID, (int)Mathf.Ceil(aliveParticleCount / (float)threadCount), 1, 1);

        positionBuffer.GetData(positionArray);
        velocityBuffer.GetData(velocityArray);
        bfieldBuffer.GetData(bfieldValArray);

        for (int i = 0; i < aliveParticleCount; i++)
        {
            particles[i].position = positionArray[i];
            particles[i].velocity = velocityArray[i];

            /*if (particles[i].position.magnitude > 0 && counter % 2 == 0)
            {
                Debug.Log("Test Point Position: x: " + testPoint.position.x + " y: " + testPoint.position.y + " z: "+testPoint.position.z +"\n"+ "Test Point B-Field: x: " + bfieldValArray[0].x + " y: " + bfieldValArray[0].y + " z: " + bfieldValArray[0].z);
                //Debug.Log("ParticlePos: " + particles[i].position);
                Vector3 crossVec = Vector3.Cross(testVelocity, bfieldValArray[0]);
                Debug.Log("v X B: x: " + crossVec.x + " y: " + crossVec.y + " z: " + crossVec.z + "\n" + "Particle Velocity: x: " + particles[i].velocity.x + "y: " + particles[i].velocity.y + "z: " + particles[i].velocity.z);
            }*/
            if (particles[i].position.magnitude < earthRadius) { particles[i].startLifetime = 0; }

            particles[i].startColor = Remap(0, lerpMax, Color.blue, Color.red, lerpMax - particles[i].position.magnitude);
        }

        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Space pressed, following particle");
            followParticle();
        }


        /* if (testPoint.transform.hasChanged)
         {
             Debug.Log(testPoint.position);
             testPoint.transform.hasChanged = false;
             Debug.Log(bfieldValArray.Length);
             Debug.Log(bfieldValArray[0]);
             Debug.Log(particles[0].position);
         }*/


        vectorFieldParticleSystem.SetParticles(particles, aliveParticleCount);

        positionBuffer.Release();
        velocityBuffer.Release();
        bfieldBuffer.Release();

    }

    public void followParticle()
    {
        InitializeParticleSystem();
        int aliveParticleCount = vectorFieldParticleSystem.GetParticles(particles);

        if (aliveParticleCount == 0 || vectorFieldParticleSystem.isPaused) { return; }

        for (int i = 0; i < aliveParticleCount; i++)
        {
            testVelocity = particles[i].velocity;
            testPointObj.transform.position = particles[i].position;
            Debug.Log("Test Point Position: x: " + testPoint.position.x + " y: " + testPoint.position.y + " z: " + testPoint.position.z + "\n"
            + "Test Point B-Field: x: " + bfieldValArray[0].x + " y: " + bfieldValArray[0].y + " z: " + bfieldValArray[0].z);
            Vector3 crossVec = Vector3.Cross(testVelocity, bfieldValArray[0]);
            Debug.Log("v X B: x: " + crossVec.x + " y: " + crossVec.y + " z: " + crossVec.z + "\n" + "Particle Velocity: x: " + particles[i].velocity.x + "y:" +
                " " + particles[i].velocity.y + "z: " + particles[i].velocity.z);
        }
    }

    void InitializeParticleSystem()
    {
        if (vectorFieldParticleSystem == null)
        {
            vectorFieldParticleSystem = GetComponent<ParticleSystem>();
            emmission = vectorFieldParticleSystem.emission;
        }

        if (particles == null || particles.Length < vectorFieldParticleSystem.main.maxParticles)
        {
            particles = new ParticleSystem.Particle[vectorFieldParticleSystem.main.maxParticles];
        }
    }

    private void OnEnable()
    {
        kernelID = computeShader.FindKernel("CSMain");
    }

    private void OnDisable()
    {
        if (velocityBuffer != null)
        {
            velocityBuffer.Release();
            velocityBuffer = null;
        }
    }

    bool checkIfOutsideBoundry(Vector3 radialVector)
    {
        if (radialVector.magnitude > 6) { return true; }
        return false;
    }

    float Remap(float iMin, float iMax, float oMin, float oMax, float v)
    {
        float t = Mathf.InverseLerp(iMin, iMax, v);
        return Mathf.Lerp(oMin, oMax, t);
    }

    Color Remap(float iMin, float iMax, Color oMin, Color oMax, float v)
    {
        float t = Mathf.InverseLerp(iMin, iMax, v);
        return Color.Lerp(oMin, oMax, t);
    }

    Vector3 NormalizePosition(Vector3 position)
    {
        float magnitude = position.magnitude;
        if (magnitude > 0)
        {
            return position / magnitude;
        }
        return position;
    }

    Vector3 NormalizeVelocity(Vector3 velocity)
    {
        float magnitude = velocity.magnitude;
        if (magnitude > 0)
        {
            return velocity / magnitude;
        }
        return velocity;
    }
}