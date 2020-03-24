using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Newtonsoft.Json.Linq;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Domain.Abstract
{
    public interface IUserRepository
    {
        Task<T_User> QueryAsync(int id);

        Task<List<T_User>> QueryAsync(string queryData, int page, int pageSize);

        Task<bool> CheckEmailAsync(string email);

        Task<bool> CheckNameAsync(string name);

        Task<UserSigninDto> CheckSigninAsync(string email, string password);

        Task<UserSigninDto> InsertAsync(T_User entity);

        Task<bool> UpdateAsync(int id, JsonPatchDocument<T_User> entity);

        Task<bool> DeleteAsync(int id);
    }
}
