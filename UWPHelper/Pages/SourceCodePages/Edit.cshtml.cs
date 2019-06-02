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
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Models;
using UWPHelper.Services.Interface;

namespace UWPHelper.Pages.SourceCodePages
{
    public class EditModel : PageModel
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IdentityContext _identityContext;
        private readonly UserManager<UWPHelperUser> _userManager;
        private readonly IDetectManager _detectManager;
        private readonly ISourceCodeManager _sourceCodeManager;

        public EditModel(IdentityContext identityContext, UserManager<UWPHelperUser> userManager, IAuthorizationService authorizationService,
            IDetectManager detectManager, ISourceCodeManager sourceCodeManager)
        {
            _identityContext = identityContext;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _detectManager = detectManager;
            _sourceCodeManager = sourceCodeManager;
        }

        [BindProperty(SupportsGet = true)]
        public SourceCodeForDisPlay sourceCodeForDisPlay { get; set; }

        public IActionResult OnGet(int? id)
        {
            //判断是否已授权
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            //查找数据
            int sourceCodeid = (int)id;
            sourceCodeForDisPlay = _sourceCodeManager.FindSourceCode(sourceCodeid);
            if (sourceCodeForDisPlay == null)
            {
                return NotFound();
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {

            //判断是否已授权
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            //判断是否为空
            if ((sourceCodeForDisPlay.SourceCode != null) && (sourceCodeForDisPlay.Name != null))
            {
                sourceCodeForDisPlay.LastEditTime = DateTime.Now;
                //寻找此次修改的元素
                var temp = _sourceCodeManager.FindSourceCode(id);

                //更换名字时，需要判断是否有重名
                if ((temp.Name != sourceCodeForDisPlay.Name) || (temp.EnglishName == sourceCodeForDisPlay.EnglishName))
                {
                    //寻找可能重名的元素
                    var sourceCode1 = _sourceCodeManager.FindSourceCodeForName(sourceCodeForDisPlay.Name);
                    var sourceCode2 = _sourceCodeManager.FindSourceCodeForEnglishName(sourceCodeForDisPlay.EnglishName);
                    if (((sourceCode1.Name == sourceCodeForDisPlay.Name) && (temp.ID != sourceCodeForDisPlay.ID)) || ((sourceCode2.EnglishName == sourceCodeForDisPlay.EnglishName) && (temp.ID != sourceCodeForDisPlay.ID)))
                    {
                        return RedirectToPage("./Index", new { ErrorMessage = "数据库中已有重名代码" });
                    }
                    //同时，若更换名称，可以默认为知识点已更换，重新计数
                    temp.SearchTime = 0;
                }

                //删除原有文件
                FileInfo file = new FileInfo("SourceCodeData//" + temp.GetFileName());
                if (file.Exists)
                {
                    file.Delete();
                }
                //创建新文件
                FileStream fs = new FileStream("SourceCodeData//" + sourceCodeForDisPlay.GetFileName(), FileMode.CreateNew);
                fs.Close();

                //更新FileList
                _detectManager.UpateItemFromInitList(temp.GetFileName(), sourceCodeForDisPlay.GetFileName());

                //更新原有信息
                temp.Name = sourceCodeForDisPlay.Name;
                temp.EnglishName = sourceCodeForDisPlay.EnglishName;
                temp.DocURL = sourceCodeForDisPlay.DocURL;
                temp.LastEditTime = sourceCodeForDisPlay.LastEditTime;

                //更新URL
                string url = "SourceCodeData//" + temp.GetFileName();
               
                //写入新文件 : 输入
                StreamWriter fw = new StreamWriter(url);
                fw.Write(sourceCodeForDisPlay.SourceCode);
                fw.Close();

                await _sourceCodeManager.UpdateSourceCode(temp);

            }
        
            else
            {
                return RedirectToPage("./Index", new { ErrorMessage = "名字与代码不可为空" });
            }

            return RedirectToPage("./Index");
        }
    }
}