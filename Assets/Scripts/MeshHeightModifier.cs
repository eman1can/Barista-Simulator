using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHeightModifier : MonoBehaviour {
    [SerializeField] private Mesh baseMesh;
    [SerializeField, Range(0, 1)] private float percent;

    private MeshFilter _meshFilter;
    private Mesh cutMesh;
    
    private Dictionary<float, LinkedList<int>> verticesByHeights;
    
    private float height;
    private float bottom;

    private float cutHeight;
    private float beforeCutHeight;
    private float afterCutHeight;
    private void OnValidate() {
        if (verticesByHeights == null)
            SetupMeshData();

        CutMesh();
    }

    private void Start() {
        if (verticesByHeights == null)
            SetupMeshData();
    }

    private void SetupMeshData() {
        height = baseMesh.bounds.size.y;
        bottom = baseMesh.bounds.min.y;
        _meshFilter = GetComponent<MeshFilter>();

        GenerateVerticesByHeight();
        // GenerateTrianglesByHeight();
    }

    private float Round(float v) {
        return (float) Math.Round(v, 5);
    }
    
    private float CalculateDistance(Vector3 x, Vector3 y) {
        var xd = Mathf.Pow(y.x - x.x, 2);
        var yd = Mathf.Pow(y.y - x.y, 2);
        var zd = Mathf.Pow(y.z - x.z, 2);
        
        if (yd < 0.0000001) {
            return -1;
        }

        return Round(Mathf.Sqrt(xd + yd + zd));
    }
    
    private void GenerateVerticesByHeight() {
        verticesByHeights = new Dictionary<float, LinkedList<int>>();
        var vertices = baseMesh.vertices;

        for (int i = 0; i < vertices.Length; i++) {
            var height = Round(vertices[i].y);
            LinkedList<int> indices;
            if (verticesByHeights.TryGetValue(height, out indices)) {
                indices.AddLast(i);
            } else {
                indices = new LinkedList<int>();
                indices.AddLast(i);
                verticesByHeights.Add(height, indices);
            }
        }
        foreach (KeyValuePair<float, LinkedList<int>> entry in verticesByHeights) {
            print(entry.Key + " verts " + entry.Value.Count);
        }
    }
    
    /*private void GenerateTrianglesByHeight() {
        var trianglesByHeights = new Dictionary<float, LinkedList<int>>();
        var triangles = baseMesh.triangles;
        var vertices = baseMesh.vertices;
        foreach (KeyValuePair<float, LinkedList<int>> entry in verticesByHeights) {
            LinkedList<int> triangleIndices = new LinkedList<int>();
            foreach (int vertex in entry.Value) {
                for (int t = 0; t < triangles.Length; t += 3) {
                    int t1 = triangles[t];
                    int t2 = triangles[t + 1];
                    int t3 = triangles[t + 2];

                    if (t1 == vertex || t2 == vertex || t3 == vertex) {
                        triangleIndices.AddLast(t1);
                        triangleIndices.AddLast(t2);
                        triangleIndices.AddLast(t3);
                    }
                }
            }

            trianglesByHeights.Add(entry.Key, triangleIndices);
        }
    }*/
    
    private void GetBeforeAndAfterCut(out LinkedList<int> beforeCut, out LinkedList<int> afterCut) {
        cutHeight = bottom + height * percent;
        beforeCutHeight = bottom;
        afterCutHeight = bottom + height;

        beforeCut = null;
        afterCut = null;
        foreach (KeyValuePair<float, LinkedList<int>> entry in verticesByHeights) {
            if (entry.Key > beforeCutHeight && entry.Key < cutHeight) {
                beforeCutHeight = entry.Key;
                beforeCut = entry.Value;
            }

            if (entry.Key < afterCutHeight && entry.Key > cutHeight) {
                afterCutHeight = entry.Key;
                afterCut = entry.Value;
            }
        }
    }

    private void CutMesh() {
        
        if (verticesByHeights == null) {
            SetupMeshData(); 
        }
        if (percent == 0) {
            cutMesh = null;
            _meshFilter.mesh = null;
            return;
        }
        
        LinkedList<int> beforeCut = null;
        LinkedList<int> afterCut = null;
        GetBeforeAndAfterCut(out beforeCut, out afterCut);

        if (beforeCut == null || afterCut == null)
            return;

        Dictionary<int, int> edges; // Goes from after to before
        Dictionary<int, int> newEdges; // Goes from before to new
        Dictionary<int, int> links; // Goes from after x to after x + 1
        GenerateEdgesAndLinks(beforeCut, afterCut, out edges, out links);

        LinkedList<Vector3> newPoints;
        GenerateNewPoints(afterCut, edges, out newEdges, out newPoints);
        


    }

    private void GenerateNewPoints(LinkedList<int> afterCut, Dictionary<int, int> edges,
        out Dictionary<int, int> newEdges, out LinkedList<Vector3> newPoints) {

        newEdges = new Dictionary<int, int>();
        newPoints = new LinkedList<Vector3>();
        
        float mult = (cutHeight - beforeCutHeight) / (afterCutHeight - beforeCutHeight);
        var vertices = baseMesh.vertices;

        int index = vertices.Length;
        Gizmos.color = Color.green;
        foreach (int v1 in afterCut) {
            int v2 = edges[v1];
            
            var vertex1 = vertices[v2];
            var vertex2 = vertices[v1];
            
            var xp = (vertex2.x - vertex1.x) * mult;
            var yp = (vertex2.y - vertex1.y) * mult;
            var zp = (vertex2.z - vertex1.z) * mult;
            
            newPoints.AddLast(new Vector3(xp + vertex1.x, yp + vertex1.y, zp + vertex1.z));
            newEdges.Add(v2, index++);
            DrawLine(vertex1, newPoints.Last.Value);
        }
    }
    
    private void GenerateEdgesAndLinks(LinkedList<int> beforeCut, LinkedList<int> afterCut, out Dictionary<int, int> edges, out Dictionary<int, int> links) {
        edges = new Dictionary<int, int>();
        links = new Dictionary<int, int>();
        
        var triangles = baseMesh.triangles;
        var vertices = baseMesh.vertices;
        for (var triangleIndex = 0; triangleIndex < triangles.Length; triangleIndex += 3) {
            var v1 = triangles[triangleIndex];
            var v2 = triangles[triangleIndex + 1];
            var v3 = triangles[triangleIndex + 2];
            
            var beforeCount = 0;
            var afterCount = 0;

            if (beforeCut.Contains(v1))
                beforeCount++;
            else if (afterCut.Contains(v1))
                afterCount++;
            if (beforeCut.Contains(v2))
                beforeCount++;
            else if (afterCut.Contains(v2))
                afterCount++;
            if (beforeCut.Contains(v3))
                beforeCount++;
            else if (afterCut.Contains(v3))
                afterCount++;

            //Gizmos.color = Color.white;
            //DrawTriangle(vertices[v1], vertices[v2], vertices[v3]);
            
            /*if (afterCount == 0 || beforeCount == 0 || (beforeCount + afterCount) < 3)
                continue;*/
            
            var edge1 = CalculateDistance(vertices[v1], vertices[v2]);
            var edge2 = CalculateDistance(vertices[v2], vertices[v3]);
            var edge3 = CalculateDistance(vertices[v3], vertices[v1]);
            
            int x1;
            int x2;
            int x3;

            if (edge1 == -1) {
                // Link between v1 && v2
                if (edge1 == edge3) {
                    if ((edges.ContainsKey(v1) && edges[v1] == v3) || (edges.ContainsKey(v3) && edges[v3] == v1)) {
                        x1 = v1;
                        x2 = v2;
                    } else {
                        x1 = v2;
                        x2 = v1;
                    }
                } else if (edge2 < edge3) {
                    // v2 → v3 is shortest
                    x1 = v2;
                    x2 = v1;
                } else {
                    // v1 → v3 is shortest
                    x1 = v1;
                    x2 = v2;
                }
                x3 = v3;
            } else if (edge2 == -1) {
                // Link between v2 && v3
                if (edge1 == edge3) {
                    if ((edges.ContainsKey(v2) && edges[v2] == v1) || (edges.ContainsKey(v1) && edges[v1] == v2)) {
                        x1 = v3;
                        x2 = v2;
                    } else {
                        x1 = v2;
                        x2 = v3;
                    }
                } else if (edge1 < edge3) {
                    // v1 → v2 is shortest
                    x1 = v2;
                    x2 = v3;
                } else {
                    // v3 → v1 is shortest
                    x1 = v3;
                    x2 = v2;
                }
                x3 = v1;
            } else {
                // Link between v3 && v1
                if (edge1 == edge2) {
                    if ((edges.ContainsKey(v1) && edges[v1] == v2) || (edges.ContainsKey(v2) && edges[v2] == v1)) {
                        x1 = v3;
                        x2 = v1;
                    } else {
                        x1 = v1;
                        x2 = v3;
                    }
                } else if (edge1 < edge2) {
                    // v1 → v2 is shortest
                    x1 = v1;
                    x2 = v3;
                } else {
                    // v2 → v3 is shortest
                    x1 = v3;
                    x2 = v1;
                }
                x3 = v2;
            }

            if (afterCut.Contains(x1)) {
                if (!links.ContainsKey(x1))
                    links.Add(x1, x2);
            }
        }
    }
    
    private Vector3 TransformPoint(Vector3 p) {
        var o = new Vector3(p.x, p.y, p.z);
        o.x *= transform.lossyScale.x;
        o.y *= transform.lossyScale.y;
        o.z *= transform.lossyScale.z;
        o = transform.rotation * o;
        o += transform.position;
        return o;
    }

    private void DrawLine(Vector3 v1, Vector3 v2) {
        var p1 = TransformPoint(v1);
        var p2 = TransformPoint(v2);
        Gizmos.DrawLine(p1, p2);
    }

    private void DrawTriangle(Vector3 v1, Vector3 v2, Vector3 v3) {
        DrawLine(v1, v2);
        DrawLine(v2, v3);
        DrawLine(v3, v1);
    }

    private void OnDrawGizmos() {
        CutMesh();
    }
}