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

namespace UWPHelper.Pages.SourceCodePages
{
    public class CreateModel : PageModel
    {
        private readonly IAuthorizationService _authorizationService;
        private IdentityContext _identityContext { get; }
        private UserManager<UWPHelperUser> _userManager { get; }

        public CreateModel(IdentityContext identityContext,UserManager<UWPHelperUser> userManager, IAuthorizationService authorizationService)
        {
            _identityContext = identityContext;
            _userManager = userManager;
            _authorizationService = authorizationService;
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
                //判断是否有重名
                var temp = await _identityContext.SourceCodes.ToListAsync();
                foreach(var item in temp)
                {
                    if (item.Name.Equals(sourceCodeForDisPlay.Name))
                    {
                        //直接返回
                        return RedirectToPage("./Index",new{ ErrorMessage = "数据库中已有重名代码"});
                    }
                }

                //写入新文件:创建
                string url = "SourceCodeData//" + sourceCodeForDisPlay.Name + ".cs";
                FileStream fp = new FileStream(url, FileMode.CreateNew);
                fp.Close();

                //写入新文件 : 输入
                StreamWriter fw = new StreamWriter(url);
                fw.Write(sourceCodeForDisPlay.SourceCode);
                fw.Close();

                //保存进入数据库
                SourceCode sourceCode = new SourceCode
                {
                    Name = sourceCodeForDisPlay.Name,
                    FileUrl = url,
                    LastEditDate = DateTime.Now,
                    SearchTime = 0,
                    DocURL = sourceCodeForDisPlay.DocURL
                };

                _identityContext.SourceCodes.Add(sourceCode);

                await _identityContext.SaveChangesAsync();
            }
            else
            {
                return RedirectToPage("./Create", new { ErrorMessage = "请输入名字与代码" });
            }

            return RedirectToPage("./Index", new { ErrorMessage = "" });
        }
    }
}