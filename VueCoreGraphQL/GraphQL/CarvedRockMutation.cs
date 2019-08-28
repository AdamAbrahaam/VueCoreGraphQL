using System;
using GraphQL;
using GraphQL.Types;
using VueCoreGraphQL.Data.Entities;
using VueCoreGraphQL.GraphQL.Types;
using VueCoreGraphQL.Repositories;

namespace VueCoreGraphQL.GraphQL
{
    public class CarvedRockMutation : ObjectGraphType
    {
        // Adding, updating, deleting data
        public CarvedRockMutation(ProductReviewRepository reviewRepository, ProductRepository productRepository)
        {
            // ProductReviewType is going to be returned when mutation is done
            FieldAsync<ProductReviewType>(
                "createReview",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<ProductReviewInputType>> {Name = "review"}),
                resolve: async context =>
                {
                    // Conversion from GraphType to Entity
                    var review = context.GetArgument<ProductReview>("review");

                    // The outcome of AddReview is monitored, exception will be catched and added to the error list even when the API is configured to not expose exceptions 
                    return await context.TryAsyncResolve(
                        async c => await reviewRepository.AddReview(review));
                });

            Field<StringGraphType>(
                "deleteProduct",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "productId" }),
                resolve: context =>
                {
                    var productId = context.GetArgument<int>("productId");
                    var product = productRepository.GetOne(productId).Result;
                    if (product == null)
                    {
                        context.Errors.Add(new ExecutionError("Couldn't find product in db."));
                        return null;
                    }

                    productRepository.Delete(product);
                    return $"The owner with the id: {productId} has been successfully deleted from db.";
                }
            );
        }
    }
}