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
                var objectType = compilationContext.Compilation.GetTypeByMetadataName(WellKnownTypeNames.SystemObject);
                if (objectType is null)
                    return;
                var stringType = compilationContext.Compilation.GetTypeByMetadataName(WellKnownTypeNames.SystemString);
                if (stringType is null)
                    return;
                var sbType = compilationContext.Compilation.GetTypeByMetadataName(WellKnownTypeNames.SystemTextStringBuilder);
                if (sbType is null)
                    return;
                var tcsType = compilationContext.Compilation.GetTypeByMetadataName(WellKnownTypeNames.SystemThreadingTasksTaskCompletionSource);
                if (tcsType is null)
                    return;
                var taskContinutationOptionsType = compilationContext.Compilation.GetTypeByMetadataName(WellKnownTypeNames.SystemThreadingTasksTaskContinuationOptions);
                if (tcsType is null)
                    return;

                compilationContext.RegisterOperationAction(operationContext =>
                {
                    // Ensure that you're using the TaskCompletionSource constructor with the object state property else exit
                    var objectCreation = (IObjectCreationOperation)operationContext.Operation;
                    if (!objectCreation.Type.OriginalDefinition.Equals(tcsType) ||
                        objectCreation.Arguments.Length == 0 ||
                        !objectCreation.Constructor.Parameters[0].Type.Equals(objectType))
                    {
                        return;
                    }

                    // Check to see if argument is a conversion operator
                    if (!(objectCreation.Arguments[0].Value is IConversionOperation conversionOperation))
                    {
                        return;
                    }

                    // Check to see if its an enum field or local
                    if (conversionOperation.Operand.Kind != OperationKind.FieldReference &&
                        conversionOperation.Operand.Kind != OperationKind.LocalReference &&
                        conversionOperation.Operand.Kind != OperationKind.PropertyReference)
                    {
                        return;
                    }

                    // If the operand is TaskContinuationOptions then report diagnostic
                    if (!conversionOperation.Operand.Type.Equals(taskContinutationOptionsType))
                    {
                        return;
                    }

                    operationContext.ReportDiagnostic(Diagnostic.Create(Rule, objectCreation.Arguments[0].Syntax.GetLocation()));

                }, OperationKind.ObjectCreation);
            });
        }
    }
}
