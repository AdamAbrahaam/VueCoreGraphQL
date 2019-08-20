using GraphQL.Types;
using VueCoreGraphQL.Data.Entities;

namespace VueCoreGraphQL.GraphQL.Types
{
    public class ProductReviewType : ObjectGraphType<ProductReview>
    {
        public ProductReviewType()
        {
            Field(t => t.Id);
            Field(t => t.Title);
            Field(t => t.Review);
        }
    }
}