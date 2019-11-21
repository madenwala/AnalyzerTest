using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Immutable;
using System.Text;

namespace AnalyzerDev
{
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public class StringBuilderAppendArgumentsAnalyzer : DiagnosticAnalyzer
    {
        public const string RuleId = "StringBuilderAppendArguments";
        private const string Category = "Naming";

        private static readonly LocalizableString s_localizableTitle = new LocalizableResourceString(nameof(Resources.StringBuilderAppendArgumentsTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_localizableMessage = new LocalizableResourceString(nameof(Resources.StringBuilderAppendArgumentsMessage), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString s_localizableDescription = new LocalizableResourceString(nameof(Resources.StringBuilderAppendArgumentsDescription), Resources.ResourceManager, typeof(Resources));

        private readonly static DiagnosticDescriptor Rule = new DiagnosticDescriptor(RuleId, s_localizableTitle, s_localizableMessage, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: s_localizableDescription);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterCompilationStartAction(compilationContext =>
            {
                //TryGetOrCreateTypeByMetadataName

                //// Ensure necessary types are available
                //if (!compilationContext.Compilation.TryGetOrCreateTypeByMetadataName(WellKnownTypeNames.SystemString, out var stringType) ||
                //    !compilationContext.Compilation.TryGetOrCreateTypeByMetadataName(WellKnownTypeNames.SystemTextStringBuilder, out var sbType) ||
                //    !compilationContext.Compilation.TryGetOrCreateTypeByMetadataName(WellKnownTypeNames.SystemThreadingTasksTaskContinuationOptions, out var taskContinutationOptionsType))
                //{
                //    return;
                //}

                var stringType = compilationContext.Compilation.GetTypeByMetadataName(WellKnownTypeNames.SystemString);
                if (stringType is null)
                    return;
                var sbType = compilationContext.Compilation.GetTypeByMetadataName(WellKnownTypeNames.SystemTextStringBuilder);
                if (sbType is null)
                    return;

                compilationContext.RegisterOperationAction(operationContext =>
                {
                    // Ensure that you're using the StringBuilder constructor with initial string property being set
                    var invocation = (IObjectCreationOperation)operationContext.Operation;
                    if (!invocation.Type.OriginalDefinition.Equals(sbType) ||
                        invocation.Arguments.Length == 0 ||
                        !invocation.Constructor.Parameters[0].Type.Equals(stringType))
                    {
                        return;
                    }

                    //if (!(invocation.Arguments[0] is IArgumentOperation arg) ||
                    //    arg.Value.Kind != OperationKind.Literal ||
                    //    !(arg.Value is ILiteralOperation operation))
                    //{
                    //    return;
                    //}
                    var arg = invocation.Arguments[0] as IArgumentOperation;

                    if (arg != null && 
                        arg.Value.Kind == OperationKind.Literal && 
                        arg.Value is ILiteralOperation literalOperation &&
                        literalOperation.ConstantValue.HasValue &&
                        literalOperation.ConstantValue.Value is string &&
                        literalOperation.ConstantValue.Value.ToString().Length <= 1)
                    {
                        operationContext.ReportDiagnostic(Diagnostic.Create(Rule, arg.Syntax.GetLocation()));
                    }
                    else if(arg != null &&
                            arg.Value.Kind != OperationKind.Conversion &&
                            arg.Value is IConversionOperation conversionOperation)
                    {
                        operationContext.ReportDiagnostic(Diagnostic.Create(Rule, arg.Syntax.GetLocation()));
                    }

                    //// Check to see if argument is a conversion operator
                    //if (!(invocation.Arguments[0] is IArgumentOperation arg) ||
                    //    arg.Value.Kind != OperationKind.Conversion ||
                    //    !(arg.Value is IConversionOperation conversionOperation))
                    //{
                    //    return;
                    //}

                    //// Check to see if its an enum field or local
                    //if (operation.Operand.Kind != OperationKind.FieldReference &&
                    //    operation.Operand.Kind != OperationKind.LocalReference)
                    //{
                    //    return;
                    //}

                    //// If the operand is string
                    //if (!operation.Operand.Type.Equals(stringType))
                    //{
                    //    return;
                    //}

                    // TODO Check to see if the string is length 1


                }, OperationKind.ObjectCreation);
            });
        }
    }
}