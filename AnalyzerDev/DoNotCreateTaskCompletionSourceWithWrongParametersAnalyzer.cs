using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;

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
                    // Ensure that a new object is being created
                    if (operationContext.Operation.Kind != OperationKind.ObjectCreation)
                    {
                        return;
                    }

                    // Ensure TaskCompletionSource type is available
                    var taskType = compilationContext.Compilation.GetTypeByMetadataName(WellKnownTypeNames.SystemThreadingTasksTaskCompletionSource);
                    if (taskType == null)
                    {
                        return;
                    }

                    // Ensure that you're using the TaskCompletionSource constructor with the object state property else exit
                    var invocation = (IObjectCreationOperation)operationContext.Operation;
                    if (invocation == null
                    || !invocation.Type.OriginalDefinition.Equals(taskType)
                    || invocation.Arguments.Length != 1
                    || !invocation.Constructor.Parameters[0].Type.Name.Equals(nameof(Object), StringComparison.OrdinalIgnoreCase))
                    {
                        return;
                    }

                    // Check to see if argument is a conversion operator
                    if (invocation.Arguments[0] is IArgumentOperation arg
                    && arg.Value.Kind == OperationKind.Conversion
                    && arg.Value is IConversionOperation conversionOperation)
                    {
                        // Check to see if its a field reference or passed in via reference
                        if (conversionOperation.Operand.Kind == OperationKind.FieldReference
                        || conversionOperation.Operand.Kind == OperationKind.LocalReference)
                        {
                            // Ensure TaskContinuationOptions is available
                            var taskContinutationOptionsType = compilationContext.Compilation.GetTypeByMetadataName(WellKnownTypeNames.SystemThreadingTasksTaskContinuationOptions);
                            if (taskContinutationOptionsType == null)
                            {
                                return;
                            }

                            // If the operand is TaskContinuationOptions then report diagnostic
                            if (conversionOperation.Operand.Type.Equals(taskContinutationOptionsType))
                            {
                                operationContext.ReportDiagnostic(Diagnostic.Create(Rule, conversionOperation.Syntax.GetLocation(), invocation.ToString())); // TODO update last param
                            }
                        }
                    }
                }, OperationKind.ObjectCreation);
                });
        }

        //private void SendDiagnostic(SyntaxNodeAnalysisContext context, ExpressionSyntax exp)
        //{
        //     All tests have passed, report bug
        //    var diagnostic = Diagnostic.Create(Rule, exp.GetLocation());
        //    context.ReportDiagnostic(diagnostic);
        //}

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
