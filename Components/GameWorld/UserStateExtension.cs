using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using StarLink;

namespace GameWorld
{
    enum UserWorldDataType
    {
        [Description("World")] World
    }

    static class UserWorldDataTypeExtension
    {
        public static string GetDescription(this UserWorldDataType header)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])header
               .GetType()
               .GetField(header.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

    static class UserStateExtension
    {
        public static void SetWorld(this UserState userState)
        {

        }
    }
}
