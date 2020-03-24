using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    public interface IWallpaperTypeRepository
    {
        Task<T_WallpaperType> QueryAsync(short id);

        Task<List<T_WallpaperType>> QueryAsync();

        Task<bool> InsertAsync(T_WallpaperType entity);

        Task<bool> UpdateAsync(short id, JsonPatchDocument<T_WallpaperType> entity);

        Task<bool> DeleteAsync(short id);
    }
}
