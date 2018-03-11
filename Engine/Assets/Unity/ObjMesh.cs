using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Kharynic.Engine.Unity
{
    class ObjMesh
    {
        public List<Vector3> GeometryVertices { get; } = new List<Vector3>(); 
        public List<Vector2> TextureVertices { get; } = new List<Vector2>(); 
        public List<Vector3> VertexNormals { get; } = new List<Vector3>();
        public List<int?> VertexComponentIds { get; } = new List<int?>();
		
        private static Vector3 Parse3dCoords(string[] line)
        {
            var vector = new Vector3();
            for (var i = 0; i < 3; i++)
                vector[i] = float.Parse(line[i + 1], CultureInfo.InvariantCulture);
            return vector;
        }

        private static Vector2 ParseUvCoords(string[] line)
        {
            var vector = new Vector2();
            for (var i = 0; i < 2; i++)
                vector[i] = float.Parse(line[i + 1], CultureInfo.InvariantCulture);
            return vector;
        }
        
        private ObjMesh() {}

        public static async Task<ObjMesh> Load (string definition)
        {
            var that = new ObjMesh();
            var lines = definition
                .Split('\n')
                .Where(l => l.Length > 0 && l[0] != '#')
                .Select(l => l.Split(' '));
            foreach (var line in lines)
            {
                var lineType = line[0];
                switch (lineType)
                {
                    case "mtllib": // material library: mtllib joint.mtl
                        // ignore
                        break;
                    case "o": // object: o Cylinder006
                        // ignore
                        break;
                    case "v": // geometric vertex: v 3.2e-3 -1.4e-10 -5.8e-2
                        that.GeometryVertices.Add(Parse3dCoords(line));
                        break;
                    case "vt": // texture vertex: vt 0.2 0.7
                        that.TextureVertices.Add(ParseUvCoords(line));
                        break;
                    case "vn": // vertex normal: vn 0.9 -5.3e-9 -6.3e-2
                        that.VertexNormals.Add(Parse3dCoords(line));
                        break;
                    case "g": // group name: g Cylinder006_CA_MISC
                        // ignore
                        break;
                    case "usemtl": // material name: usemtl CA_MISC
                        // ignore for now
                        break;
                    case "s": // smoothing group: s 1
                        // ignore for now
                        break;
                    case "f": // face: f 18/9/26 25/7/61 17/5/19
                        // entry: geometric vertex id / texture vertex id / vertex normal id
                        var indices = line
                            .Skip(1)
                            .SelectMany(entry => entry.Split('/'))
                            .Select(s => s.Length > 0 ? (int.Parse(s) - 1) : (int?)null)
                            .ToArray();
                        Debug.Assert(indices.Length == 9);
                        that.VertexComponentIds.AddRange(indices);
                        break;
                    default:
                        throw new ArgumentException($"unsupported obj format entry: {line}", nameof(definition));
                }
            }

            Debug.Assert(that.VertexComponentIds.Count % 9 == 0); // three vertices for geometry, tex and normals
            return that;
        }
    }
}