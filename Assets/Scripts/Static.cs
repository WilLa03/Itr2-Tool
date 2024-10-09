using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


/*public static class Static
{
    public static List<T> GetAllInstances1<T>() where T : ScriptableObject
    {
        return AssetDatabase.FindAssets($"t: {typeof(T).Name}").ToList().Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath).ToList();
    }
    
    
    public static T GetAllInstances<T>() where T : ScriptableObject
    {
        string[] guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name); //FindAssets uses tags check documentation for more info
        T[] a = new T[guids.Length];
        for(int i =0;i<guids.Length;i++) //probably could get optimized
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a = AssetDatabase.LoadAssetAtPath(path);
        }

        return a;

    }
    
}*/

