using GraphQLDemoNew.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StrawberryShake;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        string graphqlApiUrl = context.Configuration.GetValue<string>("GRAPHQL_API_URL");

        services
                .AddGraphQLDemoNewClient()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(graphqlApiUrl));

        services.AddHostedService<Startup>();
    })
    .Build()
    .Run();


public class Startup : IHostedService
{
    private readonly IGraphQLDemoNewClient _client;

    public Startup(IGraphQLDemoNewClient client)
    {
        _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        //var result = await _client.GetCoursesRepository.ExecuteAsync();

        //if (result.IsErrorResult())
        //{
        //    Console.WriteLine("Failed to get instructions");
        //}
        //else
        //{
        //    foreach (var course in result.Data?.CoursesRepository)
        //    {
        //        Console.WriteLine($"{course.Name} is good! Instructor: {course.Instructor.FirstName}");
        //    }
        //}

        //Console.WriteLine();

        //var courseByIdResult = await _client.GetCourseById.ExecuteAsync(Guid.Parse("db850da5-9ac6-4274-abf7-fb2f24b783de"));

        //if (result.IsErrorResult())
        //{
        //    Console.WriteLine("Failed to get course");
        //}
        //else
        //{
        //    var course = courseByIdResult.Data?.CourseById;
        //    Console.WriteLine($"Course name: {course.Name} Instructor: {course.Instructor.FirstName}");
        //}

        var createCourseResult = await _client.CreateCourse.ExecuteAsync(new CourseTypeInput()
        {
            Name = "GraphQL 101",
            Subject = Subject.Science,
            InstructorId = Guid.NewGuid()
        });
        Guid courseId = createCourseResult.Data.CreateCourse.Id;
        string createdCourseName = createCourseResult?.Data.CreateCourse.Name;
        Console.WriteLine($"Successfully created course {createdCourseName}");

        var updateCourseResult = await _client.UpdateCourse.ExecuteAsync(courseId, new CourseTypeInput()
        {
            Name = "GraphQL 102",
            Subject = Subject.Science,
            InstructorId = Guid.NewGuid()
        });

        if (updateCourseResult.IsErrorResult())
        {
            IClientError error = updateCourseResult.Errors.First();
            if (error.Code == "COURSE_NOT_FOUND")
            {
                Console.WriteLine("Course was not found");
            }
            else
            {
                Console.WriteLine("Unknown course update error");
            }
        }
        else
        {
            string updatedCourseName = updateCourseResult?.Data.UpdateCourse.Name;
            Console.WriteLine($"Successfully created course {updatedCourseName}");
        }

        var deleteCourseResult = await _client.DeleteCourse.ExecuteAsync(courseId);
        bool deleteCourseSuccessful = deleteCourseResult.Data.DeleteCourse;
        if (deleteCourseSuccessful)
        {
            Console.WriteLine("Seccessfully deleted course");
        }

        Console.ReadKey();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}