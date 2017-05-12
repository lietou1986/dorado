using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Dorado.VWS.Utils
{
    /// <summary>
    /// 枚举名称属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public class EnumNameAttribute : Attribute
    {
        private string name;

        public string Name
        {
            get { return this.name; }
        }

        public EnumNameAttribute(string name)
        {
            this.name = name;
        }
    }

    /// <summary>
    /// 枚举描述属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDescriptionAttribute : Attribute
    {
        private string description;

        public string Description
        {
            get { return this.description; }
        }

        public EnumDescriptionAttribute(string description)
        {
            this.description = description;
        }
    }

    public class EnumUtil
    {
        #region Enum操作方法

        /// <summary>
        /// 枚举类型转换为ListItem构成的List
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<ListItem> EnumToList(Type enumType, int enumValue = 0, bool selectShow = false, params string[] types)
        {
            if (!enumType.IsEnum)
                return null;
            string fullName = enumType.FullName;
            //if (!_enumList.ContainsKey(fullName))
            //{
            List<ListItem> list = new List<ListItem>();
            if (selectShow)
            {
                ListItem item_select = new ListItem();
                item_select.Selected = true;
                item_select.Text = "请选择";
                item_select.Value = string.Empty;
                list.Add(item_select);
            }

            foreach (int i in Enum.GetValues(enumType))
            {
                ListItem item_temp = new ListItem();
                string name = Enum.GetName(enumType, i);
                item_temp.Value = i.ToString();

                if (types.Length > 0)
                {
                    string nameName = string.Empty;
                    object[] objsName = enumType.GetField(name).GetCustomAttributes(typeof(EnumNameAttribute), false);
                    if (objsName != null && objsName.Length > 0)
                    {
                        nameName = ((EnumNameAttribute)objsName[0]).Name;
                    }
                    if (!string.IsNullOrEmpty(nameName))
                    {
                        bool addCur = false;
                        foreach (var s in types)
                        {
                            if (nameName == s)
                            {
                                addCur = true;
                                break;
                            }
                        }
                        if (addCur)
                        {
                            string showName = string.Empty;//获取自定义描述属性值
                            object[] objs = enumType.GetField(name).GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
                            if (objs != null && objs.Length > 0)
                            {
                                showName = ((EnumDescriptionAttribute)objs[0]).Description;
                            }
                            item_temp.Text = !string.IsNullOrEmpty(showName) ? showName : name;

                            if (item_temp.Value == enumValue.ToString())
                            {
                                item_temp.Selected = true;
                            }
                            list.Add(item_temp);
                        }
                    }
                }
                else
                {
                    string showName = string.Empty;//获取自定义描述属性值
                    object[] objs = enumType.GetField(name).GetCustomAttributes(typeof(EnumDescriptionAttribute), false);
                    if (objs != null && objs.Length > 0)
                    {
                        showName = ((EnumDescriptionAttribute)objs[0]).Description;
                    }
                    item_temp.Text = !string.IsNullOrEmpty(showName) ? showName : name;

                    if (item_temp.Value == enumValue.ToString())
                    {
                        item_temp.Selected = true;
                    }
                    list.Add(item_temp);
                }
            }

            //object syncObj = new object();//避免重复添加
            //if (!_enumList.ContainsKey(fullName))
            //{
            //    lock (syncObj)
            //    {
            //        if (!_enumList.ContainsKey(fullName))
            //        {
            //            _enumList.Add(fullName, list);
            //        }
            //    }
            //}
            //}
            //else
            //{
            //    if (enumValue > 0)
            //        foreach (var item in _enumList[fullName])
            //        {
            //            if (item.Value == enumValue.ToString())
            //                item.Selected = true;
            //        }
            //}
            return list;//_enumList[fullName];
        }

        /// <summary>
        /// 获取某个枚举的显示名称
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static string GetEnumShowName(Type enumType, string strValue)
        {
            string result = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(strValue))
                {
                    return result;
                }
                ListItem temp_item = null;
                List<ListItem> temp_list = EnumToList(enumType);
                var selectItem = temp_list.Where(t => t.Value == strValue);
                if (selectItem.Count() > 0)
                {
                    foreach (var item in selectItem)
                    {
                        temp_item = item;
                    }
                }
                if (temp_item != null)
                    result = temp_item.Text;
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        /// <summary>
        /// 获取某个枚举的显示名称
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static string GetEnumShowName(Type enumType, int intValue)
        {
            string result = string.Empty;
            try
            {
                ListItem temp_item = null;
                List<ListItem> temp_list = EnumToList(enumType);
                var selectItem = temp_list.Where(t => t.Value == intValue.ToString());
                if (selectItem.Count() > 0)
                {
                    foreach (var item in selectItem)
                    {
                        temp_item = item;
                    }
                }
                if (temp_item != null)
                    result = temp_item.Text;
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        #endregion Enum操作方法
    }
}