﻿using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;

namespace QuestlyAdmin
{
    public class Subsription
    {
        // [Subscribe(With = nameof(SubscribeToMessagesByChatId))]
        // [Topic("Chat_{chatId}")]
        // public Task<Message> OnMessageReceived([EventMessage] Message Message)
        // {
        //     return Task.FromResult(Message);
        // }
        //
        // public ValueTask<ISourceStream<Message>> SubscribeToMessagesByChatId(Guid chatId, [Service] ITopicEventReceiver eventReceiver)
        // {
        //     return eventReceiver.SubscribeAsync<Message>($"Chat_{chatId}");
        // }
        //
        // [Subscribe(With = nameof(SubscribeToRoomUsersListChanged))]
        // [Topic("Room_{chatId}")]
        // public Task<Message> OnRoomUserListChangeReceived([EventMessage] Message Message)
        // {
        //     return Task.FromResult(Message);
        // }
        //
        // public ValueTask<ISourceStream<TripUserListChange>> SubscribeToRoomUsersListChanged(Guid roomId, [Service] ITopicEventReceiver eventReceiver)
        // {
        //     return eventReceiver.SubscribeAsync<TripUserListChange>($"Room_{roomId}");
        // }
    }


}
