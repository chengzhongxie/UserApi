﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Dtos;
using Contact.API.Models;
using MongoDB.Driver;

namespace Contact.API.Data
{
    public class MongoContactRepository : IContactRepository
    {
        private readonly ContactContext _contactContext;

        public MongoContactRepository(ContactContext contactContext)
        {
            _contactContext = contactContext;
        }
        public async Task<bool> UpdateContactInfo(BaseUserInfo userInfo, CancellationToken cancellationToken)
        {
            var contactBook = (await _contactContext.ContactBooks.FindAsync(c => c.UserId.ToString() == userInfo.UserId, null, cancellationToken)).FirstOrDefault(cancellationToken);
            if (contactBook == null)
            {
                return true;
                //throw new Exception($"没有找到相关数据：{userInfo.UserId}");
            }
            var contactIds = contactBook.Contacts.Select(c => c.UserId);

            // 查询需要修改的数据
            var filter = Builders<ContactBook>.Filter.And(
                Builders<ContactBook>.Filter.In(m => m.UserId, contactIds),
                Builders<ContactBook>.Filter.ElemMatch(m => m.Contacts, contacts => contacts.UserId.ToString() == userInfo.UserId)
                );

            // 设置需要修改的数据
            var update = Builders<ContactBook>.Update
                .Set("Contacts.$.Name", userInfo.Name)
                .Set("Contacts.$.Avatar", userInfo.Avatar)
                .Set("Contacts.$.Company", userInfo.Company)
                .Set("Contacts.$.Title", userInfo.Title);
            // 修改数据
            var updateResult = _contactContext.ContactBooks.UpdateMany(filter, update);// _contactContext.ContactBooks.UpdateOne() 修改全部数据

            return updateResult.MatchedCount == updateResult.MatchedCount;

        }
    }
}
