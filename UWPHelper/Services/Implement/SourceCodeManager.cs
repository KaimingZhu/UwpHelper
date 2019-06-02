using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UWPHelper.Models;
using UWPHelper.Services.Interface;

namespace UWPHelper.Services.Implement
{
    public class SourceCodeManager : ISourceCodeManager
    {
        private readonly IdentityContext _identityContext;

        public SourceCodeManager(IdentityContext identityContext)
        {
            _identityContext = identityContext;
        }

        //检查重名信息
        public bool VaildationCheck(SourceCodeForDisPlay temp)
        {
            SourceCode sourceCode;
            //检查昵称
            sourceCode = _identityContext.SourceCodes.FirstOrDefault(m => m.Name == temp.Name);
            if(sourceCode != null)
            {
                return false;
            }
            //检查英文名
            sourceCode = _identityContext.SourceCodes.FirstOrDefault(m => m.EnglishName == temp.EnglishName);
            if (sourceCode != null)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> AddSourceCode(SourceCodeForDisPlay temp)
        {
            if (!VaildationCheck(temp))
            {
                return false;
            }

            _identityContext.SourceCodes.Add(new SourceCode(temp,false));
            await _identityContext.SaveChangesAsync();

            return true;
        }

        public SourceCodeForDisPlay FindSourceCode(int id)
        {
            var sourceCode = _identityContext.SourceCodes.FirstOrDefault(m => m.ID == id);

            return new SourceCodeForDisPlay(sourceCode);
        }

        public List<SourceCodeForDisPlay> GetSourceCodeForDisPlays()
        {
            List<SourceCodeForDisPlay> result = new List<SourceCodeForDisPlay>();
            var sourceCodes = from s in _identityContext.SourceCodes select s;
            foreach(var item in sourceCodes)
            {
                result.Add(new SourceCodeForDisPlay(item));
            }
            return result;
        }

        public SourceCodeForDisPlay FindSourceCode(string FileName)
        {
            List<SourceCodeForDisPlay> result = new List<SourceCodeForDisPlay>();
            var sourceCodes = from s in _identityContext.SourceCodes select s;
            foreach (var item in sourceCodes)
            {
                result.Add(new SourceCodeForDisPlay(item));
            }

            foreach(var item in result)
            {
                if (item.GetFileName().Equals(FileName))
                {
                    return item;
                }
            }
            return null;
        }

        public SourceCodeForDisPlay FindSourceCodeForName(string name)
        {
            var sourceCode = _identityContext.SourceCodes.FirstOrDefault(m => m.Name== name);
            return new SourceCodeForDisPlay(sourceCode);
        }

        public SourceCodeForDisPlay FindSourceCodeForEnglishName(string EnglishName)
        {
            var sourceCode = _identityContext.SourceCodes.FirstOrDefault(m => m.EnglishName == EnglishName);
            return new SourceCodeForDisPlay(sourceCode);
        }

        public async Task<bool> UpdateSourceCode(SourceCodeForDisPlay temp)
        {
            var sourceCode = _identityContext.SourceCodes.FirstOrDefault(m => m.ID == temp.ID);
            if (sourceCode == null)
            {
                return false;
            }
            else
            {
                sourceCode.LastEditDate = temp.LastEditTime;
                sourceCode.Name = temp.Name;
                sourceCode.EnglishName = temp.EnglishName;
                sourceCode.DocURL = temp.DocURL;
                sourceCode.SearchTime = temp.SearchTime;

                await _identityContext.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> RemoveSourceCode(SourceCodeForDisPlay temp)
        {
            var sourceCode = _identityContext.SourceCodes.FirstOrDefault(m => m.ID == temp.ID);
            if (sourceCode == null)
            {
                return true;
            }
            _identityContext.SourceCodes.Remove(sourceCode);
            await _identityContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveSourceCode(int id)
        {
            var sourceCode = _identityContext.SourceCodes.FirstOrDefault(m => m.ID == id);
            if (sourceCode == null)
            {
                return true;
            }
            _identityContext.SourceCodes.Remove(sourceCode);
            await _identityContext.SaveChangesAsync();

            return true;
        }

        public List<SourceCodeForDisPlay> GetSortedSourceCode()
        {
            List<SourceCodeForDisPlay> result = new List<SourceCodeForDisPlay>();
            var sourceCodes = from s in _identityContext.SourceCodes select s;
            foreach (var item in sourceCodes)
            {
                result.Add(new SourceCodeForDisPlay(item));
            }
            result.Sort();

            return result;
        }
    }
}
