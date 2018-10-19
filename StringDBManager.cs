using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringDBManager : MonoBehaviour
{

    public void Binarize()
    {
        StringDB.Binarize();
    }

    public void PrintDictionary()
    {
        Debug.Log(StringDB.ReturnDictionary());
    }

    public void Ingest(string outputTextField)
    {
        StringDB.Destroy();
        StringDB.outputFilePath = outputTextField;
        StringDB.Ingest();
    }


}
