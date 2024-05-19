﻿using FirebaseAdmin.Auth;
using GraphQLDemo.API.DataLoaders;
using GraphQLDemo.API.DTOs;
using GraphQLDemo.API.Models;
using GraphQLDemo.API.Services.Instructors;

namespace GraphQLDemo.API.Schema.Queries;

public class CourseType
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Subject Subject { get; set; }

    [IsProjected(true)]
    public Guid InstructorId { get; set; }

    [GraphQLNonNullType]
    public async Task<InstructorType> Instructor([Service] InstructorDataLoader instructorsDataloader)
    {
        InstructorDTO instructorDTO = await instructorsDataloader.LoadAsync(InstructorId);
        return new InstructorType()
        {
            Id = instructorDTO.Id,
            FirstName = instructorDTO.FirstName,
            LastName = instructorDTO.LastName,
            Salary = instructorDTO.Salary
        };
    }

    public IEnumerable<StudentType> Students { get; set; }

    [IsProjected(true)]
    public string CreatorId { get; set; }

    public async Task<UserType> Creator([Service] UserDataLoader userDataLoader)
    {
        if (CreatorId == null)
            return null;

        return await userDataLoader.LoadAsync(CreatorId, CancellationToken.None);
    }
}
