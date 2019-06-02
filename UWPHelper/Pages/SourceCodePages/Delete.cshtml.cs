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
    public class DeleteModel : PageModel
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IdentityContext _identityContext;
        private readonly UserManager<UWPHelperUser> _userManager;
        private readonly IDetectManager _detectManager;
        private readonly ISourceCodeManager _sourceCodeManager;

        public DeleteModel(IdentityContext identityContext, UserManager<UWPHelperUser> userManager, IAuthorizationService authorizationService,
            ISourceCodeManager sourceCodeManager,IDetectManager detectManager)
        {
            _identityContext = identityContext;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _detectManager = detectManager;
            _sourceCodeManager = sourceCodeManager;
        }

        [BindProperty(SupportsGet = true)]
        public SourceCodeForDisPlay sourceCodeForDisplay { get; set; }

        public IActionResult OnGet(int ?id)
        {

            //判断是否已授权
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            int sourceCodeid = (int)id;
            sourceCodeForDisplay = _sourceCodeManager.FindSourceCode(sourceCodeid);
            if(sourceCodeForDisplay == null)
            {
                return NotFound();
            }

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

            //确认对应代码
            sourceCodeForDisplay = _sourceCodeManager.FindSourceCode(id);

            if (sourceCodeForDisplay == null)
            {
                return NotFound();
            }

            //删除FileList中的记录
            _detectManager.DeleteItemFromInitList(sourceCodeForDisplay.GetFileName());

            //删除文件
            FileInfo file = new FileInfo("SourceCodeData//" + sourceCodeForDisplay.GetFileName());
            if (file.Exists)
            {
                file.Delete();
            }

            //删除对应的数据库记录
            await _sourceCodeManager.RemoveSourceCode(sourceCodeForDisplay);

            return RedirectToPage("./Index");
        }
    }
}