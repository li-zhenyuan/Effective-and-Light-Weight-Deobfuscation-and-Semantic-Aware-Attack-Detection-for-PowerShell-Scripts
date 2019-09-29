using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PowershellDeobfuscation
{
    public class Script2Vector
    {
        public static TextWriter Out = Console.Out;

        string script;

        /*
         * Features:
         * 1. AST level feature + Representational Learning[19]
         * 2. Character level & token level[9]:
	     *      1. Character frequency, entropy, length(max, min, mean, ...)
	     *      2. Distribution of operators(assignment, binary, …)
	     *      3. Statistics of command names(save for detection)
         */

        List<short> featureVectorA = new List<short>(); // Feature 1.
        List<double> featureVectorB = new List<double>(); // Feature 2. Entropy, mean length,


        public Script2Vector(string script)
        {
            // init Ast Features
            ScriptParser ast = new ScriptParser(script);
            AST2Vector(ast.asttypeList);

            // init Character Features
            featureVectorB.Add(ScriptParser.ShannonEntropy(script));
            featureVectorB.Add(script.Length);

            // init Token Features
            featureVectorB.Add(ast.maxTokenLength);
            featureVectorB.Add(ast.meanTokenLength);
        }

        public string ToAstString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in featureVectorA)
            {
                sb.Append(i);
                sb.Append(",");
            }

            return sb.ToString();
        }

        public string ToAStString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var i in featureVectorA)
            {
                sb.Append(i);
                sb.Append(",");
            }
            foreach (var i in featureVectorB)
            {
                sb.Append(i);
                sb.Append(",");
            }

            sb.Append('0');
            return sb.ToString();
        }

        void AST2Vector(List<string> asttypeList)
        {
            short[] vector = new short[71];
            foreach (string asttype in asttypeList)
            {
                switch (asttype)
                {
                    case "System.Management.Automation.Language.ArrayExpressionAst": vector[0]++; break;
                    case "System.Management.Automation.Language.ArrayLiteralAst": vector[1]++; break;
                    case "System.Management.Automation.Language.AssignmentStatementAst": vector[2]++; break;
                    case "System.Management.Automation.Language.AttributeAst": vector[3]++; break;
                    case "System.Management.Automation.Language.AttributeBaseAst": vector[4]++; break;
                    case "System.Management.Automation.Language.AttributedExpressionAst": vector[5]++; break;
                    case "System.Management.Automation.Language.BaseCtorInvokeMemberExpressionAst": vector[6]++; break;
                    case "System.Management.Automation.Language.BinaryExpressionAst": vector[7]++; break;
                    case "System.Management.Automation.Language.BlockStatementAst": vector[8]++; break;
                    case "System.Management.Automation.Language.BreakStatementAst": vector[9]++; break;
                    case "System.Management.Automation.Language.CatchClauseAst": vector[10]++; break;
                    case "System.Management.Automation.Language.CommandAst": vector[11]++; break;
                    case "System.Management.Automation.Language.CommandBaseAst": vector[12]++; break;
                    case "System.Management.Automation.Language.CommandElementAst": vector[13]++; break;
                    case "System.Management.Automation.Language.CommandExpressionAst": vector[14]++; break;
                    case "System.Management.Automation.Language.CommandParameterAst": vector[15]++; break;
                    case "System.Management.Automation.Language.ConfigurationDefinitionAst": vector[16]++; break;
                    case "System.Management.Automation.Language.ConstantExpressionAst": vector[17]++; break;
                    case "System.Management.Automation.Language.ContinueStatementAst": vector[18]++; break;
                    case "System.Management.Automation.Language.ConvertExpressionAst": vector[19]++; break;
                    case "System.Management.Automation.Language.DataStatementAst": vector[20]++; break;
                    case "System.Management.Automation.Language.DoUntilStatementAst": vector[21]++; break;
                    case "System.Management.Automation.Language.DoWhileStatementAst": vector[22]++; break;
                    case "System.Management.Automation.Language.DynamicKeywordStatementAst": vector[23]++; break;
                    case "System.Management.Automation.Language.ErrorExpressionAst": vector[24]++; break;
                    case "System.Management.Automation.Language.ErrorStatementAst": vector[25]++; break;
                    case "System.Management.Automation.Language.ExitStatementAst": vector[26]++; break;
                    case "System.Management.Automation.Language.ExpandableStringExpressionAst": vector[27]++; break;
                    case "System.Management.Automation.Language.ExpressionAst": vector[28]++; break;
                    case "System.Management.Automation.Language.FileRedirectionAst": vector[29]++; break;
                    case "System.Management.Automation.Language.ForEachStatementAst": vector[30]++; break;
                    case "System.Management.Automation.Language.FunctionDefinitionAst": vector[31]++; break;
                    case "System.Management.Automation.Language.FunctionMemberAst": vector[32]++; break;
                    case "System.Management.Automation.Language.HashtableAst": vector[33]++; break;
                    case "System.Management.Automation.Language.IfStatementAst": vector[34]++; break;
                    case "System.Management.Automation.Language.IndexExpressionAst": vector[35]++; break;
                    case "System.Management.Automation.Language.InvokeMemberExpressionAst": vector[36]++; break;
                    case "System.Management.Automation.Language.LabeledStatementAst": vector[37]++; break;
                    case "System.Management.Automation.Language.LoopStatementAst": vector[38]++; break;
                    case "System.Management.Automation.Language.MemberAst": vector[39]++; break;
                    case "System.Management.Automation.Language.MemberExpressionAst": vector[40]++; break;
                    case "System.Management.Automation.Language.MergingRedirectionAst": vector[41]++; break;
                    case "System.Management.Automation.Language.NamedAttributeArgumentAst": vector[42]++; break;
                    case "System.Management.Automation.Language.NamedBlockAst": vector[43]++; break;
                    case "System.Management.Automation.Language.ParamBlockAst": vector[44]++; break;
                    case "System.Management.Automation.Language.ParameterAst": vector[45]++; break;
                    case "System.Management.Automation.Language.ParenExpressionAst": vector[46]++; break;
                    case "System.Management.Automation.Language.PipelineAst": vector[47]++; break;
                    case "System.Management.Automation.Language.PipelineBaseAst": vector[48]++; break;
                    case "System.Management.Automation.Language.PropertyMemberAst": vector[49]++; break;
                    case "System.Management.Automation.Language.RedirectionAst": vector[50]++; break;
                    case "System.Management.Automation.Language.ReturnStatementAst": vector[51]++; break;
                    case "System.Management.Automation.Language.ScriptBlockAst": vector[52]++; break;
                    case "System.Management.Automation.Language.ScriptBlockExpressionAst": vector[53]++; break;
                    case "System.Management.Automation.Language.StatementAst": vector[54]++; break;
                    case "System.Management.Automation.Language.StatementBlockAst": vector[55]++; break;
                    case "System.Management.Automation.Language.StringConstantExpressionAst": vector[56]++; break;
                    case "System.Management.Automation.Language.SubExpressionAst": vector[57]++; break;
                    case "System.Management.Automation.Language.SwitchStatementAst": vector[58]++; break;
                    case "System.Management.Automation.Language.ThrowStatementAst": vector[59]++; break;
                    case "System.Management.Automation.Language.TrapStatementAst": vector[60]++; break;
                    case "System.Management.Automation.Language.TryStatementAst": vector[61]++; break;
                    case "System.Management.Automation.Language.TypeConstraintAst": vector[62]++; break;
                    case "System.Management.Automation.Language.TypeDefinitionAst": vector[63]++; break;
                    case "System.Management.Automation.Language.TypeExpressionAst": vector[64]++; break;
                    case "System.Management.Automation.Language.UnaryExpressionAst": vector[65]++; break;
                    case "System.Management.Automation.Language.UsingExpressionAst": vector[66]++; break;
                    case "System.Management.Automation.Language.UsingStatementAst": vector[67]++; break;
                    case "System.Management.Automation.Language.VariableExpressionAst": vector[68]++; break;
                    case "System.Management.Automation.Language.WhileStatementAst": vector[69]++; break;
                    case "System.Management.Automation.Language.ForStatementAst": vector[70]++; break;
                    default: Console.Out.WriteLine(asttype); break;
                }
            }
            for (int i = 0; i < 71; i++)
            {
                featureVectorA.Add(vector[i]);
            }
        }
    }
}
