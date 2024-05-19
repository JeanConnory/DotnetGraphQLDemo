using GraphQLDemo.API.DTOs;
using GraphQLDemo.API.Schema.Filters;
using GraphQLDemo.API.Schema.Sorters;
using GraphQLDemo.API.Services;
using GraphQLDemo.API.Services.Course;

namespace GraphQLDemo.API.Schema.Queries
{
    public class Query
    {
        private readonly CoursesRepository _coursesRepository;

        public Query(CoursesRepository coursesRepository)
        {
            _coursesRepository = coursesRepository;
        }

        public async Task<IEnumerable<CourseType>> GetCoursesRepository()
        {
            IEnumerable<CourseDTO> courseDTO = await _coursesRepository.GetAll();

            return courseDTO.Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId
            });
        }

        [UseDbContext(typeof(SchoolDbContext))]
        [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10)]
        [UseProjection]
        [UseFiltering(typeof(CourseFilterType))]
        [UseSorting(typeof(CourseSortType))]
        public IQueryable<CourseType> GetCourses([ScopedService] SchoolDbContext context)
        {
            return context.Courses.Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId,
                CreatorId = c.CreatorId
            });
        }

        [UseOffsetPaging(IncludeTotalCount = true, DefaultPageSize = 10)]
        public async Task<IEnumerable<CourseType>> GetOffsetCourses()
        {
            IEnumerable<CourseDTO> courseDTO = await _coursesRepository.GetAll();

            return courseDTO.Select(c => new CourseType()
            {
                Id = c.Id,
                Name = c.Name,
                Subject = c.Subject,
                InstructorId = c.InstructorId,
                CreatorId = c.CreatorId
            });
        }

        public async Task<CourseType> GetCourseByIdAsync(Guid id)
        {
            CourseDTO courseDTO = await _coursesRepository.GetById(id);

            return new CourseType()
            {
                Id = courseDTO.Id,
                Name = courseDTO.Name,
                Subject = courseDTO.Subject,
                InstructorId = courseDTO.InstructorId,
                CreatorId = courseDTO.CreatorId
            };
        }


        [GraphQLDeprecated("This query is deprecated")]
        public string Instructions => "Hello World!";
    }
}
