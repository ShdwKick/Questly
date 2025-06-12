using DataModels.DTOs;
using QuestlyAdmin.Services;

namespace QuestlyAdmin.Mutations;

[ExtendObjectType(typeof(Mutation))]
public class UserMutations
{
    private readonly IUserService _userService;
    
    public UserMutations(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<bool> ChangeUserBlockStatus(BlockUserDTO blockUser)
    {
        return await _userService.ChangeUserBlockStatusAsync(blockUser);
    }
    
    public async Task<string> LoginUser(string login, string password)
    {
        return await _userService.LoginUser(login, password);
    }

}