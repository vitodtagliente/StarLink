using System;
using System.ComponentModel;
using StarLink;

namespace GameWorld
{
    enum UserDataType
    {
        [Description("World")] World
    }

    static class UserWorldDataTypeExtension
    {
        public static string GetDescription(this UserDataType header)
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
        public static void SetWorld(this UserState userState, string world)
        {
            userState.Data.Add(UserDataType.World.GetDescription(), world);
        }

        public static bool TryGetWorld(this UserState userState, out string world)
        {
            world = string.Empty;
            if (userState.Data.ContainsKey(UserDataType.World.GetDescription()))
            {
                world = userState.Data[UserDataType.World.GetDescription()];
                return true;
            }
            return false;
        }
    }
}
