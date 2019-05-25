using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        private IdentityContext _identityContext { get; }
        private UserManager<UWPHelperUser> _userManager { get; }

        public CreateModel(IdentityContext identityContext,UserManager<UWPHelperUser> userManager)
        {
            _identityContext = identityContext;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public SourceCodeForDisPlay sourceCodeForDisPlay { get; set; }

        public IActionResult OnGet()
        {
            //判断是否已授权

            sourceCodeForDisPlay = new SourceCodeForDisPlay();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //判断是否已授权

            //加入数据库，并且添加本地文件
            //判断是否为空
            if (sourceCodeForDisPlay.SourceCode != null && sourceCodeForDisPlay.Name != null)
            {
                //判断是否有重名
                var temp = await _identityContext.SourceCodes.ToListAsync();
                foreach(var item in temp)
                {
                    if (item.name.Equals(sourceCodeForDisPlay.Name))
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
                    name = sourceCodeForDisPlay.Name,
                    FileUrl = url
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