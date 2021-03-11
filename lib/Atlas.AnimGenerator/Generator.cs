using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace Atlas.AnimGenerator
{
    [Generator]
    class SourceGenerator : ISourceGenerator
    {
        private HashSet<string> _allowedTypes = new()
        {
            "int?",
            "int",
            "float?",
            "float",
            "Microsoft.Xna.Framework.Vector2",
            "Microsoft.Xna.Framework.Color"
        };

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not NodeReciever reciever) return;

            var methods = new List<string>();

            foreach (var tDec in reciever.Candidates)
            {
                var model = context.Compilation.GetSemanticModel(tDec.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(tDec);

                if (symbol == null) continue;

                var fields = symbol.GetMembers()
                    .Where(s => s.DeclaredAccessibility == Accessibility.Public)
                    .Where(s => s is IFieldSymbol)
                    .Cast<IFieldSymbol>()
                    .Where(f => !f.IsConst && !f.IsOverride && !f.IsStatic && !f.IsStatic)
                    .Select(f => (f.Type, f.Name));

                var properties = symbol.GetMembers()
                    .Where(s => s.DeclaredAccessibility == Accessibility.Public)
                    .Where(s => s is IPropertySymbol)
                    .Cast<IPropertySymbol>()
                    .Where(p => !p.IsReadOnly && !p.IsIndexer && !p.IsStatic && !p.IsOverride)
                    .Select(p => (p.Type, p.Name));

                var items = fields.Concat(properties).Where(i => _allowedTypes.Contains(i.Type.ToString()));
                if (!items.Any()) continue;

                foreach (var item in items)
                {
                    var type = item.Type.ToString().TrimEnd('?');
                    var methodSig = $"public static Animation<{type}> Animate{item.Name}";
                    var methodArgs = $"this {symbol} {symbol.Name.ToLower()}, {type} start, {type} end, float length, bool loop = false, EaseType easeType = EaseType.Linear, bool pingPong = false";
                    var methodBody = $"Animation<{type}>.Create(x => {symbol.Name.ToLower()}.{item.Name} = x, start, end, length, loop, easeType, pingPong)";
                    methods.Add($"{methodSig}({methodArgs}) => {methodBody};");
                }
            }
            var builder = new StringBuilder();
            builder.Append($@"
namespace Atlas.Anim {{
    public static class GeneratedExtensions {{
        {String.Join("\n", methods.Select(method => $"{method}"))}
    }}
}}");

            context.AddSource("GeneratedAnimExtensions.cs", builder.ToString());
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new NodeReciever());
        }

        class NodeReciever : ISyntaxReceiver
        {
            public List<TypeDeclarationSyntax> Candidates { get; } = new();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is TypeDeclarationSyntax tDec)
                {
                    if (tDec.Identifier.ToString() == "Node") Candidates.Add(tDec);
                    else if (tDec.BaseList != null && tDec.BaseList.Types.Where(t => t.Type.ToString() == "Node").Any())
                    {
                        Candidates.Add(tDec);
                    }
                    else if (tDec.AttributeLists.SelectMany(a => a.Attributes).Where(a => a.Name.ToString() == "GenerateAnimation").Any())
                    {
                        Candidates.Add(tDec);
                    }
                }
            }
        }
    }
}
