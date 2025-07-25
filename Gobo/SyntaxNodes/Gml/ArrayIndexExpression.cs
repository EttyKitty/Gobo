using Gobo.Printer.DocTypes;
using Gobo.SyntaxNodes.PrintHelpers;

namespace Gobo.SyntaxNodes.Gml;

internal sealed class ArrayIndexExpression : GmlSyntaxNode, IMemberChainable
{
    public GmlSyntaxNode Object
    {
        get => BaseExpression;
        set => BaseExpression = AsChild(value);
    }

    public GmlSyntaxNode BaseExpression { get; private set; }
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

    public Doc PrintInChain(PrintContext ctx)
    {
        return Doc.Concat(
            "[",
            IndexExpression.Print(ctx),
            "]"
        );
    }

    public void SetObject(GmlSyntaxNode node)
    {
        Object = node;
    }
}