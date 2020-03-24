using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    public interface IFollowRepository
    {
        Task<bool> QueryAsync(int followerId, int followedId);

        Task<List<T_User>> QueryAsync(int id, int page, int pageSize);

        Task<bool> InsertAsync(T_Follow entity);

        Task<bool> DeleteAsync(int followerId, int followedId);
    }
}
