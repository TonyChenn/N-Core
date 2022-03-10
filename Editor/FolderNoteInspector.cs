using UnityEditor;
using UnityEngine;

namespace NCore.Editor
{
    public class Note
    {
        AssetImporter assetImporter;

        public string guid;
        public string info;

        public Note(string path)
        {
            assetImporter = AssetImporter.GetAtPath(path);
            guid = AssetDatabase.AssetPathToGUID(path);
            info = assetImporter.userData;
        }

        public void Save()
        {
            assetImporter.userData = info;
            assetImporter.SaveAndReimport();
        }
    }

    [CustomEditor(typeof(DefaultAsset))]
    public class FolderNoteInspector : UnityEditor.Editor
    {
        Note note;
        GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.Height(100) };
        public override void OnInspectorGUI()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            if (AssetDatabase.IsValidFolder(path))
                drawInspector(path);

            base.OnInspectorGUI();
        }

        void drawInspector(string path)
        {
            if (note == null) note = new Note(path);

            GUI.enabled = true;
            EditorGUILayout.LabelField("注释");
            note.info = EditorGUILayout.TextArea(note.info, options);

            if(GUILayout.Button("保存"))
            {
                note.Save();
            }
        }
    }
}

