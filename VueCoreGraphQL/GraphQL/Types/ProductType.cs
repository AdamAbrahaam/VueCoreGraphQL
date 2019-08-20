using System.Security.Claims;
using GraphQL.DataLoader;
using GraphQL.Types;
using VueCoreGraphQL.Data.Entities;
using VueCoreGraphQL.Repositories;

namespace VueCoreGraphQL.GraphQL.Types
{
    // Query can't return entities or models directly
    public class ProductType : ObjectGraphType<Product>    // Meta data for product entity
    {
        public ProductType(ProductReviewRepository reviewRepository, IDataLoaderContextAccessor dataLoaderAccessor)
        {
            // Basic fields, GraphQL can map them
            Field(t => t.Id);
            Field(t => t.Name).Description("The name of the product."); // .Description() adds description for the field
            Field(t => t.Description);
            Field(t => t.IntroducedAt);
            Field(t => t.PhotoFileName);
            Field(t => t.Price);
            Field(t => t.Rating);
            Field(t => t.Stock);

            // Enums, scalar types (int, string)
            Field<ProductTypeEnumType>("Type", "The type of product");  // Name, desc shown in schema explorer

            // Complex types (classes), loads reviews for each product
            Field<ListGraphType<ProductReviewType>>(
                "reviews",
                resolve: context =>
                {
                    var user = (ClaimsPrincipal)context.UserContext;    // (ClaimsPrincipal) => just changing type
                    // user. ...    // authorization

                    // Cashes data so we don't have many unnecessary queries
                    // DataLoader needs to be added in Startup config
                    // GetOrAddCollectionBatchLoader will crate a new or use an existing data loader by name
                    // data loader uses a dictionary to cash data, has INT as a key and ProductReview object as a value
                    // Gets data through method GetForProducts from repository
                    var loader = dataLoaderAccessor.Context.GetOrAddCollectionBatchLoader<int, ProductReview>(
                        "GetReviewsByProductId", reviewRepository.GetForProducts);

                    return loader.LoadAsync(context.Source.Id);     // Source if the Product entity
                });
        }
    }
}
