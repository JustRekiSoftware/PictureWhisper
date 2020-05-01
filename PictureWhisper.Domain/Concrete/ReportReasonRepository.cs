using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Concrete
{
    public class ReportReasonRepository : IReportReasonRepository
    {
        private DB_PictureWhisperContext context;

        public ReportReasonRepository(DB_PictureWhisperContext context)
        {
            this.context = context;
        }

        public async Task<string> QueryAsync(short id)
        {
            var reportReason = await context.ReportReasons.FindAsync(id);

            return reportReason.RR_Info;
        }

        public async Task<List<T_ReportReason>> QueryAsync()
        {
            return await context.ReportReasons.ToListAsync();
        }

        public async Task<bool> InsertAsync(T_ReportReason entity)
        {
            context.ReportReasons.Add(entity);
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

        public async Task<bool> UpdateAsync(short id, JsonPatchDocument<T_ReportReason> jsonPatch)
        {
            var target = await context.ReportReasons.FindAsync(id);
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
            var entity = await context.ReportReasons.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            context.ReportReasons.Remove(entity);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
