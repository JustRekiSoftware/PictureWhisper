using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    public interface IFavoriteRepository
    {
        Task<bool> QueryAsync(int favoritorId, int wallpaperId);

        Task<List<T_Wallpaper>> QueryAsync(int id, int page, int pageSize);

        Task<bool> InsertAsync(T_Favorite entity);

        Task<bool> DeleteAsync(int favoritorId, int wallpaperId);
    }
}
