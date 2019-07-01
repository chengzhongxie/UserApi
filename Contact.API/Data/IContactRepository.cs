using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Contact.API.Dtos;

namespace Contact.API.Data
{
    public interface IContactRepository
    {
        /// <summary>
        /// 更新联系人信息
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        Task<bool> UpdateContactInfo(BaseUserInfo userInfo, CancellationToken cancellationToken);
    }
}
