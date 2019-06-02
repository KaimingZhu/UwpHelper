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
using Microsoft.EntityFrameworkCore;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Model;
using UWPHelper.Models;
using UWPHelper.Services.Implement;
using UWPHelper.Services.Interface;

namespace UWPHelper.Pages.Shared
{
    [AllowAnonymous]
    public class ResultModel : SearchCodePageModel
    {


        public ResultModel(UserManager<UWPHelperUser> userManager, SignInManager<UWPHelperUser> signInManager, IDetectManager detectManager,
            IdentityContext identityContext, IAuthorizationService authorizationService, ISourceCodeManager sourceCodeManager) 
            : base(userManager, signInManager, detectManager, identityContext, authorizationService, sourceCodeManager)
        {

        }

        [BindProperty(SupportsGet = true)]
        public bool ifSave { get; set; }

        [BindProperty(SupportsGet = true)]
        public string FolderURL { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SourceCodeName { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<DetectResult> detectResults { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<DisplayResult> displayResults { get; set; }

        [BindProperty(SupportsGet = true)]
        public int id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            //判断是否进行历史搜索
            if (string.IsNullOrEmpty(FolderURL))
            {
                var userNow = await _userManager.GetUserAsync(User);
                if(userNow == null)
                {
                    return new NotFoundResult();
                }
                var HistoryData = await _identityContext.HistorySet.Include(r => r.User).Where(r => r.UserID == userNow.Id).ToListAsync();
                foreach(var item in HistoryData)
                {
                    if(item.ID == id)
                    {
                        FolderURL = item.GetFolderURL();
                    }
                }
            }
            else
            {
                id = int.MinValue;
            }


            //执行流程 : 
            //先执行检测
            string file;
            bool ifCorrent = await _detectManager.DetectBetweenFileListAsync(FolderURL);

            //检测完之后获得结果
            detectResults = await _detectManager.AnalyseAsync(FolderURL, _sourceCodeManager.GetSourceCodeForDisPlays());
            
            //若是用户 : 保存历史数据
            var user = await _userManager.GetUserAsync(User);

            if(user != null)
            {
                StreamReader sr = new StreamReader(FolderURL + "1.cs");
                file = sr.ReadToEnd();
                sr.Close();
            }
            else
            {
                StreamReader sr = new StreamReader(FolderURL + "1.cs");
                file = sr.ReadToEnd();
                sr.Close();
            }

            int i;
            for (i = 0; i < detectResults.Count - 1;i++)
            {
                if(detectResults[i].EndRow < detectResults[i].BeginRow)
                {
                    detectResults.Remove(detectResults[i]);
                    i--;
                    continue;
                }
                if(detectResults[i].EndRow == detectResults[i + 1].BeginRow)
                {
                    detectResults[i + 1].BeginRow++;
                }
            }
            if (detectResults[i].EndRow < detectResults[i].BeginRow)
            {
                detectResults.Remove(detectResults[i]);
            }

            //呈现对应数据
            int row = 1;
            int pos = 0;
            string tempString = "";
            int color = 0;
            foreach(var item in detectResults)
            {
                if(pos == file.Length)
                {
                    break;
                }
                //跳到对应的起始行
                tempString = "";
                while(row != item.BeginRow)
                {
                    if (pos == file.Length)
                    {
                        break;
                    }
                    var ch = file[pos++];
                    if (ch == '\n')
                    {
                        row++;
                    }
                    tempString += ch;
                }
                if(tempString.Length > 0)
                {
                    displayResults.Add(new DisplayResult(tempString, "", color %= 3));
                }

                if (pos == file.Length)
                {
                    break;
                }
                tempString = "";
                //读到终止行
                while (row != item.EndRow)
                {
                    if (pos == file.Length)
                    {
                        break;
                    }
                    var ch = file[pos++];
                    if (ch == '\n')
                    {
                        row++;
                    }
                    tempString += ch;
                }
                if (tempString.Length > 0)
                {
                    color++;
                    displayResults.Add(new DisplayResult(tempString, item.CloneFileName, color %= 3));
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ifSave)
            {
                var user = await _userManager.GetUserAsync(User);
                bool firstTime = true;
                int i = 0;
                int j = 0;
                for(;j < FolderURL.Length;)
                {
                    if(FolderURL[i] == '\\')
                    {
                        if (firstTime)
                        {
                            firstTime = false;
                        }
                        else
                        {
                            break;
                        }
                    }
                    i++;
                    j++;
                }
                j++;
                for(;FolderURL[j] != '\\'; j++)
                {
                    ;
                }
                i++;

                string time = FolderURL.Substring(i, j - i);

                List<string> stringList = new List<string>();
                string tempString = "";
                for(i = 0;i < time.Length; i++)
                {
                    if(time[i] == '_' || time[i] == '-')
                    {
                        stringList.Add(tempString);
                        tempString = "";
                    }
                    else
                    {
                        tempString += time[i];
                    }
                }
                stringList.Add(tempString);

                DateTime dateTime = new DateTime(int.Parse(stringList[0]), int.Parse(stringList[1]), int.Parse(stringList[2]), 
                    int.Parse(stringList[3]), int.Parse(stringList[4]), int.Parse(stringList[5]),int.Parse(stringList[6]));
                _identityContext.HistorySet.Add(new History()
                {
                    Name = SourceCodeName,
                    User = user,
                    UserID = user.Id,
                    AddTime = dateTime
                });
                await _identityContext.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}