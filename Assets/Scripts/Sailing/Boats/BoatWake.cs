using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WakeTrailSection
{
    public Vector3 point;
    public Vector3 rightDir;
    public float time;
}

public class BoatWake : MonoBehaviour
{
    /*
     Generates a trail that is always facing upwards using the scriptable mesh interface.
     vertex colors and uv's are generated similar to the builtin Trail Renderer.
     To use it
     1. create an empty game object
     2. attach this script and a MeshRenderer
     3. Then assign a particle material to the mesh renderer
    */
    public float width = 20.0f;
    public float time = 10.0f;
    public bool alwaysUp = false;
    public float minDistance = 0.1f;

    public Color startColor = Color.white;
    public Color endColor = new Color(1, 1, 1, 0);
    public AnimationCurve WakeShapeCurve = new AnimationCurve();
    public List<WakeTrailSection> sections;

    void LateUpdate()
    {
        //Vector3 position = transform.position;

        // Remove old sections
        List<WakeTrailSection> sectionsToRemove = new List<WakeTrailSection>();
        for (int i=0; i<sections.Count; i++) {
            if (Time.time > sections[i].time + time) {
                sectionsToRemove.Add(sections[i]);
            }
        }

        foreach (WakeTrailSection thisSection in sectionsToRemove)
        {
            sections.Remove(thisSection);
        }


        // Add a new trail section
        if (sections.Count == 0 || (sections[0].point - transform.position).sqrMagnitude > minDistance * minDistance)
        {
            WakeTrailSection section = new WakeTrailSection();
            section.point = transform.position;
            if (alwaysUp)
                section.rightDir = Vector3.right;
            else
                section.rightDir = transform.TransformDirection(Vector3.right);
            section.time = Time.time;
            sections.Add(section);
        }

        // Rebuild the mesh
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        // We need at least 2 sections to create the line
        if (sections.Count < 2)
            return;

        Vector3[] vertices = new Vector3[sections.Count * 2];
        Color[] colors = new Color[sections.Count * 2];
        Vector2[] uv = new Vector2[sections.Count * 2];

        WakeTrailSection previousSection = sections[0];
        WakeTrailSection currentSection = sections[0];

        // Use matrix instead of transform.TransformPoint for performance reasons
        //Matrix localSpaceTransform = transform.worldToLocalMatrix;

        // Generate vertex, uv and colors
        for (int i = 0; i < sections.Count; i++)
        {
            previousSection = currentSection;
            currentSection = sections[i];
            // Calculate u for texture uv and color interpolation
            float u = 0.0f;
            if (i != 0)
                u = Mathf.Clamp01((Time.time - currentSection.time) / time);

            // Calculate upwards direction
            Vector3 rightDir = currentSection.rightDir;

            // Generate vertices
            float tVal = Mathf.Clamp01((Time.time - currentSection.time) / time);
            float curveWidth = WakeShapeCurve.Evaluate(tVal) * width;
            vertices[i * 2 + 0] = transform.worldToLocalMatrix.MultiplyPoint(currentSection.point - rightDir * curveWidth);
            vertices[i * 2 + 1] = transform.worldToLocalMatrix.MultiplyPoint(currentSection.point + rightDir * curveWidth);

            uv[i * 2 + 0] = new Vector2(u, 0f);
            uv[i * 2 + 1] = new Vector2(u, 1f);

            // fade colors out over time
            var interpolatedColor = Color.Lerp(startColor, endColor, u);
            colors[i * 2 + 0] = interpolatedColor;
            colors[i * 2 + 1] = interpolatedColor;
        }

        // Generate triangles indices
        int[] triangles = new int[(sections.Count - 1) * 2 * 3];
        for (int i = 0; i < triangles.Length / 6; i++)
        {
            triangles[i * 6 + 0] = i * 2;
            triangles[i * 6 + 1] = i * 2 + 1;
            triangles[i * 6 + 2] = i * 2 + 2;

            triangles[i * 6 + 3] = i * 2 + 2;
            triangles[i * 6 + 4] = i * 2 + 1;
            triangles[i * 6 + 5] = i * 2 + 3;
        }

        // Assign to mesh  
        mesh.vertices = vertices;
        mesh.colors = colors;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}