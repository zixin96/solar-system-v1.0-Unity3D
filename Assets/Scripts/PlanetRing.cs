using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRing : MonoBehaviour
{
  // Manual Settings
  // How many segments the ring made up of:
 // Less: more angular; More: Rounder
  [Range(3, 360)]
  public int segments = 3; //Set to 3 unless default to 0
  // How far away from the center of the planet the ring starts
  // At least bigger than surface of planet
  public float innerRadius = 0.7f; // Basic sphere is 0.5f
  public float thickness = 0.5f; // Width of the ring
  public Material ringMat;

  // Cache References: so that we can easily access ring
  GameObject ring;
  Mesh ringMesh;
  MeshFilter ringMF;
  MeshRenderer ringMR;

    // Start is called before the first frame update
    void Start()
    {
      // Create Ring Object
      ring = new GameObject(name + "'s Ring");
      // Parent the ring to the planet
      ring.transform.parent = transform;
      // Make sure offset, scale, position, etc resets to 0 based on planet
      ring.transform.localScale = Vector3.one; // Scale of 1 on x, y, z
      ring.transform.localPosition = Vector3.zero; // Position (0,0,0) (Same as planet)
      ring.transform.localRotation = Quaternion.identity; // NO rotation

      // Cache References
      ringMF = ring.AddComponent<MeshFilter>();
      ringMesh = ringMF.mesh;
      ringMR = ring.AddComponent<MeshRenderer>();
      ringMR.material = ringMat;


      // Build Ring Mesh
      // + 1: Have another set of vertices at the very end: don't want them to have same UV coords
      // first * 2: both innerRadius and outerRadius
      // Second * 2: Top and bottom side of the ring
      Vector3[] vertices = new Vector3[(segments + 1) * 2 * 2];
      // * 6: one segment is a quad in our Mesh
      // * 2: Top and bottom side
      int[] triangles = new int[segments * 6 * 2];
      Vector2[] uv = new Vector2[(segments + 1) * 2 * 2];
      // Doing top and bottom side: halfway is an offset to bottom side
      int halfway = (segments + 1) * 2;

      // Loop through each segments to create out quads
      for(int i = 0; i < segments + 1; i++){
        float progress = (float)i / (float)segments; //Progress from 0 to 1
        float angle = Mathf.Deg2Rad * progress * 360; // How much progress in degress and convert to rads
        // Rings go around equator
        float x = Mathf.Sin(angle);
        float z = Mathf.Cos(angle);

        vertices[i * 2] = vertices[i * 2 + halfway] = new Vector3(x, 0f, z) * (innerRadius + thickness);
        vertices[i * 2 + 1] = vertices[i * 2 + 1 + halfway] = new Vector3(x, 0f, z) * innerRadius;
        uv[i * 2] = uv[i * 2 + halfway] = new Vector2(progress, 0f);
        uv[i * 2 + 1] = uv[i * 2 + 1 + halfway] = new Vector2(progress, 1f);

        if(i != segments){
          // 12: 4 sets of triangles, two quads for top and two for bottom
          triangles[i * 12] = i * 2;
          triangles[i * 12 + 1] = triangles[i * 12 + 4] = (i + 1) * 2;
          triangles[i * 12 + 2] = triangles[i * 12 + 3] = i * 2 + 1;
          triangles[i * 12 + 5] = (i + 1) * 2 + 1;

          triangles[i * 12 + 6] = i * 2 + halfway;
          triangles[i * 12 + 7] = triangles[i * 12 + 10] = i * 2 + 1 + halfway;
          triangles[i * 12 + 8] = triangles[i * 12 + 9] = (i + 1) * 2 + halfway;
          triangles[i * 12 + 11] = (i + 1) * 2 + 1 + halfway;
        }
      }

      ringMesh.vertices = vertices;
      ringMesh.triangles = triangles;
      ringMesh.uv = uv;
      ringMesh.RecalculateNormals();

    }
}
