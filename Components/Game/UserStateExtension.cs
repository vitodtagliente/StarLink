using System;
using System.ComponentModel;
using StarLink;

namespace Game
{
    enum UserDataType
    {
        [Description("PossessedGameObject")] PossessedGameObject,
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
        public static void SetWorld(this UserState userState, World world)
        {
            userState.Data.Add(UserDataType.World.GetDescription(), world.Name);
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

        public static void Possess(this UserState userState, GameObject gameObject)
        {
            userState.Data.Add(UserDataType.PossessedGameObject.GetDescription(), gameObject.Id.ToString());
        }

        public static bool TryGetGameObject(this UserState userState, out StarId id)
        {
            id = StarId.Empty;
            if (userState.Data.ContainsKey(UserDataType.PossessedGameObject.GetDescription()))
            {
                id = StarId.Parse(userState.Data[UserDataType.PossessedGameObject.GetDescription()]);
                return true;
            }
            return false;
        }
    }
}
