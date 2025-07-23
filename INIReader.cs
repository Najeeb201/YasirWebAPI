using System;
using System.Collections.Generic;
using System.IO;

public class INIReader
{
    private readonly string _path;

    public INIReader(string path) => _path = path;

    public string Read(string section, string key)
    {
        var data = new Dictionary<string, Dictionary<string, string>>();
        string currentSection = "";

        foreach (var line in File.ReadAllLines(_path))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                currentSection = trimmed[1..^1];
                if (!data.ContainsKey(currentSection))
                    data[currentSection] = new Dictionary<string, string>();
            }
            else if (trimmed.Contains('=') && !trimmed.StartsWith(";"))
            {
                var parts = trimmed.Split('=', 2);
                if (!data.ContainsKey(currentSection))
                    data[currentSection] = new Dictionary<string, string>();

                data[currentSection][parts[0].Trim()] = parts[1].Trim();
            }
        }

        return data.TryGetValue(section, out var sectionDict) &&
               sectionDict.TryGetValue(key, out var value) ? value : "";
    }
}
