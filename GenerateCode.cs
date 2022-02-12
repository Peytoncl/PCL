using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Net;
using System.IO.Compression;

namespace PCL
{
    class GenerateCode
    {
        static string template = "public class template : MonoBehaviour\n{\nhere\n}";

        static void Main(string[] args)
        {
            if(Directory.Exists("old")) { Directory.Delete("old", true); File.Delete("PCL.zip"); }
            string filePath = args[0];

            if(filePath == "update")
            {
                if(!File.Exists("version.txt")) return;
                WebClient client = new WebClient();
                string latest = client.DownloadString("https://pastebin.com/raw/X8qFvUAq");
                if(File.ReadAllText("version.txt") == latest)
                {
                    Console.WriteLine("you are already on the latest version bro");
                    return;
                }
                else
                {
                    //https://github.com/Peytoncl/PCL/releases/download/v1/PCL.exe

                    Directory.CreateDirectory("old");

                    client.DownloadFile("https://github.com/Peytoncl/PCL/releases/download/v" + latest + "/PCL.zip", "PCL.zip");
                    File.WriteAllText("version.txt", latest);

                    Directory.Move("ref", "old/ref");
                    foreach(string file in Directory.GetFiles(Directory.GetCurrentDirectory()))
                    {
                        if(Path.GetFileName(file) == "version.txt" || Path.GetFileName(file) == "PCL.zip") continue;

                        File.Move(file, Directory.GetCurrentDirectory() + "/old/" + Path.GetFileName(file));
                    }

                    ZipFile.ExtractToDirectory("PCL.zip", Directory.GetCurrentDirectory());

                    return;
                }
            }
            else if(filePath == "init")
            {
                if(!File.Exists("version.txt"))
                {
                    WebClient client = new WebClient();
                    string latest = client.DownloadString("https://pastebin.com/raw/X8qFvUAq");

                    File.Create("version.txt").Close();
                    File.WriteAllText("version.txt", latest);
                    
                    return;
                }
                else return;
            }
            else if(filePath == "version") 
            { 
                if(File.Exists("version.txt")) 
                {
                    string isLatest = "";

                    WebClient client = new WebClient();
                    string latest = client.DownloadString("https://pastebin.com/raw/X8qFvUAq");

                    if(File.ReadAllText("version.txt") == latest) isLatest = "which is the latest version!";
                    else isLatest = "please run 'PCL update' to update your PCL.";

                    Console.WriteLine("You are on version " + File.ReadAllText("version.txt") + ", " + isLatest); 

                    return;
                } 
                else { Console.WriteLine("Please run 'PCL init' in your command line."); return; } 
            }
            else if(filePath == "help")
            {
                Console.WriteLine("PLC help - will list all commands\nPLC (.plc filepath) - will compile a .plc file to .cs file\nPCL version - will tell you your current version\nPCL init - will create you a version folder (recommended to run before updating)");
                return;
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
