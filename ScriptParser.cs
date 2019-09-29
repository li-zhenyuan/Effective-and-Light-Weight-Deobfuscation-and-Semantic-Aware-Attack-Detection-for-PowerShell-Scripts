using System;
using System.Collections.Generic;
using System.Text;
using System.Management.Automation.Language;

namespace PowershellDeobfuscation
{
    public class ScriptParser
    {
        String sampleScript;

        public List<Ast> commandAstList = new List<Ast>();
        public List<string> asttypeList = new List<string>();

        Token[] tokens;
        public double maxTokenLength = 0;
        public double meanTokenLength = 0;

        public ScriptParser()
        {

        }
        public ScriptParser(String sampleScript)
        {
            this.sampleScript = sampleScript;
            AstParser();
            TokenParser();
        }

        public void AstParser()
        {
            ScriptBlockAst sb = System.Management.Automation.Language.Parser.ParseInput(sampleScript, out tokens, out ParseError[] errors);

            // AST type list https://docs.microsoft.com/en-us/dotnet/api/system.management.automation.language?view=powershellsdk-1.1.0
            IEnumerable<Ast> astnodeList = sb.FindAll(delegate (Ast t)
            { return true; }
            , true);


            foreach (var astnode in astnodeList)
            {
                asttypeList.Add(astnode.GetType().ToString());
            }
        }

        public void TokenParser()
        {
            foreach (var s in tokens)
            {
                string token = s.Extent.ToString();
                maxTokenLength = token.Length > maxTokenLength ? token.Length : maxTokenLength;
                meanTokenLength += token.Length;
            }

            meanTokenLength /= tokens.Length;
        }

        public static double ShannonEntropy(string s)
        {
            var map = new Dictionary<char, int>();
            foreach (char c in s)
            {
                if (!map.ContainsKey(c))
                    map.Add(c, 1);
                else
                    map[c] += 1;
            }

            double result = 0.0;
            int len = s.Length;
            foreach (var item in map)
            {
                var frequency = (double)item.Value / len;
                result -= frequency * (Math.Log(frequency) / Math.Log(2));
            }

            return result;
        }
    }

    class FindAllVisitor : AstVisitor
    {
        private List<Ast> commandAstList = new List<Ast>();

        void Initialize(Ast ast)
        {

        }

        public List<Ast> Visit(Ast ast)
        {
            if (!(ast is ScriptBlockAst || ast is FunctionMemberAst || ast is FunctionDefinitionAst))
            {

            }

            var visitor = new FindAllVisitor();

            if (ast is ScriptBlockAst)
                (ast as ScriptBlockAst).Visit(visitor);
            else if (ast is FunctionDefinitionAst)
                (ast as FunctionDefinitionAst).Body.Visit(visitor);
            else if (ast is FunctionMemberAst && (ast as FunctionMemberAst).Parameters != null)
                visitor.VisitParameters((ast as FunctionDefinitionAst).Parameters);

            List<string> commandList = new List<string>();
            foreach (var command in visitor.commandAstList)
            {
                string commandString = command.Extent.ToString();
                commandString = commandString.ToLower();
                commandList.Add(commandString);
                Console.Out.WriteLine(commandString);
            }

            return commandAstList;
        }

        internal void VisitParameters(IReadOnlyCollection<ParameterAst> parameters)
        {
            foreach (var t in parameters)
            {
                var variableExpressionAst = t.Name;
                var varPath = variableExpressionAst.VariablePath;
            }
        }

        public override AstVisitAction VisitScriptBlock(ScriptBlockAst scriptBlockAst)
        {
            return AstVisitAction.Continue;
        }

        public override AstVisitAction VisitFunctionDefinition(FunctionDefinitionAst functionDefinitionAst)
        {
            return AstVisitAction.Continue;
        }

        public override AstVisitAction VisitCommand(CommandAst commandAst)
        {
            commandAstList.Add(commandAst);
            return AstVisitAction.Continue;
        }
    }
}
