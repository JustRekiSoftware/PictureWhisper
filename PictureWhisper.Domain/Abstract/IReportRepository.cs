using Microsoft.AspNetCore.JsonPatch;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    public interface IReportRepository
    {
        Task<T_Report> QueryAsync(int id);

        Task<List<T_Report>> QueryAsync(int page, int pageSize);

        Task<bool> InsertAsync(T_Report entity);
    }
}
