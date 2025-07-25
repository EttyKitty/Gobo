using Gobo.Printer.DocTypes;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ArrayIndexExpression : GmlSyntaxNode
{
    public GmlSyntaxNode BaseExpression { get; }
    public GmlSyntaxNode IndexExpression { get; }

    public ArrayIndexExpression(TextSpan span, GmlSyntaxNode baseExpression, GmlSyntaxNode indexExpression)
        : base(span)
    {
        BaseExpression = baseExpression;
        IndexExpression = indexExpression;
        AsChildren(new List<GmlSyntaxNode> { baseExpression, indexExpression });
    }

    public override Doc PrintNode(PrintContext ctx)
    {
        return Doc.Concat(
            BaseExpression.Print(ctx),
            "[",
            IndexExpression.Print(ctx),
            "]"
        );
    }
}