using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SplitAreaGenerator : MonoBehaviour
{
    public bool DebugGenerator = true;
    public int BigHorizontalSliceCount = 6;
    public int SmallHorizontalSliceCount = 3;
    //public int BigVerticalSliceCount = 1;
    //public int SmallVerticalSliceCount = 1;
    //public int InnerSliceCount = 1;
    public float BigSplitAngle = 20;
    public float SmallSplitAngle = 20;
    public float Radius = 2;
    //public float CenterOffset = 0.25f;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private List<Vector3> vertices = new List<Vector3>();
    private List<List<Vector3>> chunkVertices = new List<List<Vector3>>();

    public int DrawChunkIndex = 0;
    private int prevDrawChunkIndex = -1;

    public GameObject ChunkPrefab = null;

    private void Awake() {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        CreateVertices();
    }

    public void Generate(){
        CreateVertices();

        for (int i = 0; i < SmallHorizontalSliceCount+BigHorizontalSliceCount; i++)
        {
            var chunk = Instantiate(ChunkPrefab, transform);
            var chunkMeshFilter = chunk.GetComponent<MeshFilter>();
            chunkMeshFilter.mesh = CreateMesh(i);
        }
    }

    private void Update() {
        if(DebugGenerator){
            CreateVertices();

            if(prevDrawChunkIndex != DrawChunkIndex){
                if(DrawChunkIndex>=0 && DrawChunkIndex<chunkVertices.Count){
                    meshFilter.mesh = CreateMesh(DrawChunkIndex);
                    prevDrawChunkIndex = DrawChunkIndex;
                }
            }
        }else{
            meshFilter.mesh = new Mesh();
            prevDrawChunkIndex = -1;
            DrawChunkIndex = 0;
        }
    }

    private Mesh CreateMesh(int idx){
        Mesh mesh = new Mesh();
                
        mesh.vertices = chunkVertices[idx].ToArray();
        mesh.triangles = new int[]{
            0,1,2,
            0,2,4,
            0,4,3,
            0,3,1,
            1,4,2,
            1,3,4
        };
        
        mesh.uv = new Vector2[]{
            new Vector2(0,0),
            new Vector2(0,0),
            new Vector2(0,0),
            new Vector2(0,0),
            new Vector2(0,0)
        };

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        mesh.RecalculateUVDistributionMetrics();
        mesh.OptimizeIndexBuffers();
        mesh.OptimizeReorderVertexBuffer();

        return mesh;
    }


    private void OnDrawGizmos() {
        if(DebugGenerator){
            DrawAllVertice();

            Gizmos.color = Color.blue;
            for (int i = 0; i < chunkVertices.Count; i++)
            {   
                if(i != DrawChunkIndex) continue;

                var chunk = chunkVertices[i];
                
                foreach (var v in chunk)
                {
                    Gizmos.DrawSphere(v, 0.05f);    
                }
            }
            
            //DrawPlanes();
        }
    }

    private void DrawAllVertice(){
        Gizmos.color = Color.yellow;
        for (int i = 0; i < vertices.Count; i++)
        {   
            var v = vertices[i];
            Gizmos.DrawSphere(v, 0.05f);
        }

        for (int i = 1; i < vertices.Count; i++)
        {   
            var v = vertices[i];
            Gizmos.DrawLine(vertices[0], v);
        }

        for (int i = 0; i < vertices.Count-1; i++)
        {   
            Gizmos.DrawLine(vertices[i], vertices[i+1]);
        }
        Gizmos.DrawLine(vertices[vertices.Count-1], vertices[1]);
    }

    private void DrawPlanes(){
        var planeNormal = Quaternion.AngleAxis(BigSplitAngle, Vector3.forward) * Vector3.up;
        DrawPlane(Vector3.zero, planeNormal);

        planeNormal = Quaternion.AngleAxis(SmallSplitAngle, Vector3.back) * Vector3.up;
        DrawPlane(Vector3.zero, planeNormal);
        
        Gizmos.color = Color.red;
        var bigVector = new Vector3(1,Mathf.Tan((BigSplitAngle/180f)*Mathf.PI),0);
        var smallVector = new Vector3(-1,Mathf.Tan((SmallSplitAngle/180f)*Mathf.PI),0);

        Gizmos.DrawSphere(bigVector*Radius, 0.05f);
        Gizmos.DrawSphere(smallVector*Radius, 0.05f);
    }

    void DrawPlane(Vector3 position, Vector3 normal, float size = 2f)
    {
        Vector3 v3;
 
        if (normal.normalized != Vector3.forward)
            v3 = Vector3.Cross(normal, Vector3.forward).normalized * normal.magnitude;
        else
            v3 = Vector3.Cross(normal, Vector3.up).normalized * normal.magnitude;;
            
        var corner0 = (position + v3)*size;
        var corner2 = (position - v3)*size;
        var q = Quaternion.AngleAxis(90f, normal);
        v3 = q * v3;
        var corner1 = (position + v3)*size;
        var corner3 = (position - v3)*size;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(corner0, corner2);
        Gizmos.DrawLine(corner1, corner3);
        Gizmos.DrawLine(corner0, corner1);
        Gizmos.DrawLine(corner1, corner2);
        Gizmos.DrawLine(corner2, corner3);
        Gizmos.DrawLine(corner3, corner0);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(position, normal);
    }

    private void CreateVertices(){
        vertices.Clear();
        vertices.Add(Vector3.zero);

        vertices.AddRange(CreateHalfVertices(BigHorizontalSliceCount, 0));
        vertices.AddRange(CreateHalfVertices(SmallHorizontalSliceCount, 0, Mathf.PI));
        
        vertices.AddRange(CreateHalfVertices(BigHorizontalSliceCount, BigSplitAngle));
        vertices.AddRange(CreateHalfVertices(SmallHorizontalSliceCount, SmallSplitAngle, Mathf.PI, true));

        chunkVertices = GetChunkVertices();
    }

    private List<Vector3> CreateHalfVertices(int sliceCount, float splitAngle, float sliceAngleOffset = 0, bool invertY=false){
        var vertices = new List<Vector3>();
        var vector = new Vector3(-1,Mathf.Tan((splitAngle/180f)*Mathf.PI),0);
        var sliceAngle = Mathf.PI/sliceCount;

        Debug.Log($"CreateHalfVertices {sliceAngle}");
        for (int i = 0; i < sliceCount; i++)
        {
            var angle = sliceAngleOffset+i*sliceAngle;
            Debug.Log($"SmallHorizontalSlice {i}:{angle}");
            var v = new Vector3(Radius*Mathf.Cos(angle-Mathf.PI/2f), 0, Radius*Mathf.Sin(angle-Mathf.PI/2f));
            v.y = (vector * v.x).y * (invertY?-1f:1f);
            vertices.Add(v);
            Debug.Log($"SmallHorizontalSlice {i}:{v}");
        }

        return vertices;
    }

    private List<List<Vector3>> GetChunkVertices()
    {
        var result = new List<List<Vector3>>();
        var v0 = vertices[0];

        var aroundCount = BigHorizontalSliceCount+SmallHorizontalSliceCount;
        for (int i = 0; i < BigHorizontalSliceCount; i++)
        {
            var chunkVertice = new List<Vector3>();
            chunkVertice.Add(v0);
            chunkVertice.Add(vertices[1+i]);
            chunkVertice.Add(vertices[1+i+1]);
            chunkVertice.Add(vertices[1+i+aroundCount]);
            chunkVertice.Add(vertices[1+i+aroundCount+1]);

            result.Add(chunkVertice);
        }

        for (int i = BigHorizontalSliceCount; i < BigHorizontalSliceCount+SmallHorizontalSliceCount; i++)
        {
            var chunkVertice = new List<Vector3>();
            chunkVertice.Add(v0);
            chunkVertice.Add(vertices[1+i]);
            chunkVertice.Add(vertices[1+i+1]);
            chunkVertice.Add(vertices[1+i+aroundCount]);
            var lastIdx = 1+i+aroundCount+1;
            if(lastIdx>=vertices.Count){
                chunkVertice.Add(vertices[1]);
            }else{
                chunkVertice.Add(vertices[lastIdx]);
            }
            

            result.Add(chunkVertice);
        }

        return result;
    }
}
