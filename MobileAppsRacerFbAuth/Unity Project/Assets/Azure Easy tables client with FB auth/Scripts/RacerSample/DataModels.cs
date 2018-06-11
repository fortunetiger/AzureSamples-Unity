using System;

[Serializable]
public class CrashInfo : EasyTablesObjectBase
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class HighScoreInfo : EasyTablesObjectBase
{
    public string Name;
    public float Time;
}