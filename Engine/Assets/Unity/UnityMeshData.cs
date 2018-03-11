using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Kharynic.Engine.Unity
{
    internal class UnityMeshData
    {
        private readonly List<Vector3> _geometryVertices = new List<Vector3>();
        private readonly List<Vector2> _textureVertices = new List<Vector2>();
        private readonly List<Vector3> _normalVertices = new List<Vector3>();
        private readonly List<int> _triangles = new List<int>();
        public int TriangleCount => _triangles.Count / 3;
        public int VertexCount => _geometryVertices.Count;

        public static async Task<UnityMeshData> Load(ObjMesh objMesh)
        {
            var @this = new UnityMeshData();
            for (var i = 0; i < objMesh.VertexComponentIds.Count; i+=3)
            {
                await AsyncExtensions.WaitMoment();
                @this.LoadVertex(objMesh, i);
            }
            return @this;
        }

        private void LoadVertex(ObjMesh objMesh, int i)
        {
            var geomVertexId = objMesh.VertexComponentIds[i + 0];
            var texVertexId = objMesh.VertexComponentIds[i + 1];
            var vertexNormalId = objMesh.VertexComponentIds[i + 2];

            if (geomVertexId != null)
            {
                Debug.Assert(0 <= geomVertexId && geomVertexId < objMesh.GeometryVertices.Count);
                var geomVertex = objMesh.GeometryVertices[geomVertexId.Value];
                _geometryVertices.Add(geomVertex);
            }

            if (texVertexId != null)
            {
                Debug.Assert(0 <= texVertexId && texVertexId < objMesh.TextureVertices.Count);
                var texVertex = objMesh.TextureVertices[texVertexId.Value];
                _textureVertices.Add(texVertex);
            }

            if (vertexNormalId != null)
            {
                Debug.Assert(0 <= vertexNormalId && vertexNormalId < objMesh.VertexNormals.Count);
                var vertexNormal = objMesh.VertexNormals[vertexNormalId.Value];
                _normalVertices.Add(vertexNormal);
            }

            _triangles.Add(i / 3);
        }

        public Mesh ToUnityMesh()
        {
            var mesh = new Mesh
            {
                vertices = _geometryVertices.ToArray(),
                uv = _textureVertices.ToArray(),
                normals = _normalVertices.ToArray(),
                triangles = _triangles.ToArray()
            };
            mesh.RecalculateBounds();
            mesh.RecalculateTangents();
            return mesh;
        }
    }
}