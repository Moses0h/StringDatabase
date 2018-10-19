using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StringDBManager))]
public class StringDBManagerEditor : Editor
{
    private string inputTextField = "C:/Users/Moses/Documents/Text.csv";
    private string outputTextField = "Assets/binary.txt";
    private StringDBManager script;

    public void Awake()
    {
        script = (StringDBManager)target;
        script.Ingest(outputTextField);
    }

    public override void OnInspectorGUI()
    {

        DrawDefaultInspector();

        GUILayout.Label("Input File Path");
        inputTextField = EditorGUILayout.TextArea(inputTextField);

        GUILayout.Label("");

        GUILayout.Label("Output File Path");
        outputTextField = EditorGUILayout.TextArea(outputTextField);

        GUILayout.Label("");

        if (GUILayout.Button("Binarize Me"))
        {
            StringDB.inputFilePath = inputTextField;
            StringDB.outputFilePath = outputTextField;
            script.Binarize();
        }

        if (GUILayout.Button("Print Dictionary"))
        {
            StringDB.outputFilePath = outputTextField;
            script.PrintDictionary();
        }

    }
}
