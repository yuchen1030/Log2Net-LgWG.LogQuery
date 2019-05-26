using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LgWG.LogQuery.DTO
{
    public class SearchBaseInputDto : IPagedResultRequest, ISortedResultRequest, IShouldNormalize
    {
        public SearchBaseInputDto()
        {
            MaxResultCount = LogQueryConsts.DefaultPageSize;
            IsDesc = true;
        }

        [Range(1, LogQueryConsts.MaxPageSize)]
        public int MaxResultCount { get; set; }

        [Range(0, int.MaxValue)]
        public int SkipCount { get; set; }

        public string Sorting { get { return _sortStr; } set { _sortStr = value; } }

        private string _sortStr = "";
        public bool IsDesc { get; set; }
        public string Other { get; set; }
        public void Normalize()
        {
            GetWholeSortString();
        }
        string GetWholeSortString()
        {
            if (string.IsNullOrEmpty(_sortStr))
            {
                _sortStr = "Id";
            }
            if (!_sortStr.ToLower().Contains(" asc") && !_sortStr.ToLower().Contains(" desc"))
            {
                _sortStr += " " + (IsDesc ? "desc" : "asc");
            }
            return _sortStr;
        }

    }
}
