using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contact.API.Data;
using DotNetCore.CAP;
using Contact.API.IntegrationEvents.Events;
using System.Threading;

namespace Contact.API.IntegrationEvents.EventHandling
{
    public class UserProfileChangedEventHandler : ICapSubscribe
    {
        private readonly IContactRepository _contactRepository;

        public UserProfileChangedEventHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }
        [CapSubscribe("finbook.userapi.userprofilechanged")]
        public async Task UpdateContactInfo(UserProfileChangedEvent @event)
        {
            var token = new CancellationToken();
            await _contactRepository.UpdateContactInfoAsync(new Dtos.UserIdentity
            {
                Avatar = @event.Avatar,
                Company = @event.Company,
                Name = @event.Name,
                Title = @event.Title,
                UserId = @event.UserId
            }, token);
        }
    }
}
