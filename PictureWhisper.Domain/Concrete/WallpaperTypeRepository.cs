using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    /// <summary>
    /// 壁纸分区数据仓库
    /// </summary>
    public class WallpaperTypeRepository : IWallpaperTypeRepository
    {
        private DB_PictureWhisperContext context;//数据库连接实例

        public WallpaperTypeRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 根据Id获取壁纸分区信息
        /// </summary>
        /// <param name="id">壁纸分区Id</param>
        /// <returns>返回壁纸分区信息</returns>
        public async Task<T_WallpaperType> QueryAsync(short id)
        {
            return await context.WallpaperTypes.FindAsync(id);
        }

        /// <summary>
        /// 获取壁纸分区列表
        /// </summary>
        /// <returns>返回壁纸分区列表</returns>
        public async Task<List<T_WallpaperType>> QueryAsync()
        {
            return await context.WallpaperTypes.ToListAsync();
        }

        /// <summary>
        /// 添加壁纸分区
        /// </summary>
        /// <param name="entity">壁纸分区信息</param>
        /// <returns>添加成功，返回true；失败则返回false</returns>
        public async Task<bool> InsertAsync(T_WallpaperType entity)
        {
            context.WallpaperTypes.Add(entity);
            try
            {
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 更新壁纸分区
        /// </summary>
        /// <param name="id">壁纸分区Id</param>
        /// <param name="jsonPatch">用于更新的JsonPatchDocument</param>
        /// <returns>更新成功，则返回true；失败，则返回false</returns>
        public async Task<bool> UpdateAsync(short id, JsonPatchDocument<T_WallpaperType> jsonPatch)
        {
            var target = await context.WallpaperTypes.FindAsync(id);
            jsonPatch.ApplyTo(target);//应用更新
            context.Entry(target).State = EntityState.Modified;//标记为已修改
            try
            {
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 删除壁纸分区
        /// </summary>
        /// <param name="id">壁纸分区Id</param>
        /// <returns>删除成功，则返回ture；失败，则返回false</returns>
        public async Task<bool> DeleteAsync(short id)
        {
            var entity = await context.WallpaperTypes.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            context.WallpaperTypes.Remove(entity);//移除壁纸分区
            try
            {
                await context.SaveChangesAsync();//保存更改
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }
    }
}
