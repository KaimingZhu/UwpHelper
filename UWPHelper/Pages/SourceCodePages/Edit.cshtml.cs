using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Models;

namespace UWPHelper.Pages.SourceCodePages
{
    public class EditModel : PageModel
    {
        private readonly IdentityContext _identityContext;
        private readonly UserManager<UWPHelperUser> _userManager;

        public EditModel(IdentityContext identityContext, UserManager<UWPHelperUser> userManager)
        {
            _identityContext = identityContext;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public SourceCodeForDisPlay sourceCodeForDisPlay { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            //检查授权

            //查找数据
            var sourceCode = await _identityContext.SourceCodes.FindAsync(id);

            if(sourceCode == null)
            {
                return NotFound();
            }

            sourceCodeForDisPlay = new SourceCodeForDisPlay(sourceCode.ID, sourceCode.name, sourceCode.FileUrl);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {

            //检查授权

            //判断是否为空
            if ((sourceCodeForDisPlay.SourceCode != null) && (sourceCodeForDisPlay.Name != null))
            {

                //寻找需要更新的数据
                var sourceCode = _identityContext.SourceCodes.FirstOrDefault(m => m.ID == id);

                //更换名字时，需要判断是否有重名
                if (sourceCode.name != sourceCodeForDisPlay.Name)
                {
                    var temp = _identityContext.SourceCodes.FirstOrDefault(m => m.name == sourceCodeForDisPlay.Name);
                    if (temp != null)
                    {
                        return RedirectToPage("./Index", new { ErrorMessage = "数据库中已有重名代码" });
                    }
                }

                //删除原有文件，保存新文件
                FileInfo file = new FileInfo(sourceCode.FileUrl);
                if (file.Exists)
                {
                    file.Delete();
                }

                //更新URL
                string url = "SourceCodeData//" + sourceCodeForDisPlay.Name + ".cs";
                //创建新文件
                FileStream fs = new FileStream(url, FileMode.CreateNew);
                fs.Close();

                //写入新文件 : 输入
                StreamWriter fw = new StreamWriter(url);
                fw.Write(sourceCodeForDisPlay.SourceCode);
                fw.Close();

                //更新名称与URL
                sourceCode.name = sourceCodeForDisPlay.Name;
                sourceCode.FileUrl = url;

                await _identityContext.SaveChangesAsync();

            }
            else
            {
                return RedirectToPage("./Index", new { ErrorMessage = "名字与代码不可为空" });
            }

            return RedirectToPage("./Index");
        }
    }
}