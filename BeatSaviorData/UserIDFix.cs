using IPA.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using BS_Utils.Gameplay;

namespace BeatSaviorData
{
    public delegate void UserIDReady();

    public class UserIDFix
    {
        public static bool UserIDIsReady = false;
        public static string UserID;
        public static event UserIDReady UserIDReady;

        private static UserInfo user;

        public static async void GetUserID()
        {
            await WaitForUserID();
            UserID = user.platformUserId;
            UserIDIsReady = true;
            UserIDReady?.Invoke();
        }

        private static async Task WaitForUserID()
        {
            user = await GetUserInfo.GetUserAsync();
        }
    }
}
