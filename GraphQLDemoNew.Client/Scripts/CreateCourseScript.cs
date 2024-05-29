using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoNew.Client.Scripts
{
    public class CreateCourseScript
    {
        private readonly IGraphQLDemoNewClient _client;

        public CreateCourseScript(IGraphQLDemoNewClient client)
        {
            _client = client;
        }

        public async Task Run()
        {
            CourseTypeInput courseInput = new CourseTypeInput()
            {
                Name = "ALGEBRA",
                Subject = Subject.Mathematics,
                InstructorId = Guid.Parse("264CEB23670640C89A9D7DB039C6CC7A")
            };

            var createCourseResult = await _client.CreateCourse.ExecuteAsync(courseInput);

            Console.WriteLine(createCourseResult.Errors);
        }
    }
}
