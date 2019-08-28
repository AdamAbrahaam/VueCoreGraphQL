using GraphQL.Types;

namespace VueCoreGraphQL.GraphQL.Types
{
    public class ProductReviewInputType : InputObjectGraphType
    {
        // GraphQL makes a clear distinction between data that is going out and data coming in
        // For mutations we need InputGraphType as the mutation argument, GraphType is then sent back when successful 
        public ProductReviewInputType()
        {
            Name = "reviewInput";
            Field<NonNullGraphType<StringGraphType>>("title");
            Field<StringGraphType>("review");
            Field<NonNullGraphType<IntGraphType>>("productId");
        }
    }
}