using System;
using UnityEngine;

public static class JsonHelper
{
    // The built in Unity Json Utility doesn't support Json arrays
    // https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity

    public static T[] GetJsonArray<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] array = null;
    }
}