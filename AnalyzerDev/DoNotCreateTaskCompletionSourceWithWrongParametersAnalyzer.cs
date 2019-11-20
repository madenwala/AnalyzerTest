//using Analyzer.Utilities;
using AnalyzerDev;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AnalyzerDev
{
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public class TaskCompletionSourceEnumAnalyzer : DiagnosticAnalyzer
    {
        public const string RuleId = "TaskCompletionSourceEnum";
        private const string Category = "Naming";

        private static readonly LocalizableString s_localizableTitle = new LocalizableResourceString(nameof(Resources.DoNotCreateTaskCompletionSourceWithWrongParametersTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_localizableMessage = new LocalizableResourceString(nameof(Resources.DoNotCreateTaskCompletionSourceWithWrongParametersMessage), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_localizableDescription = new LocalizableResourceString(nameof(Resources.DoNotCreateTaskCompletionSourceWithWrongParametersDescription), Resources.ResourceManager, typeof(Resources));

        private readonly static DiagnosticDescriptor Rule = new DiagnosticDescriptor(RuleId, s_localizableTitle, s_localizableMessage, Category, DiagnosticSeverity.Error, isEnabledByDefault: true, description: s_localizableDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            // Old C# way to do this
            //context.RegisterSyntaxNodeAction(CSharpAnalyzeSyntax, SyntaxKind.ObjectCreationExpression);


            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(compilationContext =>
            {
                compilationContext.RegisterOperationAction(operationContext =>
                {
                    if (operationContext.Operation.Kind != OperationKind.ObjectCreation)
                        return;

                    var invocation = (IObjectCreationOperation)operationContext.Operation;

                    var type = compilationContext.Compilation.GetTypeByMetadataName(WellKnownTypeNames.SystemThreadingTasksTaskCompletionSource);
                    if (!invocation.Type.OriginalDefinition.Equals(type))
                        return;
                    if (invocation.Arguments.Length != 1)
                        return;
                    if (!invocation.Constructor.Parameters[0].Type.Name.Equals(nameof(Object), StringComparison.OrdinalIgnoreCase))
                        return;
                    var arg = invocation.Arguments[0] as IArgumentOperation;


                    // TODO Replace if-then below to avoid expressions from the C# namespaces 

                    if(arg.Value.Syntax != null)
                    //if (argumentList.Arguments[0].Expression is MemberAccessExpressionSyntax argExpression
                    //    && context.SemanticModel.GetSymbolInfo(argExpression).Symbol is ISymbol argSymbol
                    //    && argSymbol.ContainingType.Name.Equals(nameof(TaskContinuationOptions), StringComparison.OrdinalIgnoreCase))
                    {
                        // TODO uncomment after you get the expression
                        // SendDiagnostic(context, argExpression);
                    }
                    else if(true) // TODO Replace with expression from below
                    //else if (argumentList.Arguments[0].Expression is IdentifierNameSyntax nameExpression
                    //    && context.SemanticModel.GetSymbolInfo(nameExpression).Symbol is ILocalSymbol nameSymbol
                    //    && nameSymbol.Type.Name.Equals(nameof(TaskContinuationOptions), StringComparison.OrdinalIgnoreCase))
                    {
                        // TODO uncomment after you get the expression
                        // TODO SendDiagnostic(context, nameExpression);
                    }

                }, OperationKind.ObjectCreation);
                });
        }

        private void SendDiagnostic(SyntaxNodeAnalysisContext context, ExpressionSyntax exp)
        {
            // All tests have passed, report bug
            var diagnostic = Diagnostic.Create(Rule, exp.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }

        //private void CSharpAnalyzeSyntax(SyntaxNodeAnalysisContext context)
        //{
        //    if (!context.Node.IsKind(SyntaxKind.ObjectCreationExpression))
        //        return;

        //    // Check if new instance is TCS and if the object parameter is being used
        //    var memberSymbol = context.SemanticModel.GetSymbolInfo(context.Node).Symbol as IMethodSymbol;
        //    if (!memberSymbol.ContainingType.Name.Equals("TaskCompletionSource", StringComparison.OrdinalIgnoreCase))
        //        return;
        //    if (memberSymbol.Parameters.Length != 1)
        //        return;
        //    if (!memberSymbol.Parameters[0].Type.Name.Equals(nameof(Object), StringComparison.OrdinalIgnoreCase))
        //        return;

        //    var createExpression = (ObjectCreationExpressionSyntax)context.Node;
        //    var argumentList = createExpression?.ArgumentList as ArgumentListSyntax;
        //    if (argumentList.Arguments.Count != 1)
        //        return;

        //    if (argumentList.Arguments[0].Expression is MemberAccessExpressionSyntax argExpression
        //        && context.SemanticModel.GetSymbolInfo(argExpression).Symbol is ISymbol argSymbol
        //        && argSymbol.ContainingType.Name.Equals(nameof(TaskContinuationOptions), StringComparison.OrdinalIgnoreCase))
        //    {
        //        // Check if the parameter is TaskContinuationOptions enumeration which would end up going to the state 
        //        // parameter and pass compilation. 
        //        SendDiagnostic(context, argExpression);
        //    }
        //    else if (argumentList.Arguments[0].Expression is IdentifierNameSyntax nameExpression
        //        && context.SemanticModel.GetSymbolInfo(nameExpression).Symbol is ILocalSymbol nameSymbol
        //        && nameSymbol.Type.Name.Equals(nameof(TaskContinuationOptions), StringComparison.OrdinalIgnoreCase))
        //    {
        //        // Check if the parameter is variable of type TaskContinuationOptions enumeration which would end up going to the state 
        //        // parameter and pass compilation. 
        //        SendDiagnostic(context, nameExpression);
        //    }
        //}
    }
}
