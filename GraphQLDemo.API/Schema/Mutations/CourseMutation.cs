using AppAny.HotChocolate.FluentValidation;
using FirebaseAdminAuthentication.DependencyInjection.Models;
using FluentValidation.Results;
using GraphQLDemo.API.DTOs;
using GraphQLDemo.API.Middlewares.UseUser;
using GraphQLDemo.API.Models;
using GraphQLDemo.API.Schema.Subscriptions;
using GraphQLDemo.API.Services.Course;
using GraphQLDemo.API.Validators;
using HotChocolate.Authorization;
using HotChocolate.Subscriptions;
using System.Security.Claims;

namespace GraphQLDemo.API.Schema.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class CourseMutation
{
    private readonly CoursesRepository _coursesRepository;
    //private readonly CourseTypeInputValidator _courseTypeInputValidator;

    public CourseMutation(CoursesRepository coursesRepository)//, CourseTypeInputValidator courseTypeInputValidator)
    {
        _coursesRepository = coursesRepository;
        //_courseTypeInputValidator = courseTypeInputValidator;
    }

    [Authorize]
    //[UseUser]
    public async Task<CourseResult> CreateCourse([UseFluentValidation, UseValidator<CourseTypeInputValidator>] CourseTypeInput courseInput, 
        [Service] ITopicEventSender topicEventSender,
       ClaimsPrincipal claimsPrincipal) // [User] User user
    {
        //Validate(courseInput);

        string userId = claimsPrincipal.FindFirstValue(FirebaseUserClaimType.ID);
        //string userId = user.Id;

        CourseDTO courseDTO = new CourseDTO()
        {
            Name = courseInput.Name,
            Subject = courseInput.Subject,
            InstructorId = courseInput.InstructorId,
            CreatorId = userId
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

    //private void Validate(CourseInputType courseInput)
    //{
    //    ValidationResult validationResult = _courseTypeInputValidator.Validate(courseInput);

    //    if (!validationResult.IsValid)
    //    {
    //        throw new GraphQLException("Invalid input");
    //    }
    //}

    [Authorize]
    [UseUser]
    public async Task<CourseResult> UpdateCourse(Guid id, [UseFluentValidation, UseValidator<CourseTypeInputValidator>] CourseTypeInput courseInput, [Service] ITopicEventSender topicEventSender,
        [User] User user) //ClaimsPrincipal claimsPrincipal
    {
        //Validate(courseInput);

        string userId = user.Id;

        CourseDTO courseDTO = await _coursesRepository.GetById(id);

        if(courseDTO == null)
        {
            throw new GraphQLException(new Error("Course not found.", "COURSE_NOT_FOUND"));
        }

        if(courseDTO.CreatorId != userId)
        {
            throw new GraphQLException(new Error("You do not have permission to update this course.", "INVALID_PERMISSION"));
        }

        courseDTO.Name = courseInput.Name;
        courseDTO.Subject = courseInput.Subject;
        courseDTO.InstructorId = courseInput.InstructorId;        

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

    [Authorize(Policy = "IsAdmin")]
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
