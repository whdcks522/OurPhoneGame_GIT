using Assets.PixelHeroes.Scripts.EditorScripts;
using UnityEditor;
using UnityEngine;

namespace Assets.PixelHeroes.Scripts.Editor
{
    /// <summary>
    /// Adds "Refresh" button to CharacterEditor script.
    /// </summary>
    [CustomEditor(typeof(CharacterEditor))]
    public class CharacterEditorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Refresh"))
            {
                ((CharacterEditor) target).SpriteCollection.Refresh();
            }
        }
    }
}