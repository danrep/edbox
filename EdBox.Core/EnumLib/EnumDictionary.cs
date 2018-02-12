using System;
using System.Collections.Generic;

namespace EdBox.Core.EnumLib
{
    public static class EnumDictionary
    {
        public static List<EnumList> GetList<T>() where T : struct
        {
            var returnList = new List<EnumList>();
            
            foreach(int e in Enum.GetValues(typeof(T)))
            {
                returnList.Add(new EnumList
                {
                    ItemId = e,
                    ItemName = ((Enum)Enum.Parse(typeof(T), e.ToString())).DisplayName()
                });
            }

            return returnList;
        }
    }

    public class EnumList
    {
        public int ItemId { get;  set; }
        public string ItemName { get; set; }
    }
}
