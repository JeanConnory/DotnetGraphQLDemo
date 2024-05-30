namespace GraphQLDemoNew.Client.Scripts;

public class SearchScript
{
    private readonly IGraphQLDemoNewClient _client;

    public SearchScript(IGraphQLDemoNewClient client)
    {
        _client = client;
    }

    public async Task Run()
    {
        Console.WriteLine("Enter a search term:");
        string term = Console.ReadLine();

        var searchResult = await _client.Search.ExecuteAsync(term);

        IEnumerable<ISearch_Search_CourseType> courses = searchResult.Data.Search.OfType<ISearch_Search_CourseType>();
        Console.WriteLine("COURSES");
        foreach (ISearch_Search_CourseType course in courses)
        {
            Console.WriteLine(course.Name);
        }

        IEnumerable<ISearch_Search_InstructorType> instructors = searchResult.Data.Search.OfType<ISearch_Search_InstructorType>();
        Console.WriteLine("INSTRUCTORS");
        foreach (ISearch_Search_InstructorType instructor in instructors)
        {
            Console.WriteLine($"{instructor.FirstName} {instructor.LastName}");
        }
        Console.WriteLine();
    }
}
