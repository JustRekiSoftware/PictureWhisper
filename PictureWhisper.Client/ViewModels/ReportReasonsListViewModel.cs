using Newtonsoft.Json.Linq;
using PictureWhisper.Client.Helper;
using PictureWhisper.Domain.Abstract;
using PictureWhisper.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureWhisper.Client.ViewModels
{
    /// <summary>
    /// 举报理由列表的ViewModel
    /// </summary>
    public class ReportReasonsListViewModel
    {
        public ObservableCollection<T_ReportReason> ReportReasons { get; set; }

        public ReportReasonsListViewModel()
        {
            ReportReasons = new ObservableCollection<T_ReportReason>();
        }

        /// <summary>
        /// 获取举报理由列表
        /// </summary>
        /// <returns></returns>
        public async Task GetReportReasonsAsync()
        {
            ReportReasons.Clear();
            using (var client = await HttpClientHelper.GetAuthorizedHttpClientAsync())
            {
                //获取举报理由列表
                var url = string.Format("{0}report/reason",
                    HttpClientHelper.baseUrl);
                var response = await client.GetAsync(new Uri(url));
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var reasons = JArray.Parse(
                    await response.Content.ReadAsStringAsync());
                if (!response.IsSuccessStatusCode)
                {
                    return;
                }
                var result = reasons.ToObject<List<T_ReportReason>>();
                if (result == null)
                {
                    return;
                }
                foreach (var reason in result)
                {
                    this.ReportReasons.Add(reason);
                }
            }
        }
    }
}
