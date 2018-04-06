using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Kharynic.Engine.Scripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Kharynic.Engine.Unity
{
    public class WorldLoader
    {
        private static readonly DataContractJsonSerializer Serializer = new DataContractJsonSerializer(typeof(World));
        
        [Serializable]
        public class Vector3Dec
        {
            public decimal x;
            public decimal y;
            public decimal z;

            public Vector3Dec(Vector3 other)
            {
                x = Math.Round(new decimal(other.x), 2, MidpointRounding.ToEven);
                y = Math.Round(new decimal(other.y), 2, MidpointRounding.ToEven);
                z = Math.Round(new decimal(other.z), 2, MidpointRounding.ToEven);
            }
        }
        
        public class World
        {
            public class Object
            {
                public string Mesh;
                public Vector3Dec Position;
                public Vector3Dec Rotation;
            }
            public List<Object> Objects = new List<Object>();
        }
        
        [Scriptable]
        public static string Write()
        {
            var loadedMeshes = Object.FindObjectsOfType<MeshFilter>();
            var world = new World();
            foreach (var mesh in loadedMeshes)
            {
                var name = mesh.name;
                var nameEnd = name.LastIndexOf(" (", StringComparison.InvariantCulture);
                if (nameEnd > 0)
                    name = name.Substring(0, nameEnd);
                var transform = mesh.gameObject.transform;
                world.Objects.Add(new World.Object
                {
                    Mesh = name,
                    Position = new Vector3Dec(transform.position),
                    Rotation = new Vector3Dec(transform.eulerAngles)
                });
            }
            using (var stream = new MemoryStream())
            {
                Serializer.WriteObject(stream, world);
                return Encoding.UTF8.GetString(stream.ToArray());
            } 
        }

        [Scriptable]
        public static async void Load(string path)
        {
            var json = await Resources.LoadText(path);
            Debug.Log($"{nameof(WorldLoader)}.{nameof(Load)}({path}): {json.Length}b loaded");
            var world = (World) Serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(json)));
            Debug.Log($"{nameof(WorldLoader)}.{nameof(Load)}({path}): file deserialized");
            foreach (var @object in world.Objects)
            {
                var meshPath = $"resources/meshes/{@object.Mesh}.obj";
                var wo = WorldObject.Create(meshPath);
                wo.SetPosition((float) @object.Position.x, (float) @object.Position.y, (float) @object.Position.z);
                wo.SetRotation((float) @object.Rotation.x, (float) @object.Rotation.y, (float) @object.Rotation.z);
            }
            Debug.Log($"{nameof(WorldLoader)}.{nameof(Load)}({path}): {world.Objects.Count} objects loaded");
        }
    }
}
