using System;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;

namespace Plarium.Tools.NoisePresentation
{
    public class CreateSimpleCubeMesh : EditorWindow
    {
        [MenuItem("Tools/CreateSimpleCubeMesh")]
        private static void Open()
        {
            var w = GetWindow<CreateSimpleCubeMesh>();
            w.Show();
        }

        /*private static Vector3[] _vertices =
        {
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1)
        };

        private static Vector2[] _uv0 =
        {
            new Vector3(0, 0),
            new Vector3(1, 0),
            new Vector3(0, 1),
            new Vector3(1, 1),
            new Vector3(0, 0),
            new Vector3(1, 0),
            new Vector3(0, 1),
            new Vector3(1, 1)
        };

        private static int[] _triangles =
        {
            0, 1, 2,
            3, 2, 1,
            
            0, 2, 4,
            6, 4, 2,
            
            0, 4, 1,
            1, 4, 5,
            
            1, 7, 3,
            1, 5, 7,
            
            2, 3, 7,
            7, 6, 2,
            
            4, 6, 7,
            7, 5, 4,
        };*/
        
        private static Vector3[] _vertices =
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),

            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1)
        };

        private static int[] _triangles =
        {
            0, 1, 2,
            3, 2, 1,
            
            0, 2, 4,
            6, 4, 2,
            
            0, 4, 1,
            1, 4, 5,
            
            1, 7, 3,
            1, 5, 7,
            
            2, 3, 7,
            7, 6, 2,
            
            4, 6, 7,
            7, 5, 4,
        };
        
        private void OnGUI()
        {
            if (GUILayout.Button("Create"))
            {
                Create();
            }
        }

        private void Create()
        {
            var mesh = new Mesh();
            mesh.vertices = _vertices;
            mesh.triangles = _triangles;

            var path = "Assets/Resources/CubeSimple.mesh";
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(mesh, path);
            AssetDatabase.Refresh();
        }
    }
}