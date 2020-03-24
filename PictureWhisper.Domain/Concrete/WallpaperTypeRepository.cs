using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using PictureWhisper.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    public class WallpaperTypeRepository : IWallpaperTypeRepository
    {
        private DB_PictureWhisperContext context;

        public WallpaperTypeRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        public async Task<T_WallpaperType> QueryAsync(short id)
        {
            return await context.WallpaperTypes.FindAsync(id);
        }

        public async Task<List<T_WallpaperType>> QueryAsync()
        {
            return await context.WallpaperTypes.ToListAsync();
        }

        public async Task<bool> InsertAsync(T_WallpaperType entity)
        {
            context.WallpaperTypes.Add(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> UpdateAsync(short id, JsonPatchDocument<T_WallpaperType> jsonPatch)
        {
            var target = await context.WallpaperTypes.FindAsync(id);
            jsonPatch.ApplyTo(target);
            context.Entry(target).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> DeleteAsync(short id)
        {
            var entity = await context.WallpaperTypes.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            context.WallpaperTypes.Remove(entity);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return false;
            }

            return true;
        }
    }
}
