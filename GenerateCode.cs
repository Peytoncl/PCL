using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace PCL
{
    class GenerateCode
    {
        static string template = "public class template : MonoBehaviour\n{\nhere\n}";

        static void Main(string[] args)
        {
            string filePath = args[0];

            if(filePath == "update")
            {
                
            }

            string fileContents = File.ReadAllText(filePath);

            string newScript = template;

            newScript = newScript.Replace("template", Path.GetFileNameWithoutExtension(filePath));
            newScript = newScript.Replace("here", MakeCs.Generate(fileContents));

            List<string> allUsings = new List<string>();
            foreach(string line in newScript.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if(line.Contains("using"))
                {
                    foreach(string potentialUsing in line.Split("\n"))
                    {
                        if(potentialUsing.Contains("using"))
                        {
                            newScript = newScript.Replace(potentialUsing, "");

                            allUsings.Add(potentialUsing + "\n");
                        }
                    }
                }
            }

            if(File.Exists(filePath + ".cs"))
            {
                File.Delete(filePath + "cs");
            }
            File.Create(filePath + ".cs").Close();

            foreach(string using1 in allUsings)
            {
                var sb = new StringBuilder();
                sb.Append(newScript);
                sb.Insert(0, using1);
                newScript = sb.ToString();
            }

            File.WriteAllText(filePath + ".cs", newScript);
        }
    }
}
