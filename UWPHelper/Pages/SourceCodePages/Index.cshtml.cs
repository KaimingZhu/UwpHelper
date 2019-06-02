using System;
using System.Collections.Generic;
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
    public class IndexModel : PageModel
    {
        private IAuthorizationService _authorizationService;
        private readonly ISourceCodeManager _sourceCodeManager;
        private UserManager<UWPHelperUser> _userManager { get; }
        private readonly IDetectManager _detectManager;

        public IndexModel(ISourceCodeManager sourceCodeManager, UserManager<UWPHelperUser> userManager, 
            IAuthorizationService authorizationService, IDetectManager detectManager)
        {
            _sourceCodeManager = sourceCodeManager;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _detectManager = detectManager;
        }

        [BindProperty(SupportsGet = true)]
        public IList<SourceCodeForDisPlay> sourceCodes_ForDisPlay { get; set; }

        public IActionResult OnGetAsync()
        {
            //判断是否已授权
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            //错误信息返回

            //读取数据
            sourceCodes_ForDisPlay = _sourceCodeManager.GetSourceCodeForDisPlays();

            return Page();
        }

        public async Task GetSourceCodeForDisPlayAsync()
        {
            var SourceCodes = _sourceCodeManager.GetSourceCodeForDisPlays();

            foreach (var item in SourceCodes){
                if (item.ifExist)
                {
                    sourceCodes_ForDisPlay.Add(item);
                }
                else
                {
                    //如果对应的.cs文件不存在的话: 删除数据库记录，并且删除对应的FileList记录项
                    await _sourceCodeManager.RemoveSourceCode(item);
                    _detectManager.DeleteItemFromInitList(item.GetFileName());
                }
            }
        }
    }
}