using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContactManager.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Models;
using UWPHelper.Services.Interface;

namespace UWPHelper.Pages.SourceCodePages
{
    public class CreateModel : PageModel
    {
        private readonly IAuthorizationService _authorizationService;
        private IdentityContext _identityContext { get; }
        private UserManager<UWPHelperUser> _userManager { get; }
        private readonly IDetectManager _detectManager;
        private readonly ISourceCodeManager _sourceCodeManager;

        public CreateModel(IdentityContext identityContext,UserManager<UWPHelperUser> userManager, IAuthorizationService authorizationService,
            IDetectManager detectManager, ISourceCodeManager sourceCodeManager)
        {
            _identityContext = identityContext;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _detectManager = detectManager;
            _sourceCodeManager = sourceCodeManager;
        }

        private string TimeToString(DateTime Time)
        {
            return Time.Year.ToString() + "_" + Time.Month.ToString() + "_" + Time.Day.ToString() + "-" +
                Time.Hour.ToString() + "_" + Time.Minute.ToString() + "_" + Time.Second.ToString() + "_" + Time.Millisecond.ToString();
        }

        [BindProperty(SupportsGet = true)]
        public SourceCodeForDisPlay sourceCodeForDisPlay { get; set; }

        public IActionResult OnGet()
        {
            //判断是否已授权
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);

            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            sourceCodeForDisPlay = new SourceCodeForDisPlay();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //判断是否已授权
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            //加入数据库，并且添加本地文件
            //判断是否为空
            if (sourceCodeForDisPlay.SourceCode != null && sourceCodeForDisPlay.Name != null)
            {
                if (!Directory.Exists("SourceCodeData")){
                    Directory.CreateDirectory("SourceCodeData");
                }

                //先写文件
                var time = DateTime.Now;
                string url = "SourceCodeData//" + TimeToString(time) + "-" + sourceCodeForDisPlay.EnglishName + ".cs";
                string filename = TimeToString(time) + "-" + sourceCodeForDisPlay.EnglishName + ".cs";
                FileStream fp = new FileStream(url, FileMode.CreateNew);
                fp.Close();

                //写入新文件 : 输入
                StreamWriter fw = new StreamWriter(url);
                fw.Write(sourceCodeForDisPlay.SourceCode);
                fw.Close();

                //保存进入数据库
                sourceCodeForDisPlay.LastEditTime = time;
                sourceCodeForDisPlay.SearchTime = 0;
                await _sourceCodeManager.AddSourceCode(sourceCodeForDisPlay);

                //在List中添加这一项信息
                _detectManager.AddItemtoInitList(filename);
            }
            else
            {
                return RedirectToPage("./Create", new { ErrorMessage = "请输入名字与代码" });
            }

            return RedirectToPage("./Index", new { ErrorMessage = "" });
        }
    }
}