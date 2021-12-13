using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = UnityEngine.Vector4;

public class LiquidMeshModifier : MonoBehaviour {
    [SerializeField] private Mesh baseMesh;
    [SerializeField, Range(0, 1)] private float percent;

    private MeshFilter _meshFilter;
    
    private bool startCalled = false;
    private float height;
    private float bottom;

    private float cutHeight;
    private float beforeCutHeight;
    private float afterCutHeight;
    private float lastAfterCutHeight;

    private Vector3[] loopVertices;

    private Mesh cutMesh = null;
    private Dictionary<float, LinkedList<int>> verticesByHeights;
    private Dictionary<float, LinkedList<int>> trianglesByHeights;

    private void OnValidate() {
        CutMesh();
    }

    private void Start() {
        SetupMeshData();
    }

    private void SetupMeshData() {
        height = baseMesh.bounds.size.y;
        bottom = baseMesh.bounds.min.y;
        _meshFilter = GetComponent<MeshFilter>();

        GenerateVerticesByHeight();
        GenerateTrianglesByHeight();
    }

    private void GenerateVerticesByHeight() {
        verticesByHeights = new Dictionary<float, LinkedList<int>>();
        var vertices = baseMesh.vertices;

        for (int i = 0; i < vertices.Length; i++) {
            var vertex = vertices[i];
            var height = (float) Math.Round(vertex.y, 5);
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

    private void GenerateTrianglesByHeight() {
        trianglesByHeights = new Dictionary<float, LinkedList<int>>();
        var triangles = baseMesh.triangles;
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
    }

    private void GetCutTriangles(LinkedList<int> beforeCut, LinkedList<int> afterCut,
        out LinkedList<int> cutTriangles) {
        var triangles = baseMesh.triangles;
        cutTriangles = new LinkedList<int>();
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

            if (beforeCount > 0 && afterCount > 0 && (beforeCount + afterCount) == 3) {
                //DrawTriangle(vertices[v1], vertices[v2], vertices[v3]);
                cutTriangles.AddLast(v1);
                cutTriangles.AddLast(v2);
                cutTriangles.AddLast(v3);
            }
        }
    }

    private void CutMesh() {
        if (verticesByHeights == null) {
            SetupMeshData();
        }
        
        if (percent == 1) {
            Mesh mesh = new Mesh();
            mesh.vertices = (Vector3[]) baseMesh.vertices.Clone();
            mesh.triangles = (int[]) baseMesh.triangles.Clone();
            mesh.normals = (Vector3[]) baseMesh.normals.Clone();
            mesh.tangents = (Vector4[]) baseMesh.tangents.Clone();
            mesh.uv = (Vector2[]) baseMesh.uv.Clone();
            cutMesh = mesh;
            _meshFilter.mesh = mesh;
            return;
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
        
        Dictionary<int, int> edges;

        Dictionary<int, VertexNode<int>> sortedBeforeVertices;
        Dictionary<int, VertexNode<int>> sortedAfterVertices;

        int[] beforeVerticesArray = new int[0];
        int[] afterVerticesArray = new int[0];

        LinkedList<int> cutTriangles;
        LinkedList<int> topTriangles;
        LinkedList<int> loopTriangles;
        LinkedList<int> previousTriangles;

        // Get the triangles we bisect at our cut height
        GetCutTriangles(beforeCut, afterCut, out cutTriangles);

        Debug.Log("There are " + (cutTriangles.Count / 3) + " cut triangles");

        // Get the vertexes
        var vertices = baseMesh.vertices;
        int previousVertexCount = vertices.Length;

        GenerateSortedBeforeVerticesAndEdges(cutTriangles, beforeCut, afterCut, out sortedBeforeVertices,
            out sortedAfterVertices, out edges);
        if (sortedBeforeVertices.Count > 0)
            ConvertVertexLoopToArray(sortedBeforeVertices, out beforeVerticesArray);
        if (sortedAfterVertices.Count > 0)
            ConvertVertexLoopToArray(sortedAfterVertices, out afterVerticesArray);
        Debug.Log("Found " + beforeVerticesArray.Length + " vertices in before loop");
        Debug.Log("Found " + afterVerticesArray.Length + " vertices in after loop");
        Debug.Log("Found " + edges.Count + " edges");

        GenerateLoopVertices(afterVerticesArray, afterCut.Count, edges, out loopVertices);

        // Generate all the triangles
        GenerateTopTriangles(previousVertexCount, loopVertices.Length, out topTriangles);
        GenerateLoopTriangles(previousVertexCount, afterVerticesArray, edges, out loopTriangles);
        GetPreviousTriangles(out previousTriangles);

        // Normalize previous verts, loop verts
        // Normalize previous triangles, loop triangles, top triangles
        Vector3[] meshVertices = new Vector3[previousVertexCount + loopVertices.Length];
        int[] meshTriangles = new int[previousTriangles.Count + loopTriangles.Count + topTriangles.Count];

        LinkedListNode<int> t1;
        LinkedListNode<int> t2;
        LinkedListNode<int> t3;
        Dictionary<int, int> vertexMap = new Dictionary<int, int>();;
        int vertexIndex = 0;
        int triangleIndex = 0;
        
        // Normalize previous triangles
        if (previousTriangles.Count > 0) {
            t1 = previousTriangles.First;
            t2 = t1.Next;
            t3 = t2.Next;
            for (var t = 3; t < previousTriangles.Count; t += 3) {
                if (!vertexMap.ContainsKey(t1.Value))
                    vertexMap.Add(t1.Value, vertexIndex++);
                if (!vertexMap.ContainsKey(t2.Value))
                    vertexMap.Add(t2.Value, vertexIndex++);
                if (!vertexMap.ContainsKey(t3.Value))
                    vertexMap.Add(t3.Value, vertexIndex++);

                meshTriangles[triangleIndex++] = vertexMap[t1.Value];
                meshTriangles[triangleIndex++] = vertexMap[t2.Value];
                meshTriangles[triangleIndex++] = vertexMap[t3.Value];

                t1 = t3.Next;
                t2 = t1.Next;
                t3 = t2.Next;
            }
        }

        // Normalize loop triangles
        if (loopTriangles.Count > 0) {
            t1 = loopTriangles.First;
            t2 = t1.Next;
            t3 = t2.Next;
            for (var t = 3; t < loopTriangles.Count; t += 3) {
                if (!vertexMap.ContainsKey(t1.Value))
                    vertexMap.Add(t1.Value, vertexIndex++);
                if (!vertexMap.ContainsKey(t2.Value))
                    vertexMap.Add(t2.Value, vertexIndex++);
                if (!vertexMap.ContainsKey(t3.Value))
                    vertexMap.Add(t3.Value, vertexIndex++);

                meshTriangles[triangleIndex++] = vertexMap[t1.Value];
                meshTriangles[triangleIndex++] = vertexMap[t2.Value];
                meshTriangles[triangleIndex++] = vertexMap[t3.Value];

                t1 = t3.Next;
                t2 = t1.Next;
                t3 = t2.Next;
            }
        }

        // Normalize top triangles
        if (topTriangles.Count > 0) {
            t1 = topTriangles.First;
            t2 = t1.Next;
            t3 = t2.Next;
            for (var t = 3; t < topTriangles.Count; t += 3) {
                if (!vertexMap.ContainsKey(t1.Value))
                    vertexMap.Add(t1.Value, vertexIndex++);
                if (!vertexMap.ContainsKey(t2.Value))
                    vertexMap.Add(t2.Value, vertexIndex++);
                if (!vertexMap.ContainsKey(t3.Value))
                    vertexMap.Add(t3.Value, vertexIndex++);

                meshTriangles[triangleIndex++] = vertexMap[t1.Value];
                meshTriangles[triangleIndex++] = vertexMap[t2.Value];
                meshTriangles[triangleIndex++] = vertexMap[t3.Value];

                t1 = t3.Next;
                t2 = t1.Next;
                t3 = t2.Next;
            }
        }

        // Normalize all verts

        for (var i = 0; i < vertices.Length; i++) {
            int v;
            if (vertexMap.TryGetValue(i, out v)) {
                meshVertices[v] = vertices[i];
            }
        }

        for (var i = 0; i < loopVertices.Length; i++) {
            int v;
            if (vertexMap.TryGetValue(previousVertexCount + i, out v)) {
                meshVertices[v] = loopVertices[i];
            }
        }

        // Generate normals from vertices
        var meshNormals = new Vector3[meshVertices.Length];
        for (var i = 0; i < meshNormals.Length; i++) {
            var vertex = meshVertices[i];
            var normal = new Vector3(vertex.x, vertex.y, vertex.z);
            normal.Normalize();
            meshNormals[i] = normal;
        }
        
        // Generate UVs from vertices
        var meshUVs = new Vector2[meshVertices.Length];
        for (var i = 0; i < meshUVs.Length; i++) {
            var vertex = meshVertices[i];
            var uv = new Vector2((vertex.y - bottom) / height, i / (float) meshUVs.Length);
            meshUVs[i] = uv;
        }

        cutMesh = new Mesh();
        cutMesh.vertices = meshVertices;
        cutMesh.triangles = meshTriangles;
        cutMesh.uv = meshUVs;
        cutMesh.RecalculateNormals();
        cutMesh.RecalculateTangents();
        cutMesh.RecalculateBounds();
        
        _meshFilter.mesh = cutMesh;
        
    }
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

    private void ConvertVertexLoopToArray(Dictionary<int, VertexNode<int>> loopVertices, out int[] verticesArray) {
        verticesArray = new int[loopVertices.Count];
        int firstIndex = loopVertices.Keys.Min();
        VertexNode<int> vertex = loopVertices[firstIndex];

        Debug.Log("Convert " + loopVertices.Count + " verts.");
        for (int arrayIndex = 0; arrayIndex < loopVertices.Count; arrayIndex++) {
            verticesArray[arrayIndex] = vertex.Value;
            vertex = vertex.Next;
        }
    }

    private VertexNode<int> ReverseList(VertexNode<int> node, bool next = true) {
        if (next) {
            if (node.Next == null) {
                node.Next = node.Last;
                node.Last = null;
                return node;
            } else {
                var nextNode = ReverseList(node.Next, next);
                node.Next = node.Last;
                node.Last = nextNode;
                return node;
            }
        } else {
            if (node.Last == null) {
                node.Last = node.Next;
                node.Next = null;
                return node;
            } else {
                var lastNode = ReverseList(node.Last, next);
                node.Last = node.Next;
                node.Next = lastNode;
                return node;
            }
        }
    }
    private void GenerateSortedBeforeVerticesAndEdges(LinkedList<int> cutTriangles, LinkedList<int> beforeCut,
        LinkedList<int> afterCut, out Dictionary<int, VertexNode<int>> sortedBeforeVertices,
        out Dictionary<int, VertexNode<int>> sortedAfterVertices, out Dictionary<int, int> edges) {
        sortedBeforeVertices = new Dictionary<int, VertexNode<int>>();
        sortedAfterVertices = new Dictionary<int, VertexNode<int>>();
        edges = new Dictionary<int, int>();
        var vertices = baseMesh.vertices;

        var v1 = cutTriangles.First;
        var v2 = v1.Next;
        var v3 = v2.Next;
        int triangleCount = 0;
        for (var t = 0; t < cutTriangles.Count; t += 3) {
            var edge1 = CalculateDistance(v1.Value, v2.Value);
            var edge2 = CalculateDistance(v2.Value, v3.Value);
            var edge3 = CalculateDistance(v3.Value, v1.Value);
            
            if ((edge1 == -1 && edge2 == -1) || (edge2 == -1 && edge3 == -1) || (edge1 == -1 && edge3 == -1)) {
                Debug.Log("Too many links!");
            }
            
            int x1;
            int x2;
            int x3;

            if (edge1 == -1) {
                // Link between v1 && v2
                if (edge1 == edge3) {
                    if ((edges.ContainsKey(v1.Value) && edges[v1.Value] == v3.Value) || (edges.ContainsKey(v3.Value) && edges[v3.Value] == v1.Value)) {
                        x1 = v1.Value;
                        x2 = v2.Value;
                    } else {
                        x1 = v2.Value;
                        x2 = v1.Value;
                    }
                } else if (edge2 < edge3) {
                    // v2 → v3 is shortest
                    x1 = v2.Value;
                    x2 = v1.Value;
                } else {
                    // v1 → v3 is shortest
                    x1 = v1.Value;
                    x2 = v2.Value;
                }
                x3 = v3.Value;
            } else if (edge2 == -1) {
                // Link between v2 && v3
                if (edge1 == edge3) {
                    if ((edges.ContainsKey(v2.Value) && edges[v2.Value] == v1.Value) || (edges.ContainsKey(v1.Value) && edges[v1.Value] == v2.Value)) {
                        x1 = v3.Value;
                        x2 = v2.Value;
                    } else {
                        x1 = v2.Value;
                        x2 = v3.Value;
                    }
                } else if (edge1 < edge3) {
                    // v1 → v2 is shortest
                    x1 = v2.Value;
                    x2 = v3.Value;
                } else {
                    // v3 → v1 is shortest
                    x1 = v3.Value;
                    x2 = v2.Value;
                }
                x3 = v1.Value;
            } else {
                // Link between v3 && v1
                if (edge1 == edge2) {
                    if ((edges.ContainsKey(v1.Value) && edges[v1.Value] == v2.Value) || (edges.ContainsKey(v2.Value) && edges[v2.Value] == v1.Value)) {
                        x1 = v3.Value;
                        x2 = v1.Value;
                    } else {
                        x1 = v1.Value;
                        x2 = v3.Value;
                    }
                } else if (edge1 < edge2) {
                    // v1 → v2 is shortest
                    x1 = v1.Value;
                    x2 = v3.Value;
                } else {
                    // v2 → v3 is shortest
                    x1 = v3.Value;
                    x2 = v1.Value;
                }
                x3 = v2.Value;
            }
            
            
            //Gizmos.color = Color.red;
            //DrawLine(vertices[x2], vertices[x3]);

            
            VertexNode<int> vertex1 = null;
            VertexNode<int> vertex2 = null;
            if (beforeCut.Contains(x1)) {
                // Add to before loop
                if (!sortedBeforeVertices.TryGetValue(x1, out vertex1)) {
                    vertex1 = new VertexNode<int>(x1);
                    sortedBeforeVertices.Add(x1, vertex1);
                }

                if (!sortedBeforeVertices.TryGetValue(x2, out vertex2)) {
                    vertex2 = new VertexNode<int>(x2);
                    sortedBeforeVertices.Add(x2, vertex2);
                }
                //Gizmos.color = Color.blue;
                //DrawLine(vertices[x1], vertices[x2]);
            } else if (afterCut.Contains(x1)) {
                // Add to after loop
                if (!sortedAfterVertices.TryGetValue(x1, out vertex1)) {
                    vertex1 = new VertexNode<int>(x1);
                    sortedAfterVertices.Add(x1, vertex1);
                }

                if (!sortedAfterVertices.TryGetValue(x2, out vertex2)) {
                    vertex2 = new VertexNode<int>(x2);
                    sortedAfterVertices.Add(x2, vertex2);
                }
                
                //Gizmos.color = Color.red;
                //DrawLine(vertices[x1], vertices[x2]);

                // Add to edges?
                if (!edges.ContainsKey(x1) && !edges.ContainsKey(x3)) {
                    edges.Add(x1, x3);
                    Gizmos.color = Color.white;
                    //DrawTriangle(vertices[x1], vertices[x2], vertices[x3]);
                    triangleCount++;
                    //Gizmos.color = Color.green;
                    //DrawLine(vertices[x1], vertices[x3]);
                } else {
                    Debug.Log("Miss edge " + x1 + " " + x2);
                }
            }
            
            if (vertex1.Next != null) {
                vertex1.Last = ReverseList(vertex1.Next);
                vertex1.Next = null;
            }

            if (vertex2.Last != null) {
                vertex2.Next = ReverseList(vertex2.Last, false);
                vertex2.Last = null;
            }

            vertex1.Next = vertex2;
            vertex2.Last = vertex1;

            if (v3.Next != null) {
                v1 = v3.Next;
                v2 = v1.Next;
                v3 = v2.Next;
            }
        }

        if (sortedBeforeVertices.Count == 0) {
            int x1 = beforeCut.First.Value;
            var vertex1 = new VertexNode<int>(x1);
            sortedBeforeVertices.Add(x1, vertex1);
        }
        
        // Add special override for beforeCut or afterCut having one element
        //Gizmos.color = Color.green;
        if (afterCut.Count == 1) {
            edges.Clear();
            var x1 = afterCut.First.Value;
            foreach (int x3 in beforeCut) {
                edges.Add(x1, x3);
                //DrawLine(vertices[x1], vertices[x3]);
            }
        } else if (beforeCut.Count == 1) {
            edges.Clear();
            var x3 = beforeCut.First.Value;
            foreach (int x1 in afterCut) {
                edges.Add(x1, x3);
                //DrawLine(vertices[x1], vertices[x3]);
            }
        }
        Debug.Log("Got " + triangleCount + " triangles");
        
    }

    private void GenerateLoopVertices(int[] sortedVertices, int afterCount, Dictionary<int, int> edges, out Vector3[] loopVertices) {
        loopVertices = new Vector3[edges.Count];
        var vertices = baseMesh.vertices;
    
        float mult = (cutHeight - beforeCutHeight) / (afterCutHeight - beforeCutHeight);

        for (int i = 0; i < edges.Count; i++) {
            var v1 = sortedVertices[i];
            var v2 = edges[v1];
            var vertex1 = vertices[v2];
            var vertex2 = vertices[v1];
            
            var xp = (vertex2.x - vertex1.x) * mult;
            var yp = (vertex2.y - vertex1.y) * mult;
            var zp = (vertex2.z - vertex1.z) * mult;
            
            var p = new Vector3(xp + vertex1.x, yp + vertex1.y, zp + vertex1.z);
            loopVertices[i] = p;
        }
        //Gizmos.color = Color.white;
    }

    private void GenerateTopTriangles(int startIndex, int vertexCount, out LinkedList<int> topTriangles) {
        topTriangles = new LinkedList<int>();

        if (vertexCount < 3) {
            return;
        }

        int left = startIndex;
        int right = startIndex + vertexCount - 1;
        bool makeLeft = true;
        
        topTriangles.AddLast(right);
        topTriangles.AddLast(left++);
        topTriangles.AddLast(left);
        
        if (vertexCount == 3)
            return;

        while (left + 2 < right) {
            topTriangles.AddLast(right);
            topTriangles.AddLast(left);
            

            if (makeLeft) {
                topTriangles.AddLast(++left);
            } else {
                topTriangles.AddLast(--right);
            }

            makeLeft = !makeLeft;
        }
        
        topTriangles.AddLast(right);
        topTriangles.AddLast(left++);
        topTriangles.AddLast(left);
    }

    private void GenerateLoopTriangles(int startIndex, int[] afterVertices, Dictionary<int, int> edges, out LinkedList<int> loopTriangles) {
        loopTriangles = new LinkedList<int>();
        for (var i = 0; i < edges.Count; i++) {
            var a1 = afterVertices[i];
            var v1 = edges[a1];
            var v2 = startIndex + i;
            int v11;
            int v21;

            if (i + 1 == edges.Count) {
                var a11 = afterVertices[0];
                v11 = edges[a11];
                v21 = startIndex;
            } else {
                var a11 = afterVertices[i + 1];
                v11 = edges[a11];
                v21 = startIndex + i + 1;
            }

            loopTriangles.AddLast(v1);
            loopTriangles.AddLast(v11);
            loopTriangles.AddLast(v21);
            
            loopTriangles.AddLast(v21);
            loopTriangles.AddLast(v2);
            loopTriangles.AddLast(v1);
        }
    }

    private void GetPreviousTriangles(out LinkedList<int> previousTriangles) {
        previousTriangles = new LinkedList<int>();

        foreach (KeyValuePair<float, LinkedList<int>> heightEntry in trianglesByHeights) {
            if (heightEntry.Key < cutHeight && heightEntry.Key != beforeCutHeight) {
                foreach (int vertex in heightEntry.Value) {
                    previousTriangles.AddLast(vertex);
                }
            }
        }
    }

    private int GetPreviousVertices(float cutHeight) {
        int count = 0;

        foreach (KeyValuePair<float, LinkedList<int>> heightEntry in verticesByHeights) {
            if (heightEntry.Key < cutHeight) {
                count += heightEntry.Value.Count;
            }
        }

        return count;
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

    private float CalculateDistance(int x, int y) {
        var vertices = baseMesh.vertices;
        var xd = Mathf.Pow(vertices[y].x - vertices[x].x, 2);
        var yd = Mathf.Pow(vertices[y].y - vertices[x].y, 2);
        var zd = Mathf.Pow(vertices[y].z - vertices[x].z, 2);
        
        if (yd < 0.0000001) {
            return -1;
        }

        return (float) Math.Round(Mathf.Sqrt(xd + yd + zd), 6);
    }

    /*private void OnDrawGizmos() {
        Start();
        CutMesh();
    }*/
}