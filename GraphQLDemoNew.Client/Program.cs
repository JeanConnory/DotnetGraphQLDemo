using Firebase.Auth;
using GraphQLDemoNew.Client;
using GraphQLDemoNew.Client.Scripts;
using GraphQLDemoNew.Client.Stores;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StrawberryShake;
using System.Net.Http.Headers;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        string graphqlApiUrl = context.Configuration.GetValue<string>("GRAPHQL_API_URL");

        string httpGraphQLApiUrl = $"https://{graphqlApiUrl}";
        string webSocketsGraphQLApiUrl = $"ws://{graphqlApiUrl}";

        services
                .AddGraphQLDemoNewClient()
                .ConfigureHttpClient((services, c) =>
                {
                    c.BaseAddress = new Uri(httpGraphQLApiUrl);
                    TokenStore tokenStore = services.GetService<TokenStore>();
                    c.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenStore.AccessToken);
                })
                .ConfigureWebSocketClient(c => c.Uri = new Uri(webSocketsGraphQLApiUrl));

        services.AddHostedService<Startup>();

        services.AddSingleton<TokenStore>();

        string firebaseApiKey = context.Configuration.GetValue<string>("FIREBASE_API_KEY");
        services.AddSingleton(new FirebaseAuthProvider(new FirebaseConfig(firebaseApiKey)));

        services.AddTransient<GetCoursesScript>();
        services.AddTransient<CreateCourseScript>();
        services.AddTransient<LoginScript>();
    })
    .Build()
    .Run();


public class Startup : IHostedService
{
    private readonly LoginScript _loginScript;
    private readonly CreateCourseScript _createCourseScript;

    public Startup(CreateCourseScript createCourseScript, LoginScript loginScript)
    {
        _createCourseScript = createCourseScript;
        _loginScript = loginScript;
    }

    //private readonly GetCoursesScript _getCoursesScript;
    //public Startup(GetCoursesScript getCoursesScript)
    //{
    //    _getCoursesScript = getCoursesScript;
    //}

    //private readonly IGraphQLDemoNewClient _client;
    //public Startup(IGraphQLDemoNewClient client)
    //{
    //    _client = client;
    //}

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        #region Queries

        await _loginScript.Run();
        await _createCourseScript.Run();

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

        //var createCourseResult = await _client.CreateCourse.ExecuteAsync(new CourseTypeInput()
        //{
        //    Name = "GraphQL 101",
        //    Subject = Subject.Science,
        //    InstructorId = Guid.NewGuid()
        //});
        //Guid courseId = createCourseResult.Data.CreateCourse.Id;
        //string createdCourseName = createCourseResult?.Data.CreateCourse.Name;
        //Console.WriteLine($"Successfully created course {createdCourseName}");

        //var updateCourseResult = await _client.UpdateCourse.ExecuteAsync(courseId, new CourseTypeInput()
        //{
        //    Name = "GraphQL 102",
        //    Subject = Subject.Science,
        //    InstructorId = Guid.NewGuid()
        //});

        //if (updateCourseResult.IsErrorResult())
        //{
        //    IClientError error = updateCourseResult.Errors.First();
        //    if (error.Code == "COURSE_NOT_FOUND")
        //    {
        //        Console.WriteLine("Course was not found");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Unknown course update error");
        //    }
        //}
        //else
        //{
        //    string updatedCourseName = updateCourseResult?.Data.UpdateCourse.Name;
        //    Console.WriteLine($"Successfully created course {updatedCourseName}");
        //}

        //var deleteCourseResult = await _client.DeleteCourse.ExecuteAsync(courseId);
        //bool deleteCourseSuccessful = deleteCourseResult.Data.DeleteCourse;
        //if (deleteCourseSuccessful)
        //{
        //    Console.WriteLine("Seccessfully deleted course");
        //}

        #endregion

        #region Subscriptions

        //_client.CourseCreated.Watch().Subscribe(result =>
        //{
        //    string name = result.Data.CourseCreated.Name;
        //    Console.WriteLine($"Course {name} was created");
        //});

        //_client.CourseUpdated.Watch(courseId).Subscribe(result =>
        //{
        //    string name = result.Data.CourseUpdated.Name;
        //    Console.WriteLine($"Course {courseId} was renamed to {name}");
        //});

        #endregion

        Console.ReadKey();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}