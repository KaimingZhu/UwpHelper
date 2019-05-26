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

namespace UWPHelper.Pages.SourceCodePages
{
    public class DeleteModel : PageModel
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly IdentityContext _identityContext;
        private readonly UserManager<UWPHelperUser> _userManager;

        public DeleteModel(IdentityContext identityContext, UserManager<UWPHelperUser> userManager, IAuthorizationService authorizationService)
        {
            _identityContext = identityContext;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [BindProperty(SupportsGet = true)]
        public SourceCodeForDisPlay sourceCodeForDisPlay { get; set; }

        public IActionResult OnGet(int ?id)
        {

            //判断是否已授权
            var isAuthorized = User.IsInRole(Constants.ContactAdministratorsRole);
            if (!isAuthorized)
            {
                return new ChallengeResult();
            }

            var sourceCode = _identityContext.SourceCodes.FirstOrDefault(r => r.ID == id);
            if(sourceCode == null)
            {
                return NotFound();
            }

            sourceCodeForDisPlay = new SourceCodeForDisPlay(sourceCode);

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
            var sourceCode = _identityContext.SourceCodes.FirstOrDefault(r => r.ID == id);

            if (sourceCode == null)
            {
                return NotFound();
            }

            //删除文件
            FileInfo file = new FileInfo(sourceCode.FileUrl);
            if (file.Exists)
            {
                file.Delete();
            }

            //删除对应的数据库记录
            _identityContext.SourceCodes.Remove(sourceCode);
            await _identityContext.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}