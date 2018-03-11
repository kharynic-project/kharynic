using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kharynic.Engine.Unity
{
	public class MeshLoader
    {
        public static async Task<GameObject> LoadObject(string path)
        {
	        var gameObject = new GameObject
	        {
		        name = Path.GetFileNameWithoutExtension(path) ?? "object"
	        };
	        var meshFilter = gameObject.AddComponent<MeshFilter>();
	        meshFilter.mesh = await LoadMesh(path);
			var meshRenderer = gameObject.AddComponent<MeshRenderer>();
			meshRenderer.lightProbeUsage = LightProbeUsage.Off;
			meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
			meshRenderer.shadowCastingMode = ShadowCastingMode.On;
			meshRenderer.receiveShadows = true;
			meshRenderer.allowOcclusionWhenDynamic = true;
			return gameObject;
        }

	    public static async Task<Mesh> LoadMesh(string path)
	    {
		    var startTime = DateTime.Now;
		    var objDef = await Resources.LoadText(path);
		    var objMesh = await ObjMesh.Load(objDef);

		    var vertices = new Dictionary<Tuple<int, int, int>, int>();
		    
		    var geometryVertices = new List<Vector3>();
		    var textureVertices = new List<Vector2>();
		    var normalVertices = new List<Vector3>();
		    var triangles = new List<int>();
		    
		    for (var i = 0; i < objMesh.VertexComponentIds.Count; i+=3)
		    {
			    var geomVertexId = objMesh.VertexComponentIds[i + 0];
			    var texVertexId = objMesh.VertexComponentIds[i + 1];
			    var vertexNormalId = objMesh.VertexComponentIds[i + 2];
			    
			    Debug.Assert(0 <= geomVertexId && geomVertexId < objMesh.GeometryVertices.Count);
			    Debug.Assert(0 <= texVertexId && texVertexId < objMesh.TextureVertices.Count);
			    Debug.Assert(0 <= vertexNormalId && vertexNormalId < objMesh.VertexNormals.Count);
			    
			    var geomVertex = objMesh.GeometryVertices[geomVertexId];
			    var texVertex = objMesh.TextureVertices[texVertexId];
			    var vertexNormal = objMesh.VertexNormals[vertexNormalId];
			    
			    geometryVertices.Add(geomVertex);
			    textureVertices.Add(texVertex);
			    normalVertices.Add(vertexNormal);
			    triangles.Add(i / 3);
		    }
			
			var mesh = new Mesh
			{
				vertices = geometryVertices.ToArray(),
				uv = textureVertices.ToArray(),
				normals = normalVertices.ToArray(),
				triangles = triangles.ToArray()
			};

		    var name = Path.GetFileNameWithoutExtension(path);
		    var time = (DateTime.Now - startTime).TotalSeconds;
		    Debug.Log($"mesh {name} loaded in {time}s ({triangles.Count/3} tris, {geometryVertices.Count} verts)");
		    return mesh;
	    }
    }
}