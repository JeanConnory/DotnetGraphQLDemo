using GraphQLDemoNew.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StrawberryShake;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        string graphqlApiUrl = context.Configuration.GetValue<string>("GRAPHQL_API_URL");

        string httpGraphQLApiUrl = $"https://{graphqlApiUrl}";
        string webSocketsGraphQLApiUrl = $"ws://{graphqlApiUrl}";

        services
                .AddGraphQLDemoNewClient()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(httpGraphQLApiUrl))
                .ConfigureWebSocketClient(c => c.Uri = new Uri(webSocketsGraphQLApiUrl));

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
        #region Queries

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

        #endregion

        #region Mutation

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

        //var deleteCourseResult = await _client.DeleteCourse.ExecuteAsync(courseId);
        //bool deleteCourseSuccessful = deleteCourseResult.Data.DeleteCourse;
        //if (deleteCourseSuccessful)
        //{
        //    Console.WriteLine("Seccessfully deleted course");
        //}

        #endregion

        #region Subscriptions

        var course = _client.CourseCreated.Watch().Subscribe(result =>
        {
            string name = result.Data.CourseCreated.Name;
            Console.WriteLine($"Course {name} was created");
        });

        _client.CourseUpdated.Watch(courseId).Subscribe(result =>
        {
            string name = result.Data.CourseUpdated.Name;
            Console.WriteLine($"Course {courseId} was renamed to {name}");
        });

        #endregion

        Console.ReadKey();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}