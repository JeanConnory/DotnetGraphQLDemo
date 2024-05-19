using FirebaseAdmin;
using FirebaseAdmin.Auth;
using GraphQLDemo.API.Schema.Queries;

namespace GraphQLDemo.API.DataLoaders;

public class UserDataLoader : BatchDataLoader<string, UserType>
{
    public const int MAX_FIREBASE_USERS_BATCH_SIZE = 100;
    private readonly FirebaseAuth _firebaseAuth;

    public UserDataLoader(FirebaseApp firebaseApp, IBatchScheduler batchScheduler) : base(batchScheduler, new DataLoaderOptions()
    {
        MaxBatchSize = MAX_FIREBASE_USERS_BATCH_SIZE
    })
    {
        _firebaseAuth = FirebaseAuth.GetAuth(firebaseApp);
    }

    protected override async Task<IReadOnlyDictionary<string, UserType>> LoadBatchAsync(IReadOnlyList<string> userIds, CancellationToken cancellationToken)
    {
        List<UidIdentifier> userIdentifiers = userIds.Select(u => new UidIdentifier(u)).ToList();
        GetUsersResult userResult = await _firebaseAuth.GetUsersAsync(userIdentifiers);

        return userResult.Users.Select(u => new UserType() 
        { 
            Id = u.Uid,
            Username = u.DisplayName,
            PhotoUrl = u.PhotoUrl
        }).ToDictionary(u => u.Id);
    }
}
