using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using UWPHelper.Areas.Identity.Data;
using UWPHelper.Models;
using UWPHelper.Services.Implement;
using UWPHelper.Services.Interface;

namespace UWPHelper.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly UserManager<UWPHelperUser> _userManager;
        private readonly SignInManager<UWPHelperUser> _signInManager;
        private readonly IdentityContext _identityContext;
        private readonly IAuthorizationService _authorizationService;
        private readonly ISourceCodeManager _sourceCodeManager;
        private readonly IDetectManager _detectManager;

        public IndexModel(UserManager<UWPHelperUser> userManager, SignInManager<UWPHelperUser> signInManager, 
            IdentityContext identityContext, IAuthorizationService authorizationService,ISourceCodeManager sourceCodeManager,IDetectManager detectManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _identityContext = identityContext;
            _authorizationService = authorizationService;
            _sourceCodeManager = sourceCodeManager;
            _detectManager = detectManager;
        }

        private string userid { get; set; }

        [BindProperty(SupportsGet = true)]
        private UWPHelperUser user { get; set; }

        [BindProperty(SupportsGet = true)]
        public IList<History> HistoryData { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchUser { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Time { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IfRealUser { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool ifFind { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchCode { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {

            //判断是否从结果页面跳转回来
            if (SearchUser != null)
            {
                if (!ifFind)
                {
                    //若未找到，删除临时文件与对应的记录，并且将其绑定到SearchCode上
                    if (IfRealUser)
                    {
                        //若是注册用户，需要删除数据库项中对应的记录
                        //只需选出最晚的记录即可

                        //选出最早的数据
                        var history = HistoryData[0];
                        foreach (var item in HistoryData)
                        {
                            if (item.AddTime > history.AddTime)
                            {
                                history = item;
                            }
                        }
                        //删除数据
                        var temp = await _identityContext.HistorySet.FindAsync(history.ID);

                        if (temp != null)
                        {
                            _identityContext.Remove(temp);
                        }

                        await _identityContext.SaveChangesAsync();
                    }

                    //读取对应的文件，并且进行绑定
                    StreamReader sr;
                    if (!IfRealUser)
                    {
                        sr = new StreamReader("SearchData\\temp\\" + Time + ".txt");
                    }
                    else
                    {
                        sr = new StreamReader("SearchData\\" + SearchUser + "\\" + Time + ".txt");
                    }

                    SearchCode = sr.ReadToEnd();
                    sr.Close();


                    /** 删除缓存的文件 **/
                    if (IfRealUser)
                    {
                        FileInfo fp_search = new FileInfo("SearchData\\" + SearchUser + "\\" + Time + ".txt");
                        if (fp_search.Exists)
                        {
                            fp_search.Delete();
                        }
                        FileInfo fp_result = new FileInfo("SearchData\\" + SearchUser + "\\" + Time + "_result.txt");
                        if (fp_result.Exists)
                        {
                            fp_result.Delete();
                        }
                    }
                    else
                    {
                        FileInfo fp_search = new FileInfo("SearchData\\temp\\" + Time + ".txt");
                        if (fp_search.Exists)
                        {
                            fp_search.Delete();
                        }
                        FileInfo fp_result = new FileInfo("SearchData\\temp\\" + Time + "_result.txt");
                        if (fp_result.Exists)
                        {
                            fp_result.Delete();
                        }
                    }
                }

                TempData["alertMessage"] = "Sorry, the time has ran out, maybe you can try again.";
            }
            else
            {
                TempData["alertMessage"] = null;
            }

            user = await _userManager.GetUserAsync(User);
            if(user != null)
            {
                HistoryData = await _identityContext.HistorySet.Include(r => r.User).Where(r => r.UserID == user.Id).ToListAsync();
            }
            else
            {
                HistoryData = new List<History>();
            }

            return Page();
        }

        private string TimeToString(DateTime Time)
        {
            return Time.Year.ToString() + Time.Month.ToString() + Time.Day.ToString() + "-" + 
                Time.Hour.ToString() + Time.Minute.ToString() +Time.Second.ToString() + Time.Millisecond.ToString();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(user == null)
            {
                userid = null;
            }
            else
            {
                userid = user.Id;
            }
            var TimeNow = DateTime.Now;

            //此处应该先执行保存操作
            //传递给下一个页面时间戳+id的信息
            if (user != null)
            {
                var amount = HistoryData.Count;
                //先检查是否已有15个数据
                if (amount == 15)
                {
                    //选出最早的数据
                    var history = HistoryData[0];
                    foreach (var item in HistoryData)
                    {
                        if (item.AddTime < history.AddTime)
                        {
                            history = item;
                        }
                    }
                    //删除数据
                    var temp = await _identityContext.HistorySet.FindAsync(history.ID);

                    if (temp != null)
                    {
                        _identityContext.Remove(temp);
                    }

                    /** 删除缓存的文件 **/
                    FileInfo fp_search = new FileInfo("SearchData\\" + user.Id + "\\" + TimeToString(temp.AddTime) + ".txt");
                    if (fp_search.Exists)
                    {
                        fp_search.Delete();
                    }
                    FileInfo fp_result = new FileInfo("SearchData\\" + user.Id + "\\" + TimeToString(temp.AddTime) + "_result.txt");
                    if (fp_result.Exists)
                    {
                        fp_result.Delete();
                    }
                }
            }

            //保存数据
            //如果用户存在，则创建文件夹后写入
            if (user != null)
            {

                TempData["alertMessage"] = null;

                if (SearchCode != null)
                {

                    History newData = new History
                    {
                        User = user,
                        UserID = user.Id,
                        AddTime = TimeNow,
                        Data = SearchCode
                    };

                    _identityContext.HistorySet.Add(newData);

                    //先判断文件夹是否存在，若不存在，则创建
                    if (!Directory.Exists("SearchData\\" + user.Id )){
                        Directory.CreateDirectory("SearchData\\" + user.Id);
                    }

                    //创建文件
                    FileStream fp = new FileStream("SearchData//" + user.Id +"//" + TimeToString(TimeNow) + ".txt", FileMode.CreateNew);
                    fp.Close();

                    //开始写入
                    StreamWriter fw = new StreamWriter("SearchData//" + user.Id + "//" + TimeToString(TimeNow) + ".txt");
                    fw.Write(SearchCode);
                    fw.Close();
                }
                else
                {
                    /** 需要修改 **/
                    TempData["alertMessage"] = "Please Enter the Code you Want to Search";
                    return RedirectToPage("./Index");
                }
            }
            else
            {
                if (SearchCode != null)
                {

                    TempData["alertMessage"] = null;

                    //先判断文件夹是否存在，若不存在，则创建
                    if (!Directory.Exists("SearchData\\temp"))
                    {
                        Directory.CreateDirectory("SearchData\\temp");
                    }

                    //创建文件
                    FileStream fp = new FileStream("SearchData//temp//" + TimeToString(TimeNow) + ".txt", FileMode.CreateNew);
                    fp.Close();
                    //开始写入
                    StreamWriter fw = new StreamWriter("SearchData//temp//" + TimeToString(TimeNow) + ".txt");
                    fw.Write(SearchCode);
                    fw.Close();
                }
                else
                {
                    /** 需要修改 **/
                    TempData["alertMessage"] = "Please Enter the Code you Want to Search";
                    return RedirectToPage("./Index");
                }
            }

            await _identityContext.SaveChangesAsync();

            //页面跳转
            return RedirectToPage("./Result", new { User = userid, Time = TimeToString(TimeNow), IfRealUser = (user != null) });
        }

        private IActionResult View()
        {
            throw new NotImplementedException();
        }
    }
}
