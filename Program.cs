using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowershellDeobfuscation
{
    class Program
    {
        static void Main(string[] args)
        {
            Deobfuscation();
        }

        static void Deobfuscation()
        {
            string sampleScript = "";

            string scriptPath = "Data\\sample_1.ps1";
            string modelPath = "Data\\ObfuscationClassifierModel.zip";

            FileInfo file = new FileInfo(scriptPath);

            try
            {
                FileStream fs = new FileStream(scriptPath, FileMode.Open, FileAccess.Read);
                TextReader scriptIn = new StreamReader(fs);
                sampleScript = scriptIn.ReadToEnd();
            }
            catch
            {
                return;
            }

            AstTree tree = new AstTree(sampleScript);

            tree.InitPipeSubTree();

            TraverseSubTree traverser = new TraverseSubTree(modelPath);
            traverser.TraverseCheckPipeSubtree(tree);
            Console.Out.WriteLine(tree.root.updatedCommand);
        }
    }
}
