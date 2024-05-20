namespace GraphQLDemo.API.Schema.Queries;

//[InterfaceType("SearchResult")]
[UnionType("SearchResult")]
public interface ISearchResultType
{
    Guid Id { get; }
}
