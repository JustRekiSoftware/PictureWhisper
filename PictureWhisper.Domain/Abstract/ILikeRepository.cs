using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    public interface ILikeRepository
    {
        Task<bool> QueryAsync(int userId, int wallpaperId);

        Task<bool> InsertAsync(T_Like entity);

        Task<bool> DeleteAsync(int userId, int wallpaperId);
    }
}
