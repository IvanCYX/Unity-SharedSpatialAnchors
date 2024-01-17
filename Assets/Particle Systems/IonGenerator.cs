using System.Collections.Specialized;
using System.Xml.Serialization;
using UnityEngine;

[ExecuteInEditMode]
public class IonGenerator : MonoBehaviour
{
    ParticleSystem vectorFieldParticleSystem;
    ParticleSystem.Particle[] particles;

    public GameObject positiveCharge;
    public GameObject negativeCharge;

    public Color color1 = new Color(1, 0, 0, 1);
    public Color color2 = new Color(0, 0, 1, 1);

    [Range(-10, 10)]
    public float chargeOnPointCharge = 5.0f;

    [Range(-1, 1)]
    public float chargeOnTestCharge = 1.0f;

    [Range(0.1f, 10)]
    public float massOfTestCharge = 0.5f;

    private Vector3 force;

    private Vector3 magneticField;

    void Update()
    {
        InitializeParticleSystem();
        int aliveParticleCount = vectorFieldParticleSystem.GetParticles(particles);

        Vector3 positiveChargePostion = positiveCharge.gameObject.transform.localPosition;

        Vector3 electricField;

        for (int i = 0; i < aliveParticleCount; i++)
        {
            if (checkIfOutsideBoundry(particles[i].position)) { particles[i].remainingLifetime = 0; }
            electricField = getElectricFieldFromPointCharge(particles[i].position, positiveChargePostion, chargeOnPointCharge);

            //F = qE
            force = 0.1f * chargeOnTestCharge * electricField;
            //particles[i].velocity += force / massOfTestCharge;

            particles[i].startColor = Remap(0, 5, color1, color2, electricField.magnitude);

            //Calculate Lorentz force on particle F = qv x B, F = qE + qv x B, F = q(E + v x B)
            particles[i].velocity = (chargeOnTestCharge * electricField) + (Vector3.Cross((chargeOnPointCharge * particles[i].velocity), magneticField));
            

            //increase to show deflection at higher speeds
            if (electricField.magnitude >= 200) { particles[i].remainingLifetime = 0; }
        }

        vectorFieldParticleSystem.SetParticles(particles, aliveParticleCount);
    }

    void InitializeParticleSystem()
    {
        if (vectorFieldParticleSystem == null)
        {
            vectorFieldParticleSystem = GetComponent<ParticleSystem>();
        }

        if (particles == null || particles.Length < vectorFieldParticleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[vectorFieldParticleSystem.main.maxParticles];
    }

    Vector3 getElectricFieldFromPointCharge(Vector3 radialVector, Vector3 pointChargePosition, float pointChargeCharge)
    {
        return pointChargeCharge * (radialVector - pointChargePosition) / Mathf.Pow((radialVector - pointChargePosition).magnitude, 3);
    }

    bool checkIfOutsideBoundry(Vector3 radialVector)
    {
        if (radialVector.magnitude > 6) { return true; }

        Vector3 positiveChargePostion = positiveCharge.gameObject.transform.localPosition;
        Vector3 negativeChargePostion = negativeCharge.gameObject.transform.localPosition;

        if ((radialVector - positiveChargePostion).magnitude < 0.05 || (radialVector - negativeChargePostion).magnitude < 0.05) { return true; }

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
}