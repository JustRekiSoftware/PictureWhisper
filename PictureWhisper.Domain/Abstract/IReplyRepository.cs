using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    public interface IReplyRepository
    {
        Task<List<T_Reply>> QueryAsync(string type, int id, int page, int pageSize);

        Task<bool> InsertAsync(T_Reply entity);

        Task<bool> DeleteAsync(int id);
    }
}
