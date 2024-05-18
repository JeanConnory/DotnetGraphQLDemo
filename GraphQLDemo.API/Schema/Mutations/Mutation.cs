using FirebaseAdminAuthentication.DependencyInjection.Models;
using GraphQLDemo.API.DTOs;
using GraphQLDemo.API.Schema.Queries;
using GraphQLDemo.API.Schema.Subscriptions;
using GraphQLDemo.API.Services.Course;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;
using System.Security.Claims;

namespace GraphQLDemo.API.Schema.Mutations;

public class Mutation
{
    private readonly CoursesRepository _coursesRepository;

    public Mutation(CoursesRepository coursesRepository)
    {
        _coursesRepository = coursesRepository;
    }

    [Authorize]
    public async Task<CourseResult> CreateCourse(CourseInputType courseInput, [Service] ITopicEventSender topicEventSender, ClaimsPrincipal claimsPrincipal)
    {
        string userId = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.ID);
        string email = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.EMAIL);
        string username = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.USERNAME);
        string verified = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.EMAIL_VERIFIED);

        CourseDTO courseDTO = new CourseDTO()
        {
            Name = courseInput.Name,
            Subject = courseInput.Subject,
            InstructorId = courseInput.InstructorId
        };

        courseDTO = await _coursesRepository.Create(courseDTO);

        CourseResult course = new CourseResult()
        {
            Id = courseDTO.Id,
            Name = courseDTO.Name,
            Subject = courseDTO.Subject,
            InstructorId = courseDTO.InstructorId
        };

        await topicEventSender.SendAsync(nameof(Subscription.CourseCreated), course);

        return course;
    }

    [Authorize]
    public async Task<CourseResult> UpdateCourse(Guid id, CourseInputType courseInput, [Service] ITopicEventSender topicEventSender, CancellationToken cancellationToken)
    {
        CourseDTO courseDTO = new CourseDTO()
        {
            Id = id,
            Name = courseInput.Name,
            Subject = courseInput.Subject,
            InstructorId = courseInput.InstructorId
        };

        courseDTO = await _coursesRepository.Update(courseDTO);

        CourseResult course = new CourseResult()
        {
            Id= courseDTO.Id,
            Name = courseDTO.Name,
            Subject = courseDTO.Subject,
            InstructorId = courseDTO.InstructorId
        };

        string updateCourseTopic = $"{course.Id}_{nameof(Subscription.CourseUpdated)}";
        await topicEventSender.SendAsync(updateCourseTopic, course);
        //await topicEventSender.SendAsync(nameof(UpdateCourse), course, cancellationToken); Versão Nova

        return course;
    }

    [Authorize]
    public async Task<bool> DeleteCourse(Guid id)
    {
        try
        {
            return await _coursesRepository.Delete(id);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
