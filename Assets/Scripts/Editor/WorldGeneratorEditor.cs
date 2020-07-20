using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldGenerator))]
class WorldGeneratorEditor:Editor
{
    public override void OnInspectorGUI() {
		DrawDefaultInspector();

		WorldGenerator worldGenerator = (WorldGenerator)target;
		if (GUILayout.Button("Generate")) {
			worldGenerator.Generate();
		}
    }

}