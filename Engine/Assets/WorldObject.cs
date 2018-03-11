using System;
using System.IO;
using System.Threading.Tasks;
using Kharynic.Engine.Scripting;
using Kharynic.Engine.Unity;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kharynic.Engine
{
    public class WorldObject
    {
        private readonly GameObject _gameObject;
        
        [Scriptable]
        public string Name
        {
            get { return _gameObject.name; }
            set { _gameObject.name = value; }
        }

        public WorldObject(string meshPath)
        {
            _gameObject = new GameObject();
            Name = Path.GetFileNameWithoutExtension(meshPath) ?? "object";
            ConfigureRendering();
            LoadMesh(meshPath);
        }
        
        private static readonly Lazy<Material> DefaultMaterial = new Lazy<Material>(() =>
        {
            var shader = Shader.Find("Standard");
            return new Material(shader);
        });

        private void ConfigureRendering()
        {
            var meshRenderer = _gameObject.AddComponent<MeshRenderer>();
            meshRenderer.lightProbeUsage = LightProbeUsage.Off;
            meshRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            meshRenderer.shadowCastingMode = ShadowCastingMode.On;
            meshRenderer.receiveShadows = true;
            meshRenderer.allowOcclusionWhenDynamic = true;
            meshRenderer.material = DefaultMaterial.Value;
        }

        // TODO: support for constructors in scripting
        [Scriptable]
        public static WorldObject Create(string meshPath)
        {
            return new WorldObject(meshPath);
        }

        [Scriptable]
        public void SetPosition(float x, float y, float z)
        {
            _gameObject.transform.localPosition = new Vector3(x,y,z);
        }

        [Scriptable]
        public void SetRotation(float x, float y, float z)
        {
            _gameObject.transform.eulerAngles = new Vector3(x,y,z);
        }

        private async void LoadMesh(string meshPath)
        {
            var name = Path.GetFileNameWithoutExtension(meshPath);
            Debug.Log($"loading mesh {name}: started");
            _gameObject.transform.localScale = new Vector3(-1, 1, 1);
            var meshFilter = _gameObject.AddComponent<MeshFilter>();
            var startTime = DateTime.Now;
            var objDef = await Resources.LoadText(meshPath);
            Debug.Log($"loading mesh {name}: file loaded");
            var objMesh = await ObjMesh.Load(objDef);
            Debug.Log($"loading mesh {name}: file parsed");
            var unityMeshData = await UnityMeshData.Load(objMesh);
            Debug.Log($"loading mesh {name}: mesh processed");
            meshFilter.mesh = unityMeshData.ToUnityMesh();

            var time = (DateTime.Now - startTime).TotalSeconds;
            Debug.Log($"loading mesh {name}: finished after {time}s ({objDef.Length} bytes, " +
                      $"{unityMeshData.TriangleCount/3} tris, {unityMeshData.VertexCount} verts)");
        }
    }
}