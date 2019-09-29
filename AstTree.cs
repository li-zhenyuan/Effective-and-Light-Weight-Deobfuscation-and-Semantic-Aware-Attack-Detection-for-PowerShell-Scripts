using System;
using System.Management.Automation;
using System.Management.Automation.Language;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;


namespace PowershellDeobfuscation
{

    public class AstNode
    {
        public Ast ast;
        public enum NodeType { originNode, replaceNode, pipeNode, cleanPipeNode }; // 
        public NodeType type;

        public AstNode parent = null;
        public List<AstNode> childList = new List<AstNode>();

        // For PipeSubTree
        public int pipeNodeCount = 0; // the count of pipeNode in subtree
        public string command = "";
        public string updatedCommand = "";

        public class ReplaceSet
        {
            public string oldString;
            public string newString;

            public ReplaceSet(string oldString, string newString)
            {
                this.oldString = oldString;
                this.newString = newString;
            }
        }
        public List<ReplaceSet> replaceList = new List<ReplaceSet>();
        // ===============

        // For Code Similarity
        public string astString = "";
        public double matched = 0;
        // ===============

        public AstNode(Ast ast)
        {
            this.ast = ast;
            if (ast.GetType().ToString() == "System.Management.Automation.Language.PipelineAst")
            {
                type = NodeType.pipeNode;
            }
            else
            {
                type = NodeType.originNode;
            }
        }

        public AstNode(Ast ast, NodeType type)
        {
            this.ast = ast;
            if (ast.GetType().ToString() == "System.Management.Automation.Language.PipelineAst")
            {
                this.type = NodeType.pipeNode;
            }
            else
            {
                this.type = type;
            }
        }

        public bool HasChild()
        {
            return childList != null && childList.Count() != 0;
        }

        override public string ToString()
        {
            return ast.Extent.Text;
        }

        public string GetASTType()
        {
            return ast.GetType().ToString().Split('.')[4];
        }

        public string GetScript()
        {
            return ast.Extent.Text.Replace("\n", " ").Replace("\t", "").Replace("\r", "");
        }

        public static string GetShapedScript(string script)
        {
            if (script.Length > 60)
            {
                return (script.Substring(0, 30) + "..." + script.Substring(script.Length - 30)).Replace("\n", " ").Replace("\t", "").Replace("\r", "");
            }
            else
                return script.Replace("\n", " ").Replace("\t", "").Replace("\r", "");
        }

        public string GetShapedScript()
        {
            return GetShapedScript(ast.Extent.Text);
        }

        public static string GetColorFromType(NodeType type)
        {
            string color = "black";

            switch (type)
            {
                case AstNode.NodeType.pipeNode: color = "red"; break;
                case AstNode.NodeType.cleanPipeNode: color = "coral"; break;
                case AstNode.NodeType.replaceNode: color = "blue"; break;
                case AstNode.NodeType.originNode: color = "black"; break;
                default: break;
            }

            return color;
        }

        public void UpdateCommand(string oldCommands, string newCommands)
        {
            if (newCommands.Length == 0) // why?
                return;

            foreach (var set in replaceList)
            {
                oldCommands = oldCommands.Replace(set.oldString, set.newString);
            }

            if (oldCommands != newCommands)
            {
                replaceList.Add(new ReplaceSet(oldCommands, newCommands));
            }
            if (updatedCommand.Length == 0)
                updatedCommand = command.Replace(oldCommands, newCommands);
            else
                updatedCommand = updatedCommand.Replace(oldCommands, newCommands);
        }
    }

    public class AstTree
    {
        public AstNode root;
        public List<AstNode> nodeList = new List<AstNode>();
        public enum TreeType { basic, pipeSubTree };
        public TreeType type = TreeType.basic;

        public AstTree(string script)
        {
            script = Preprocess(script);
            ScriptBlockAst sb = System.Management.Automation.Language.Parser.ParseInput(script, out Token[] tokens, out ParseError[] errors);
            IEnumerable<Ast> astnodes = sb.FindAll(delegate (Ast t) { return true; }, true);
            List<Ast> astnodeList = astnodes.ToList<Ast>();
            ConstructTree(astnodeList);
        }

        public AstTree(string script, AstNode.NodeType type) : this(script)
        {
            foreach (var node in nodeList)
            {
                if (node.type != AstNode.NodeType.pipeNode)
                    node.type = type;
            }
        }

        public AstTree(AstNode node)
        {
            root = node;
            Traverse(node);
        }

        static string Preprocess(string sampleScript)
        {
            string result = sampleScript;
            result = result.Replace("`", "");//.ToLower();

            var temp = Regex.Matches(result, @"{([^}]*)}");

            return result;
        }

        public void Traverse(AstNode node)
        {
            Queue<AstNode> unvisitedNodeQueue = new Queue<AstNode>();
            unvisitedNodeQueue.Enqueue(node);

            while (unvisitedNodeQueue.Count > 0)
            {
                AstNode n = unvisitedNodeQueue.Dequeue();

                // traverse action...
                nodeList.Add(n);

                foreach (AstNode nc in n.childList)
                {
                    unvisitedNodeQueue.Enqueue(nc);
                }
            }
        }

        public void AddSubTree(AstNode node, string script)
        {
            if (script == "")
                return;

            AddSubTree(node, new AstTree(script));
        }

        public void AddSubTree(AstNode node, string script, int indexOfNode)
        {
            if (script == "")
                return;

            AddSubTree(node, new AstTree(script), indexOfNode);
        }

        void AddSubTree(AstNode node, AstTree tree)
        {
            AddSubTree(node, tree, 0);
        }

        public void AddSubTree(AstNode node, AstTree tree, int indexOfNode)
        {
            tree.root.parent = node;

            foreach (var n in tree.nodeList)
            {
                //n.type = AstNode.NodeType.replaceNode;

                nodeList.Add(n);
            }

            try
            {
                node.childList.Insert(indexOfNode, tree.root);
            }
            catch
            {
                return;
            }
        }

        public int RemoveSubTree(AstNode parent, AstNode node)
        {
            int indexOfNode = parent.childList.IndexOf(node);
            parent.childList.Remove(node);

            Queue<AstNode> unvisitedNodeQueue = new Queue<AstNode>();
            unvisitedNodeQueue.Enqueue(node);

            while (unvisitedNodeQueue.Count > 0)
            {
                AstNode n = unvisitedNodeQueue.Dequeue();

                nodeList.Remove(n);

                foreach (AstNode nc in n.childList)
                {
                    unvisitedNodeQueue.Enqueue(nc);
                }
            }

            return indexOfNode;
        }

        public void ReplaceSubTree(AstNode parent, AstNode originalNode, AstTree replacedTree)
        {
            int index = RemoveSubTree(parent, originalNode);
            AddSubTree(parent, replacedTree, index);
        }

        public void ReplaceSubTree(AstNode parent, AstNode originalNode, string replacedScript)
        {
            int index = RemoveSubTree(parent, originalNode);
            AddSubTree(parent, replacedScript, index);
        }

        public void ConstructTree(List<Ast> astnodeList)
        {
            root = new AstNode(astnodeList[0]);
            nodeList.Add(root);

            int nodeCount = astnodeList.Count();
            for (int i = 1; i < nodeCount; i++)
            {
                nodeList.Add(new AstNode(astnodeList[i]));
            }

            for (int i = 0; i < nodeCount; i++)
            {
                var iHash = nodeList[i].ast.GetHashCode();
                //System.Console.Out.WriteLine(iHash);
                for (int j = 0; j < nodeCount; j++)
                {
                    if (nodeList[j].ast.Parent == null)
                    {
                        continue;
                    }
                    if (iHash == nodeList[j].ast.Parent.GetHashCode())
                    {
                        nodeList[i].childList.Add(nodeList[j]);
                        nodeList[j].parent = nodeList[i];
                    }
                }
            }
        }

        public string GetTreeTag(AstNode node)
        {
            if (type == TreeType.basic)
                return node.ast.GetType().ToString().Split('.')[4] + "\\n" + node.GetShapedScript().Replace("\n", " ").Replace("\"", "\\\"").Replace("\t", "");
            else if (type == TreeType.pipeSubTree)
                return node.ast.GetType().ToString().Split('.')[4]
                    + "\\n"
                    + AstNode.GetShapedScript(node.command).Replace("\n", " ").Replace("\"", "\\\"").Replace("\t", "")
                    + "\\n"
                    + AstNode.GetShapedScript(node.updatedCommand).Replace("\n", " ").Replace("\"", "\\\"").Replace("\t", "")
                    + "\\n"
                    + node.pipeNodeCount
                    ;
            else
                return "";
        }

        public void DrawTreewithDot(string dotFileName)
        {
            FileStream fs = new FileStream(dotFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            TextWriter dot = new StreamWriter(fs);

            dot.WriteLine("digraph G {");

            string color = "black";
            foreach (AstNode node in nodeList)
            {
                color = AstNode.GetColorFromType(node.type);

                Console.Out.WriteLine(GetTreeTag(node));
                dot.WriteLine(String.Format("\tn{0} [label=\"{1}\", color=\"{2}\"]", node.GetHashCode(), GetTreeTag(node), color));
            }

            foreach (AstNode node in nodeList)
            {
                if (node.childList.Count() == 0)
                    continue;
                else
                {
                    foreach (AstNode child in node.childList)
                        dot.WriteLine(String.Format("\tn{0} -> n{1}", node.GetHashCode(), child.GetHashCode()));
                }
            }

            dot.WriteLine("}");
            dot.Close();
            fs.Close();
        }

        public void DrawTreewithDot()
        {
            DrawTreewithDot("ASTtree.dot");
        }

        public List<string> FindAllPipeNode()
        {
            List<string> pipeNodeList = new List<string>();

            foreach (AstNode node in nodeList)
            {
                if (node.ast.GetType().ToString() == "System.Management.Automation.Language.PipelineAst")
                    pipeNodeList.Add(node.GetScript());
            }

            return pipeNodeList;
        }

        public static Queue<AstNode> FindAllPipeNodeInSubTree(AstNode root)
        {
            Queue<AstNode> pipeQueue = new Queue<AstNode>();
            Queue<AstNode> childQueue = new Queue<AstNode>();

            childQueue.Enqueue(root);
            AstNode node;
            while (childQueue.Count != 0)
            {
                node = childQueue.Dequeue();
                if (node.type == AstNode.NodeType.pipeNode)
                {
                    pipeQueue.Enqueue(node);
                }
                foreach (var child in node.childList)
                {
                    childQueue.Enqueue(child);
                }
            }

            return pipeQueue;
        }

        public static AstData Tree2Feature(AstTree tree)
        {
            return Tree2Feature(tree.root);
        }

        public static AstData Tree2Feature(AstNode node)
        {
            string command = "";
            if (node.updatedCommand.Length != 0)
                command = node.command;
            else if (node.command.Length != 0)
                command = node.command;
            else
                command = node.ast.Extent.ToString();

            AstData data = new AstData(new Script2Vector(command).ToAStString());

            return data;
        }

        // For PipeSubTree
        public string GetCommand(AstNode node)
        {
            StringBuilder sb = new StringBuilder();

            // Deep-First Traversal
            Stack<AstNode> nodeStack = new Stack<AstNode>();
            nodeStack.Push(node);
            AstNode top;
            while (nodeStack.Count != 0)
            {
                top = nodeStack.Pop();
                if (top.HasChild())
                {
                    for (int i = top.childList.Count() - 1; i >= 0; i--)
                    {
                        nodeStack.Push(top.childList[i]);
                    }
                }
                else
                {
                    sb.Append(top.command);
                }
            }

            return sb.ToString();
        }

        public int CountPipeNode(AstNode node)
        {
            int count = 0;

            foreach (var n in node.childList)
            {
                count += CountPipeNode(n);
            }

            node.pipeNodeCount = count;
            if (node.type == AstNode.NodeType.pipeNode)
            {
                count++;
            }

            return count;
        }

        public void Shrink(AstNode node)
        {
            List<AstNode> tempList = new List<AstNode>(node.childList);
            foreach (var n in tempList)
            {
                RemoveSubTree(node, n);
            }

            node.childList.Clear();
        }

        public void CompressTree(AstNode root)
        {
            if (root.pipeNodeCount == 0)
            {
                Shrink(root);
            }
            else
            {
                foreach (var child in root.childList)
                {
                    CompressTree(child);
                }
            }
        }

        public void InitCommands(AstNode root)
        {
            if (root.pipeNodeCount > 0 || root.type == AstNode.NodeType.pipeNode)

                root.command = root.ToString();
            foreach (var child in root.childList)
            {
                InitCommands(child);
            }
        }

        public void InitPipeSubTree()
        {
            CountPipeNode(root);
            InitCommands(root);
            CompressTree(root);

            type = TreeType.pipeSubTree;
        }
    }
}

