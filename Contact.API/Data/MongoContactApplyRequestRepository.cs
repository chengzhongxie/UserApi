﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Models;
using MongoDB.Driver;

namespace Contact.API.Data
{
    public class MongoContactApplyRequestRepository : IContactApplyRequestRepository
    {
        private readonly ContactContext _contactContext;

        public MongoContactApplyRequestRepository(ContactContext contactContext)
        {
            _contactContext = contactContext;
        }

        public Task<bool> AddRequstAsync(ContactApplyRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ApprovalAsync(string applierId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<ContactApplyRequest>> GetRequestListAsync(string userId, CancellationToken cancellationToken)
        {
            return (await _contactContext.ContactApplyRequests.FindAsync(r => r.UserId.ToString() == userId)).ToList(cancellationToken);
        }
    }
}
