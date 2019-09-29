using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace PowershellDeobfuscation
{
    public class TraverseSubTree
    {
        string modelPath = "Data\\ObfuscationClassifierModel.zip";

        Classifier c = new Classifier();
        InstancePF psIns = new InstancePF();

        public TraverseSubTree()
        {
            c.initPredEngine(modelPath);
        }

        public TraverseSubTree(string modelPath)
        {
            this.modelPath = modelPath;
            c.initPredEngine(modelPath);
        }

        public void ReverseTraverseCheckSubtree(AstTree tree)
        {
            AstNode node = tree.root;

            Queue<AstNode> unvisitedNodeQueue = new Queue<AstNode>();
            unvisitedNodeQueue.Enqueue(node);

            Stack<AstNode> pipeNodeStack = new Stack<AstNode>();

            while (unvisitedNodeQueue.Count > 0)
            {
                AstNode n = unvisitedNodeQueue.Dequeue();
                
                if (n.ast.GetType().ToString() == "System.Management.Automation.Language.PipelineAst")
                {
                    pipeNodeStack.Push(n);
                }

                foreach (AstNode nc in n.childList)
                {
                    unvisitedNodeQueue.Enqueue(nc);
                }
            }
            
            while (pipeNodeStack.Count > 0)
            {
                AstNode n = pipeNodeStack.Pop();
                Classifier.ClassifierResult result = c.testWithModel(AstTree.Tree2Feature(n));

                if (result != Classifier.ClassifierResult.unobfuscated)
                {
                    // what to do with the obfuscated sub-tree
                    string returnScript = psIns.addScript(n.ast.Extent.Text);
                    Console.Out.WriteLine(String.Format("Script:{0}, result:{1}, Deobfuscation:{2}", n.ast.Extent.Text, result, returnScript));

                    tree.AddSubTree(n, returnScript);
                }
                else
                {
                    Console.Out.WriteLine(String.Format("Script:{0}, result:{1}", n.ast.Extent.Text, result));
                }
            }
        }

        public class DeobfuscationResult
        {
            public int obfuscated = 0;
            public string originalScript = "";
            public string deobfuscatedScript = "";
            public string shapedScript = "";
        }

        public List<DeobfuscationResult> ReverseTraverseCheckSubtreeWithExperimentalOutput(AstTree tree)
        {
            AstNode node = tree.root;

            Queue<AstNode> unvisitedNodeQueue = new Queue<AstNode>();
            unvisitedNodeQueue.Enqueue(node);

            Stack<AstNode> pipeNodeStack = new Stack<AstNode>();

            while (unvisitedNodeQueue.Count > 0)
            {
                AstNode n = unvisitedNodeQueue.Dequeue();
                
                if (n.ast.GetType().ToString() == "System.Management.Automation.Language.PipelineAst")
                {
                    pipeNodeStack.Push(n);
                }

                foreach (AstNode nc in n.childList)
                {
                    unvisitedNodeQueue.Enqueue(nc);
                }
            }

            InstancePF psIns = new InstancePF();

            List<DeobfuscationResult> outputList = new List<DeobfuscationResult>();
            while (pipeNodeStack.Count > 0)
            {
                AstNode n = pipeNodeStack.Pop();
                Classifier.ClassifierResult result = c.testWithModel(AstTree.Tree2Feature(n));

                DeobfuscationResult output = new DeobfuscationResult();
                output.originalScript = AstNode.GetShapedScript(n.ast.Extent.Text);


                if (result != Classifier.ClassifierResult.unobfuscated)
                {
                    string returnScript = psIns.addScript(n.ast.Extent.Text);
                    Console.Out.WriteLine(String.Format("Script:{0}, result:{1}, Deobfuscation:{2}", n.ast.Extent.Text, result, returnScript));

                    output.obfuscated = 1;
                    output.deobfuscatedScript = AstNode.GetShapedScript(returnScript);
                    
                    if (returnScript.Length != 0)
                        tree.RemoveSubTree(n, n.childList[0]);
                    tree.AddSubTree(n, returnScript);
                }
                else
                {
                    Console.Out.WriteLine(String.Format("Script:{0}, result:{1}", n.ast.Extent.Text, result));
                }
                outputList.Add(output);
            }
            return outputList;
        }

        public void TraverseCheckSubtree(AstTree tree)
        {
            TraverseCheckSubtree(tree.root);
        }

        public void TraverseCheckSubtree(AstNode node)
        {
            Queue<AstNode> unvisitedNodeQueue = new Queue<AstNode>();
            unvisitedNodeQueue.Enqueue(node);

            while (unvisitedNodeQueue.Count > 0)
            {
                AstNode n = unvisitedNodeQueue.Dequeue();
                
                if (n.ast.GetType().ToString() == "System.Management.Automation.Language.PipelineAst")
                {
                    AstData data = AstTree.Tree2Feature(n);
                    Classifier.ClassifierResult result = c.testWithModel(data);
                    Console.Out.WriteLine(String.Format("Script:{0}, result:{1}", n.ast.Extent.Text, result.ToString()));
                }

                foreach (AstNode nc in n.childList)
                {
                    unvisitedNodeQueue.Enqueue(nc);
                }
            }
        }

        public int TraverseCheckPipeSubtree(AstTree tree)
        {
            int obfuscatedOrNot = 0;

            Queue<AstNode> pipeQueue = new Queue<AstNode>();
            AstTree subtree;
            AstNode parent;

            foreach (AstNode node in tree.nodeList)
            {
                if (node.type == AstNode.NodeType.pipeNode)
                {
                    pipeQueue.Enqueue(node);
                }
            }

            AstNode top;
            while (pipeQueue.Count != 0)
            {
                top = pipeQueue.Dequeue();
                int subtreePipeCount = -1;

                if (top.pipeNodeCount != 0)
                {
                    pipeQueue.Enqueue(top);
                }
                else
                {
                    if (c.testWithModel(AstTree.Tree2Feature(top)) == Classifier.ClassifierResult.unobfuscated)
                    {
                        top.type = AstNode.NodeType.cleanPipeNode;
                        string feature = new Script2Vector(top.command).ToAStString();
                        Console.Out.WriteLine(feature);
                        if (top.updatedCommand.Length == 0)
                            top.updatedCommand = top.command;
                    }
                    else
                    {
                        try
                        {
                            string tempCommand = "";
                            tempCommand = psIns.addScript(top.command);
                            if (tempCommand.Length == 0)
                            {
                                top.type = AstNode.NodeType.cleanPipeNode;
                                goto a;
                            }
                            else
                            {
                                tempCommand = CheckParents(tempCommand, top);
                                top.updatedCommand = tempCommand;
                            }
                        }
                        catch
                        {
                            top.type = AstNode.NodeType.cleanPipeNode;
                            goto a;
                        }
                        obfuscatedOrNot = 1;

                        subtree = new AstTree(top.updatedCommand, AstNode.NodeType.replaceNode);
                        subtree.InitPipeSubTree();

                        tree.ReplaceSubTree(top.parent, top, subtree);
                        
                        Queue<AstNode> newQueue = AstTree.FindAllPipeNodeInSubTree(subtree.root);
                        AstNode node;
                        while (newQueue.Count != 0)
                        {
                            node = newQueue.Dequeue();
                        }

                    a:;
                    }
                    
                    parent = top.parent;
                    while (!(parent.parent == null || parent.type == AstNode.NodeType.pipeNode))
                    {
                        parent = parent.parent;
                    }
                    parent.UpdateCommand(top.command, top.updatedCommand);
                    experimentOut.WriteLine(String.Format("{0}`{2}`{1}", AstNode.GetShapedScript(top.command), AstNode.GetShapedScript(top.updatedCommand), top.command != top.updatedCommand));

                    parent = top.parent;
                    while (parent.parent != null)
                    {
                        parent.pipeNodeCount += subtreePipeCount;

                        if (parent.pipeNodeCount == 0)
                        {
                            tree.Shrink(parent);
                        }

                        parent = parent.parent;
                    }
                }
            }
            return obfuscatedOrNot;
        }

        private string CheckParents(string tempCommand, AstNode top)
        {
            while (top.parent.GetASTType().Contains("Command"))
            {

            }
            return tempCommand;
        }

        public static FileStream fs = new FileStream("experimentResult.ps1", FileMode.OpenOrCreate, FileAccess.Write);
        public static TextWriter experimentOut = new StreamWriter(fs);
    }
}
