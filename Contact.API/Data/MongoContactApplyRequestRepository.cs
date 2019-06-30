using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contact.API.Models;

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

        public Task<bool> GetRequestListAsync(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
