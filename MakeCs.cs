using System;

namespace PCL
{
    public class MakeCs
    {
        static string[] tokens1 = {"local var", "fun void", "module class", "&& and"};
        static string[] statements = {"if", "while", "for", "foreach", "do"};

        public static string Generate(string script)
        {
            string newScript = script;

            foreach(string token in tokens1)
            {
                try
                {
                    newScript = newScript.Replace(token.Split(" ")[0], token.Split(" ")[1]);
                }
                catch {}
            }

            foreach(string line in script.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach(string statement in statements)
                {
                    if(line.Contains(statement))
                    {
                        string newLine = line.Replace(statement, statement + "(");
                        newLine += ")";

                        newScript = newScript.Replace(line, newLine);
                    }
                }
            }

            return newScript;
        }
    }
}