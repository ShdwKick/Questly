using HotChocolate;
using QuestlyAdmin.Repositories;

namespace QuestlyAdmin.Mutations
{
    //TODO: разнести методы по отдельным классам наследникам
    public class Mutation
    {
        private readonly IUserRepository _userRepository;
        
        public Mutation(IUserRepository userRepository)        {
            _userRepository = userRepository;
        }
    }
}