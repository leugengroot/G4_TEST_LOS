namespace RpgGame.Global;

using Godot;
using System;
using System.IO;

public partial class ExternalData : Node
{
    public static void SaveToJsonFile(string path, string fileName, string data)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = Path.Join(path, fileName);
                
        try
        {
            File.WriteAllText(path, data);
        }
        catch (Exception e) { GD.Print(e); }
    }

    public static string LoadJsonFile(string path, string fileName)
    {
        string data = null;
        path = Path.Join(path, fileName);
        
        try
        {            
            data = File.ReadAllText(path);
        }
        catch (Exception e) { GD.Print(e); }

        return data;
    }
}
