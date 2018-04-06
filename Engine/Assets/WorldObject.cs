using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Kharynic.Engine.Scripting;
using Kharynic.Engine.Unity;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Kharynic.Engine
{
    public class WorldObject
    {
        private readonly GameObject _gameObject;
        private static readonly Dictionary<string, Mesh> _meshCache = new Dictionary<string, Mesh>();

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
        public void Remove()
        {
            Object.Destroy(_gameObject);
        }

        [Scriptable]
        public void SetRotation(float x, float y, float z)
        {
            _gameObject.transform.eulerAngles = new Vector3(x,y,z);
        }

        private static bool _loadingInProgress = false;

        private async void LoadMesh(string meshPath)
        {
            _gameObject.transform.localScale = new Vector3(-1, 1, 1);
            var meshFilter = _gameObject.AddComponent<MeshFilter>();
            Mesh mesh;
            while(_loadingInProgress && !_meshCache.ContainsKey(meshPath))
                await new WaitForSecondsRealtime(0.1f).WaitAsync();
            if (_loadingInProgress && !_meshCache.ContainsKey(meshPath))
                throw new Exception("WaitWhile failed");
            if (!_meshCache.ContainsKey(meshPath))
            {
                Debug.Log($"LoadMesh({meshPath})");
                _loadingInProgress = true;
                var startTime = DateTime.Now;
                var objDef = await Resources.LoadText(meshPath);
                var objMesh = await ObjMesh.Load(objDef);
                var unityMeshData = await UnityMeshData.Load(objMesh);
                mesh = unityMeshData.ToUnityMesh();
                var time = (DateTime.Now - startTime).TotalSeconds;
                _meshCache[meshPath] = mesh;
                _loadingInProgress = false;
                Debug.Log($"LoadMesh({meshPath}): finished after {time}s ({objDef.Length} bytes, " +
                          $"{unityMeshData.TriangleCount / 3} tris, {unityMeshData.VertexCount} verts)");
            }
            else
                mesh = _meshCache[meshPath];
            meshFilter.sharedMesh = mesh;
        }
    }
}