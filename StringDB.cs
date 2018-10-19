using System;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using LumenWorks.Framework.IO.Csv;

public static class StringDB
{
    /// <summary>
    /// Language enumerator
    /// </summary>
    public enum LEnum
    {
        English,
        Korean
    }

    /// <summary>
    /// String enumerator key
    /// </summary>
    public enum SEnum
    {
        Title1,
        Title2
    }

    public static Dictionary<LEnum, Dictionary<SEnum, string>> dictionary = new Dictionary<LEnum, Dictionary<SEnum, string>>();

    //Default Language
    public static LEnum lang = LEnum.English;
    public static string markUp = "~!~";


    // ex: C:/ Users / Moses / Documents / Text.csv
    public static string inputFilePath = "";


    //ex: "assets/binary.txt
    public static string outputFilePath = "";

    public static void Binarize()
    {
        string allText;
        using (var read = new StreamReader(String.Format(@"{0}", inputFilePath)))
        {
            allText = read.ReadToEnd();
        }

        var filepath = outputFilePath;

        using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
        {
            writer.WriteLine(Binary.Base64Encode(allText));
            Debug.Log("Binarized!");
        }
    }

    //public static void UnBinarize()
    //{
    //    string allText;

    //    using (var read = new StreamReader(@"assets/binary.csv"))
    //    {
    //        allText = read.ReadToEnd();
    //    }

    //    var filepath = "binary.csv";

    //    using (StreamWriter writer = new StreamWriter(new FileStream(filepath, FileMode.Create, FileAccess.Write)))
    //    {
    //        writer.WriteLine(Binary.Base64Decode(allText));
    //    }
    //}

    /// <summary>
    /// Create dictionary by reading from the binary file
    /// </summary>
    public static void Ingest()
    {
        List<string> lang = new List<string>();
        //UnBinarize();
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        using (var reader = new StreamReader(outputFilePath))
        {
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();

            string allStrings = reader.ReadToEnd();

            writer.Write(Binary.Base64Decode(allStrings));
            writer.Flush();
            stream.Position = 0;
        }

        using (CachedCsvReader csv = new CachedCsvReader(new StreamReader(stream), true))
        {
            int fieldCount = csv.FieldCount;

            string[] headers = csv.GetFieldHeaders();

            for (int count = 1; count < headers.Length; count++)
            {
                lang.Add(headers[count]);
            }

            for (int count = 0; count < lang.Count; count++)
            {
                csv.MoveTo(-1);

                LEnum lEnum = (LEnum)Enum.Parse(typeof(LEnum), lang[count]);
                Dictionary<SEnum, string> StringDict = new Dictionary<SEnum, string>();

                while (csv.ReadNextRecord())
                {
                    SEnum sEnum = (SEnum)Enum.Parse(typeof(SEnum), csv[0]);
                    StringDict.Add(sEnum, csv[count + 1]);
                }

                dictionary.Add(lEnum, StringDict);

            }
        }

    }

    /// <summary>
    /// Switch whole dictionary to a language
    /// </summary>
    /// <param name="lEnum"></param>
    public static void SetDefaultLanguage(LEnum lEnum)
    {
        lang = lEnum;
    }

    /// <summary>
    /// Retrieve string value with given key and lang
    /// </summary>
    /// <param name="sEnum"></param>
    /// <param name="lEnum"></param>
    /// <returns></returns>
    public static string GetString(LEnum lEnum, SEnum sEnum)
    {
        return dictionary[lEnum][sEnum];
    }

    /// <summary>
    /// Retrieve string value with given key and replace with wildcards
    /// </summary>
    /// <param name="sEnum"></param>
    /// <param name="wildcards"></param>
    /// <returns></returns>
    public static string GetString(SEnum sEnum, string[] wildcards)
    {
        string line = dictionary[lang][sEnum];

        if (NumberOfMarkups(line) == wildcards.Length)
        {
            for (int count = 0; count < wildcards.Length; count++)
            {
                line = line.Substring(0, line.IndexOf(markUp, StringComparison.Ordinal)) + wildcards[count] + line.Substring(line.IndexOf(markUp, StringComparison.Ordinal) + markUp.Length);
            }
            return line;
        }
        return "Number of wildcards does not match number of spaces available";
    }

    /// <summary>
    /// Retrieve string value with given key and language, and replace with wildcards
    /// </summary>
    /// <param name="sEnum"></param>
    /// <param name="lEnum"></param>
    /// <param name="wildcards"></param>
    /// <returns></returns>
    public static string GetString(LEnum lEnum, SEnum sEnum, string[] wildcards)
    {
        string line = dictionary[lEnum][sEnum];

        if (NumberOfMarkups(line) == wildcards.Length)
        {
            for (int count = 0; count < wildcards.Length; count++)
            {
                line = line.Substring(0, line.IndexOf(markUp, StringComparison.Ordinal)) + wildcards[count] + line.Substring(line.IndexOf(markUp, StringComparison.Ordinal) + markUp.Length);
            }
            return line;
        }
        return "Number of wildcards does not match number of spaces available";
    }


    /// <summary>
    /// return number of wildcard spaces in string
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    public static int NumberOfMarkups(string line)
    {
        return (line.Split(Convert.ToChar(markUp.Substring(0, 1))).Length - 1) / 2;

    }

    /// <summary>
    /// print the whole dictionary
    /// </summary>
    public static string ReturnDictionary()
    {
        string whole = "";
        foreach (KeyValuePair<LEnum, Dictionary<SEnum, string>> dict in dictionary)
        {
            Console.WriteLine(string.Format("LEnum = {0}", dict.Key));
            whole += string.Format("LEnum = {0}", dict.Key);
            whole += "\n {";
            foreach (KeyValuePair<SEnum, string> stringDict in dict.Value)
            {
                whole += "\n" + string.Format("SEnum = {0}, Value = {1}", stringDict.Key, stringDict.Value);
                //Console.WriteLine(string.Format("SEnum = {0}, Value = {1}", stringDict.Key, stringDict.Value));
            }
            whole += "\n }\n";
        }
        return whole;
    }

    /// <summary>
    /// Delete all data in dictionaries
    /// </summary>
    public static void Destroy()
    {
        dictionary.Clear();
    }


}


public static class Binary
{
    public static string Base64Encode(string plainText)
    {
        var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);


        return System.Convert.ToBase64String(plainTextBytes);
    }

    public static string Base64Decode(string base64EncodedData)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

}

