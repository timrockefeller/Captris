using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldManager))]
class WorldManagerEditor:Editor
{
    public override void OnInspectorGUI() {
		DrawDefaultInspector();

		WorldManager worldManager = (WorldManager)target;
		if (GUILayout.Button("Generate")) {
			worldManager.Generate();
		}
    }

}