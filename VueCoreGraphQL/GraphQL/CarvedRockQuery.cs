using GraphQL.Types;
using VueCoreGraphQL.GraphQL.Types;
using VueCoreGraphQL.Repositories;

namespace VueCoreGraphQL.GraphQL
{
    public class CarvedRockQuery : ObjectGraphType
    {
        public CarvedRockQuery(ProductRepository productRepository, ProductReviewRepository reviewRepository)
        {
            Field<ListGraphType<ProductType>>(      // I want to return a list => ListGraphType
                "products",
                resolve: context => productRepository.GetAll()
                );

            Field<ProductType>(
                "product",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>>  // NOT optional ID argument
                    {Name = "id"}),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("id");    // Get the specific ID from context
                    return productRepository.GetOne(id);
                });

            Field<ListGraphType<ProductReviewType>>(
                "reviews",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> {Name = "productId"}),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("productId");
                    return reviewRepository.GetForProduct(id);
                });
        }
    }
}
