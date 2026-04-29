using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PickupShape : MonoBehaviour
{
    public enum Shape { Star, Diamond, Triangle, Cross, Heart, ChevronUp, ChevronDown }

    [SerializeField]
    private Shape ShapeType;

    [SerializeField]
    private float Radius = 0.5f;

    [SerializeField]
    private float Thickness = 0.2f;

    void Awake()
    {
        Mesh mesh = ExtrudePolygon(GetOutline(), Thickness);
        GetComponent<MeshFilter>().mesh = mesh;

        MeshCollider mc = GetComponent<MeshCollider>();
        if (mc != null)
        {
            mc.sharedMesh = mesh;
        }
    }

    private Vector2[] GetOutline()
    {
        switch (ShapeType)
        {
            case Shape.Star: return MakeStar(5, Radius, Radius * 0.45f);
            case Shape.Diamond: return MakeRegular(4, Radius);
            case Shape.Triangle: return MakeRegular(3, Radius);
            case Shape.Cross: return MakeCross(Radius, Radius * 0.35f);
            case Shape.Heart: return MakeHeart(64, Radius);
            case Shape.ChevronUp: return MakeChevron(Radius, true);
            case Shape.ChevronDown: return MakeChevron(Radius, false);
            default: return MakeRegular(4, Radius);
        }
    }

    private Vector2[] MakeRegular(int n, float r)
    {
        Vector2[] pts = new Vector2[n];
        for (int i = 0; i < n; i++)
        {
            float a = Mathf.PI * 2f * i / n + Mathf.PI / 2f;
            pts[i] = new Vector2(Mathf.Cos(a) * r, Mathf.Sin(a) * r);
        }
        return pts;
    }

    private Vector2[] MakeStar(int points, float outer, float inner)
    {
        Vector2[] pts = new Vector2[points * 2];
        for (int i = 0; i < points * 2; i++)
        {
            float a = Mathf.PI * 2f * i / (points * 2) + Mathf.PI / 2f;
            float r = (i % 2 == 0) ? outer : inner;
            pts[i] = new Vector2(Mathf.Cos(a) * r, Mathf.Sin(a) * r);
        }
        return pts;
    }

    // Sampled from the standard heart parametric curve, then shifted so the
    // origin sits inside the kernel — required by the fan triangulation.
    private Vector2[] MakeHeart(int segments, float r)
    {
        Vector2[] pts = new Vector2[segments];
        const float YShift = 3.5f;
        float k = r / 17f;
        for (int i = 0; i < segments; i++)
        {
            float t = Mathf.PI * 2f * i / segments;
            float sin = Mathf.Sin(t);
            float x = 16f * sin * sin * sin;
            float y = 13f * Mathf.Cos(t) - 5f * Mathf.Cos(2f * t) - 2f * Mathf.Cos(3f * t) - Mathf.Cos(4f * t);
            pts[i] = new Vector2(x * k, (y + YShift) * k);
        }
        return pts;
    }

    // Thick V outline. The whole shape is shifted vertically so the origin
    // sits on the apex segment (between outer and inner apex) — required by
    // the fan triangulator.
    private Vector2[] MakeChevron(float r, bool pointingUp)
    {
        float w = 0.9f * r;
        float a = 0.6f * r;
        float b = -0.6f * r;
        float t = 0.3f * r;
        float armSpan = a - b;
        float dC = (t / w) * Mathf.Sqrt(w * w + armSpan * armSpan);
        float innerA = a - dC;
        float innerBx = (b - innerA) * w / armSpan;
        float yShift = (a + innerA) * 0.5f;
        float aS = a - yShift;
        float bS = b - yShift;
        float innerAS = innerA - yShift;

        Vector2[] pts = new Vector2[6];
        if (pointingUp)
        {
            pts[0] = new Vector2(0f, aS);
            pts[1] = new Vector2(-w, bS);
            pts[2] = new Vector2(innerBx, bS);
            pts[3] = new Vector2(0f, innerAS);
            pts[4] = new Vector2(-innerBx, bS);
            pts[5] = new Vector2(w, bS);
        }
        else
        {
            pts[0] = new Vector2(0f, -aS);
            pts[1] = new Vector2(w, -bS);
            pts[2] = new Vector2(-innerBx, -bS);
            pts[3] = new Vector2(0f, -innerAS);
            pts[4] = new Vector2(innerBx, -bS);
            pts[5] = new Vector2(-w, -bS);
        }
        return pts;
    }

    private Vector2[] MakeCross(float arm, float halfWidth)
    {
        return new Vector2[]
        {
            new Vector2(-halfWidth,  arm),       new Vector2( halfWidth,  arm),
            new Vector2( halfWidth,  halfWidth), new Vector2( arm,        halfWidth),
            new Vector2( arm,       -halfWidth), new Vector2( halfWidth, -halfWidth),
            new Vector2( halfWidth, -arm),       new Vector2(-halfWidth, -arm),
            new Vector2(-halfWidth, -halfWidth), new Vector2(-arm,       -halfWidth),
            new Vector2(-arm,        halfWidth), new Vector2(-halfWidth,  halfWidth),
        };
    }

    // Extrudes a 2D polygon outline along the Z axis. Triangulates each cap
    // as a fan from the origin — only valid for star-shaped polygons whose
    // center sees every outline vertex (true for all shapes used here).
    private Mesh ExtrudePolygon(Vector2[] outline, float thickness)
    {
        int n = outline.Length;
        float h = thickness * 0.5f;

        int frontStart = 0;
        int backStart = n;
        int frontCenter = 2 * n;
        int backCenter = 2 * n + 1;

        Vector3[] verts = new Vector3[2 * n + 2];
        for (int i = 0; i < n; i++)
        {
            verts[frontStart + i] = new Vector3(outline[i].x, outline[i].y, -h);
            verts[backStart + i] = new Vector3(outline[i].x, outline[i].y, h);
        }
        verts[frontCenter] = new Vector3(0, 0, -h);
        verts[backCenter] = new Vector3(0, 0, h);

        int[] tris = new int[n * 4 * 3];
        int t = 0;

        for (int i = 0; i < n; i++)
        {
            int next = (i + 1) % n;
            tris[t++] = frontCenter;
            tris[t++] = frontStart + next;
            tris[t++] = frontStart + i;
        }

        for (int i = 0; i < n; i++)
        {
            int next = (i + 1) % n;
            tris[t++] = backCenter;
            tris[t++] = backStart + i;
            tris[t++] = backStart + next;
        }

        for (int i = 0; i < n; i++)
        {
            int next = (i + 1) % n;
            int a = frontStart + i;
            int b = frontStart + next;
            int c = backStart + next;
            int d = backStart + i;
            tris[t++] = a; tris[t++] = b; tris[t++] = c;
            tris[t++] = a; tris[t++] = c; tris[t++] = d;
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }
}
