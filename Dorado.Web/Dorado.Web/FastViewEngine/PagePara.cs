using System;

namespace Dorado.Web.FastViewEngine
{
    /// <summary>
    /// 通用查询结构
    /// </summary>
    public class SearchPara
    {
        public string key0 = string.Empty;
        public string key1 = string.Empty;
        public string key2 = string.Empty;
        public string key3 = string.Empty;
        public string key4 = string.Empty;
        public string key5 = string.Empty;
        public string key6 = string.Empty;
        public string key7 = string.Empty;
        public string key8 = string.Empty;
        public string key9 = string.Empty;

        public long kind0;
        public long kind1;
        public long kind2;
        public long kind3;
        public long kind4;
        public long kind5;
        public long kind6;
        public long kind7;
        public long kind8;
        public long kind9;

        public int page;
        public int pagesize;
        public int maxcount;
    }

    /// <summary>
    /// 详细信息结构
    /// </summary>
    public struct ViewPara
    {
        public long id;
    }

    /// <summary>
    /// 主键信息结构
    /// </summary>
    public struct KeyPara
    {
        public string id;

        public string[] ids
        {
            get
            {
                if (string.IsNullOrEmpty(id))
                    return new string[] { };
                return id.Split(new[] { ',', '|' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}