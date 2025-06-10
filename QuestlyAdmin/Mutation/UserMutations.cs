using DataModels;
using HotChocolate;
using HotChocolate.Types;
using QuestlyAdmin.Repositories;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutations
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    
    public UserMutations(IUserRepository userRepository, IUserService userService)
    {
        _userRepository = userRepository;
        _userService = userService;
    }
    
}