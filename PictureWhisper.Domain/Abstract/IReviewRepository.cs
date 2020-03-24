using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    public interface IReviewRepository
    {
        Task<List<T_Review>> QueryAsync(int id, int page, int pageSize);

        Task<bool> InsertAsync(T_Review entity);

        Task<List<T_Review>> GetReviewMessageAsync(int id, int page, int pageSize);
    }
}
