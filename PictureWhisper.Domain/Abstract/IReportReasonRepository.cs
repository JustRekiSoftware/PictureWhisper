using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    public interface IReportReasonRepository
    {
        Task<string> QueryAsync(short id);

        Task<List<T_ReportReason>> QueryAsync();

        Task<bool> InsertAsync(T_ReportReason entity);

        Task<bool> UpdateAsync(short id, JsonPatchDocument<T_ReportReason> entity);

        Task<bool> DeleteAsync(short id);
    }
}
