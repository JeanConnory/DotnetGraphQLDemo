using GraphQLDemoNew.Client;
using Microsoft.Extensions.DependencyInjection;
using StrawberryShake;

Console.WriteLine("Hello, World!");

var serviceCollection = new ServiceCollection();

serviceCollection
                .AddGraphQLDemoNewClient()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://localhost:7243/graphql"));

IServiceProvider services = serviceCollection.BuildServiceProvider();
IGraphQLDemoNewClient client = services.GetRequiredService<IGraphQLDemoNewClient>();

var result = await client.GetInstructions.ExecuteAsync();

if (result.IsErrorResult())
{
    Console.WriteLine("Failed to get instructions");
}
else
{
    Console.WriteLine(result.Data?.Instructions);
}

Console.ReadKey();