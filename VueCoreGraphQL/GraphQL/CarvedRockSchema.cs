using GraphQL;
using GraphQL.Types;

namespace VueCoreGraphQL.GraphQL
{
    public class CarvedRockSchema : Schema
    {
        public CarvedRockSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<CarvedRockQuery>();    // Query => data retrieval
            Mutation = resolver.Resolve<CarvedRockMutation>();  // Mutation => adding, updating, deleting data
        }
    }
}
