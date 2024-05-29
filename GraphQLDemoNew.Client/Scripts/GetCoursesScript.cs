using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphQLDemoNew.Client.Scripts
{
    public class GetCoursesScript
    {
        private readonly IGraphQLDemoNewClient _client;

        public GetCoursesScript(IGraphQLDemoNewClient client)
        {
            _client = client;
        }

        public async Task Run()
        {
            ConsoleKey key;

            int? first = 5;
            string after = null;

            int? last = null;
            string before = null;

            do
            {
                var courseResult = await _client.GetCourses.ExecuteAsync(first, after, last, before, null,
                    //new CourseTypeFilterInput()
                    //{
                    //    Subject = new SubjectOperationFilterInput()
                    //    {
                    //        Eq = Subject.ComputerScience
                    //    }
                    //},
                    new List<CourseTypeSortInput>()
                    {
                    new CourseTypeSortInput()
                    {
                        CourseName = SortEnumType.Asc
                    }
                    });

                Console.WriteLine($"{"CourseName",-10} | {"Subject",-10}");
                Console.WriteLine();
                foreach(var course in courseResult.Data.Courses.Nodes)
                {
                    Console.WriteLine($"{course.Name,-10} | {course.Subject,-10}");
                }

                var pageInfo = courseResult.Data.Courses.PageInfo;

                if(pageInfo.HasPreviousPage)
                {
                    Console.WriteLine("Press 'A' to move to the previous page.");
                }
                if(pageInfo.HasNextPage)
                {
                    Console.WriteLine("Press 'D' to move to the previous page.");
                }

                Console.WriteLine("Press 'Enter' to exit.");

                key = Console.ReadKey().Key;

                if(key == ConsoleKey.A && pageInfo.HasPreviousPage)
                {
                    last = 5;
                    before = pageInfo.StartCursor;
                    first = null;
                    after = null;
                }
                else if (key == ConsoleKey.D && pageInfo.HasNextPage)
                {
                    first = 5;
                    after = pageInfo.EndCursor;
                    last = null;
                    before = null;
                }
            }
            while(key != ConsoleKey.Enter);            
        }
    }
}
