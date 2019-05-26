using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Models;

namespace UWPHelper.Pages.Shared
{
    [AllowAnonymous]
    public class ResultModel : PageModel
    {

        private readonly UserManager<UWPHelperUser> _userManager;
        private readonly SignInManager<UWPHelperUser> _signInManager;
        private readonly IdentityContext _identityContext;

        public ResultModel(UserManager<UWPHelperUser> userManager, SignInManager<UWPHelperUser> signInManager, IdentityContext identityContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identityContext = identityContext;
        }

        [BindProperty(SupportsGet = true)]
        public string User { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Time { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IfRealUser { get; set; }

        [BindProperty]
        public string SearchCode { get; set; }

        [BindProperty]
        public bool ifFind { get; set; }

        public IActionResult OnGet()
        {
            //在前端实现绑定后，在此处实现文件读取
            ReadFile();
            //呈现到前端绑定的部分上
            if (ifFind == false)
            {
                return RedirectToPage("./Index", new { SearchUser = User, Time, IfRealUser = (User != null),ifFind });
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            return RedirectToPage("./Index", new { SearchUser = User, ifFind });
        }

        private bool IfFileExist()
        {
            bool ifRunOut = false;
            FileInfo fp;
            if(!IfRealUser)
            {
                fp = new FileInfo("SearchData\\temp\\" + Time + "_result.txt");
            }
            else
            {
                fp = new FileInfo("SearchData\\" + User + "\\" + Time + "_result.txt");
            }
            DateTime begin = DateTime.Now;
            while (!fp.Exists)
            {
                DateTime now = DateTime.Now;
                if ((now - begin).Seconds >= 30)
                {
                    //若超出两分钟时间，记为超时
                    ifRunOut = true;
                    break;
                }
            }
            if (ifRunOut == true)
            {
                //若超时，记为查找失败，返回对应页面
                ifFind = false;
                return false;
            }
            else
            {
                ifFind = true;
                return true;
            }
        }

        private void ReadFile()
        {
            //保持读取
            //判断文件是否存在
            var temp = IfFileExist();
            //若存在，开始读取并绑定
            if(temp == true)
            {
                StreamReader sr;
                if(!IfRealUser)
                {
                    sr = new StreamReader("SearchData\\temp\\" + Time + "_result.txt");
                }
                else
                {
                    sr = new StreamReader("SearchData\\" + User + "\\" + Time + "_result.txt");
                }

                SearchCode = sr.ReadToEnd();
                sr.Close();
            }
        }
    }
}