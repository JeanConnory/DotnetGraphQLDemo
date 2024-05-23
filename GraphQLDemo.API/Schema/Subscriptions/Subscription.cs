using GraphQLDemo.API.Schema.Mutations;
using GraphQLDemo.API.Schema.Queries;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace GraphQLDemo.API.Schema.Subscriptions;

public class Subscription
{
    [Subscribe]
    public CourseResult CourseCreated([EventMessage] CourseResult course) => course;

    [Subscribe]
    public InstructorResult InstructorCreated([EventMessage] InstructorResult instructor) => instructor;

    [SubscribeAndResolve]
    public ValueTask<ISourceStream<CourseResult>> CourseUpdated(Guid courseId, [Service] ITopicEventReceiver topicEventReceiver)
    {
        string topicName = $"{courseId}_{nameof(Subscription.CourseUpdated)}";

        return topicEventReceiver.SubscribeAsync<CourseResult>(topicName);
    }

    [Subscribe]
    [Topic(nameof(CourseMutation.UpdateCourse))]
    public CourseResult CourseUpdateNewVersion(Guid courseId, [EventMessage] CourseResult course)
        => course;
    
}
