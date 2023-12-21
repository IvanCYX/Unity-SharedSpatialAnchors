using UnityEngine;

public class ChargedParticleSimulation : MonoBehaviour
{
    public ComputeShader magneticFieldShader;
    public ParticleSystem particleSystem;
    public float particleSpeed = 5f;
    public float charge = 1f;
    public float magneticFieldStrength = 1f;

    ComputeBuffer particlesBuffer;

    void Start()
    {
        InitializeParticles();
    }

    void InitializeParticles()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        int numParticlesAlive = particleSystem.GetParticles(particles);

        particlesBuffer = new ComputeBuffer(numParticlesAlive, sizeof(float) * 9);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            particles[i].remainingLifetime = 1;
        }

        particlesBuffer.SetData(particles);
    }

    void Update()
    {
        if (particlesBuffer == null)
            return;

        int kernelID = magneticFieldShader.FindKernel("CSMain");

        magneticFieldShader.SetFloat("DeltaTime", Time.deltaTime);
        magneticFieldShader.SetFloat("Charge", charge);
        magneticFieldShader.SetFloat("MagneticFieldStrength", magneticFieldStrength);

        magneticFieldShader.SetBuffer(kernelID, "ParticlesBuffer", particlesBuffer);
        magneticFieldShader.Dispatch(kernelID, particlesBuffer.count / 32, 1, 1);

        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particlesBuffer.count];
        particlesBuffer.GetData(particles);

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].position += particles[i].velocity * particleSpeed * Time.deltaTime;
        }

        particleSystem.SetParticles(particles, particles.Length);
    }

    void OnDestroy()
    {
        if (particlesBuffer != null)
        {
            particlesBuffer.Release();
            particlesBuffer = null;
        }
    }
}
