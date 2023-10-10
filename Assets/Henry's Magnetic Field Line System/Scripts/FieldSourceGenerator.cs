using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class FieldSourceGenerator : MonoBehaviour {
    [Range(0,1)] public int visualizationMode = 0; 

    [Header("Charge-Spawning Visualization Mode Settings")]
    [Range(0,250)] public int fieldSourceCount = 61;
    [Range(0,1)] public float sphereRadius = 0.015f;

    [Header("Grid-Spawning Visualization Mode Settings")]
    [Range(0,250)] public int resolution = 33;
    [Range(0,1f)] public float spacing = 0.285f;

    [Header("Magnet List")]
    public List<GameObject> magnets;
    public Vector4[] charges;
    private const int maxMagnetCount = 5;

    [Header("References")]
    public MeshFilter meshFilter;
    public Material FieldLineGeneratorMaterial;

    private float goldenRatio = (1 + Mathf.Sqrt(5))/2f;

    private void Update() {
        //Extracting magnetic "charges" from magnet gameObjects -- feel free to replace this to fit your own data structures, all the program requires is an array of magnetic "charges"
        //Note that the size of the charge array is fixed in order to match the shader code but maxMagnetCount can be arbitrarily large
        charges = new Vector4[maxMagnetCount * 2];
        int chargeIndex = 0;

        foreach(GameObject magnet in magnets) {
            foreach(Transform pole in magnet.transform) {
                if(pole.gameObject.tag == "NegativePole") {
                    //xyz components store charge position and the w component stores charge
                    charges[chargeIndex] = new Vector4(pole.position.x, pole.position.y, pole.position.z, -1);
                    ++chargeIndex;
                } else if(pole.gameObject.tag == "PositivePole") {
                    charges[chargeIndex] = new Vector4(pole.position.x, pole.position.y, pole.position.z, 1);
                    ++chargeIndex;
                }
            }
        }

        chargeIndex = 0;

        //Init field line source point mesh
        Mesh fieldSourceMesh = new Mesh();

        Vector3[] vertices;
        int[] indices;
        Vector2[] uv;

        if(visualizationMode == 0) {
            //Creating a point mesh for field line sources around magentic "charges" using the Fibonacci lattice
            vertices = new Vector3[magnets.Count * fieldSourceCount * 2];
            indices = new int[magnets.Count * fieldSourceCount * 2];
            uv = new Vector2[magnets.Count * fieldSourceCount * 2];

            foreach(Vector4 charge in charges) {
                if(charge.w == 0) {continue;}
                
                for(int i = 0; i < fieldSourceCount; i++) {
                    float theta = 2 * Mathf.PI * i / goldenRatio;
                    float phi = Mathf.Acos(1 - 2 * (i + 0.5f) / fieldSourceCount);

                    vertices[i + fieldSourceCount * chargeIndex] = new Vector3(charge.x, charge.y, charge.z) - sphereRadius * new Vector3(Mathf.Cos(theta) * Mathf.Sin(phi), Mathf.Sin(theta) * Mathf.Sin(phi), Mathf.Cos(phi));
                    indices[i + fieldSourceCount * chargeIndex] = i + fieldSourceCount * chargeIndex;
                    uv[i + fieldSourceCount * chargeIndex] = new Vector2(charge.w, 0);
                }

                ++chargeIndex;
            } 
        } else if(visualizationMode == 1) {
            //Creating a point mesh for field line sources in a grid pattern
            vertices = new Vector3[resolution * resolution * resolution * 2];
            indices = new int[resolution * resolution * resolution * 2];
            uv = new Vector2[resolution * resolution * resolution * 2];

            int vertexIndex = 0;
            for(int i = 0; i < resolution; i++) {
                for(int j = 0; j < resolution; j++) {
                    for(int k = 0; k < resolution; k++) {
                        vertices[vertexIndex] = new Vector3(-resolution * spacing * 0.5f + spacing * i, -resolution * spacing * 0.5f + spacing * j, -resolution * spacing * 0.5f + spacing * k);
                        indices[vertexIndex] = vertexIndex;
                        uv[vertexIndex] = new Vector2(1, 0);
                        
                        vertexIndex++;
                    }
                }
            }

            for(int i = 0; i < resolution; i++) {
                for(int j = 0; j < resolution; j++) {
                    for(int k = 0; k < resolution; k++) {
                        vertices[vertexIndex] = new Vector3(-resolution * spacing * 0.5f + spacing * i, -resolution * spacing * 0.5f + spacing * j, -resolution * spacing * 0.5f + spacing * k);
                        indices[vertexIndex] = vertexIndex;
                        uv[vertexIndex] = new Vector2(-1, 0);
                        
                        vertexIndex++;
                    }
                }
            } 
        } else {
            vertices = new Vector3[0];
            indices = new int[0];
            uv = new Vector2[0];
        }

        //Pushing the computed mesh to the mesh filter
        fieldSourceMesh.vertices = vertices;
        fieldSourceMesh.SetIndices(indices, MeshTopology.Points, 0);
        fieldSourceMesh.SetUVs(0, uv);

        meshFilter.sharedMesh = fieldSourceMesh;

        //Sending charge data to the shader
        FieldLineGeneratorMaterial.SetFloat("_NumberOfCharges", magnets.Count * 2);
        if(magnets.Count > 0) {FieldLineGeneratorMaterial.SetVectorArray("_Charges", charges);}
    }
}
