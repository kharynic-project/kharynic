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
		    var objDef = await Resources.LoadText(path);
		    var objMesh = new ObjMesh(objDef);
			
			return new Mesh
			{
				vertices = objMesh.GeometryVertices.ToArray(),
				uv = objMesh.TexVertices.ToArray(),
				normals = objMesh.VertexNormals.ToArray(),
				triangles = objMesh.GeometryTriangles.SelectMany(t => t).ToArray()
			};
	    }
    }
}