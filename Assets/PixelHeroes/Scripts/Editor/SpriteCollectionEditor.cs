using Assets.PixelHeroes.Scripts.CollectionScripts;
using UnityEditor;
using UnityEngine;

namespace Assets.PixelHeroes.Scripts.Editor
{
    /// <summary>
    /// Adds "Refresh" button to SpriteCollection script.
    /// </summary>
    [CustomEditor(typeof(SpriteCollection))]
    public class SpriteCollectionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var collection = (SpriteCollection) target;

            if (GUILayout.Button("Refresh"))
            {
                collection.Refresh();
            }

            if (GUILayout.Button("Apply Palette"))
            {
                collection.ApplyPalette();
            }
        }
    }
}