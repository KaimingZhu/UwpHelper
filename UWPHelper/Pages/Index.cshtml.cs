using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Models;
using UWPHelper.Services.Implement;
using UWPHelper.Services.Interface;

namespace UWPHelper.Pages
{
    [AllowAnonymous]
    public class IndexModel : SearchCodePageModel
    {
        public IndexModel(UserManager<UWPHelperUser> userManager, SignInManager<UWPHelperUser> signInManager,IDetectManager detectManager,
            IdentityContext identityContext, IAuthorizationService authorizationService, ISourceCodeManager sourceCodeManager) : base(userManager,signInManager,detectManager,identityContext,authorizationService,sourceCodeManager)
        {
            
        }

        [BindProperty(SupportsGet = true)]
        public IList<SourceCodeForDisPlay> sourceCodes { get; set; }

        [BindProperty(SupportsGet = true)]
        public IList<History> HistoryData { get; set; }

        [Required]
        [BindProperty]
        public string SearchCode { get; set; }

        [Required]
        [BindProperty]
        public string SearchCodeName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            //将历史删除与文件删除操作交给Result页面进行处理

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                HistoryData = await _identityContext.HistorySet.Include(r => r.User).Where(r => r.UserID == user.Id).ToListAsync();
            }
            else
            {
                HistoryData = new List<History>();
            }

            sourceCodes = _sourceCodeManager.GetSortedSourceCode();
            return Page();
        }

        private string TimeToString(DateTime Time)
        {
            return Time.Year.ToString() + "_" + Time.Month.ToString() + "_" + Time.Day.ToString() + "-" +
                Time.Hour.ToString() + "_" + Time.Minute.ToString() + "_" + Time.Second.ToString() + "_" + Time.Millisecond.ToString();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            string userid;

            if (user == null)
            {
                userid = null;
            }
            else
            {
                userid = user.Id;
            }
            var TimeNow = DateTime.Now;

            //此处应该先执行保存操作
            //传递给下一个页面文件保存目录的信息
            //历史添加数据交给Result页面执行

            string FolderURL = "";

            if (!Directory.Exists("SearchData\\")){
                Directory.CreateDirectory("SearchData\\");
            }

            //保存数据
            //如果用户存在，则创建文件夹后写入
            if (user != null)
            {

                //先判断文件夹是否存在，若不存在，则创建
                if (!Directory.Exists("SearchData\\" + user.Id))
                {
                    Directory.CreateDirectory("SearchData\\" + user.Id);
                }

                Directory.CreateDirectory("SearchData\\" + user.Id + "\\" + TimeToString(TimeNow) + "\\");

                FolderURL = "SearchData\\" + user.Id + "\\" + TimeToString(TimeNow) + "\\";
            }
            else
            {
                //先判断文件夹是否存在，若不存在，则创建
                if (!Directory.Exists("SearchData\\temp"))
                {
                    Directory.CreateDirectory("SearchData\\temp");
                }

                Directory.CreateDirectory("SearchData\\temp\\" + TimeToString(TimeNow) + "-" + SearchCodeName);

                FolderURL = "SearchData\\temp\\" + TimeToString(TimeNow) + "-" + SearchCodeName + "\\";
            }

            //创建文件
            FileStream fp = new FileStream(FolderURL + "1.cs", FileMode.CreateNew);
            fp.Close();

            //开始写入
            StreamWriter fw = new StreamWriter(FolderURL + "1.cs");
            fw.Write(SearchCode);
            fw.Close();

            //页面跳转
            return RedirectToPage("./Result",new { FolderURL = FolderURL, SourceCodeName = SearchCodeName});
        }
    }
}