using IPA.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatSaviorData
{
    public static class BSUtilsTemporaryFix
    {
        private static FieldAccessor<PlatformLeaderboardsModel, IPlatformUserModel>.Accessor AccessPlatformUserModel;
        static string userName = null;
        static string userID = null;
        static UserInfo.Platform platform;
        private static Task<UserInfo> getUserTask;
        private static object getUserLock = new object();
        private static TaskCompletionSource<bool> shouldBeReadyTask = new TaskCompletionSource<bool>();
        private static bool isReady => shouldBeReadyTask.Task.IsCompleted;
        private static IPlatformUserModel _platformUserModel;

        static BSUtilsTemporaryFix()
        {
            try
            {
                AccessPlatformUserModel = FieldAccessor<PlatformLeaderboardsModel, IPlatformUserModel>.GetAccessor("_platformUserModel");
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error getting PlatformUserModel, GetUserInfo is unavailable: {ex.Message}");
                Logger.log.Debug(ex);
            }
        }

        internal static void TriggerReady()
        {
            shouldBeReadyTask.TrySetResult(true);
        }

        public static IPlatformUserModel GetPlatformUserModel()
        {
            return _platformUserModel ?? SetPlatformUserModel();
        }

        internal static IPlatformUserModel SetPlatformUserModel()
        {
            if (_platformUserModel != null)
                return _platformUserModel;
            try
            {
                // Need to check for null because there's multiple PlatformLeaderboardsModels (at least sometimes), and one has a null IPlatformUserModel with 'vrmode oculus'
                var leaderboardsModel = Resources.FindObjectsOfTypeAll<PlatformLeaderboardsModel>().Where(p => AccessPlatformUserModel(ref p) != null).FirstOrDefault();
                IPlatformUserModel platformUserModel = null;
                if (leaderboardsModel == null)
                {
                    Logger.log.Error($"Could not find a 'PlatformLeaderboardsModel', GetUserInfo unavailable.");
                    return null;
                }
                if (AccessPlatformUserModel == null)
                {
                    Logger.log.Error($"Accessor for 'PlatformLeaderboardsModel._platformUserModel' is null, GetUserInfo unavailable.");
                    return null;
                }

                platformUserModel = AccessPlatformUserModel(ref leaderboardsModel);
                _platformUserModel = platformUserModel;
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error getting 'IPlatformUserModel', GetUserInfo unavailable: {ex.Message}");
                Logger.log.Debug(ex);
            }
            return _platformUserModel;
        }

        public static async void UpdateUserInfo()
        {
            SetPlatformUserModel();

            try
            {
                await GetUserAsync();
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error retrieving UserInfo: {ex.Message}.");
                Logger.log.Debug(ex);
            }
        }

        /// <summary>
        /// Attempts to retrieve the UserInfo. Returns null if <see cref="IPlatformUserModel"/> is unavailable.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Thrown if <see cref="IPlatformUserModel"/> returns null for the <see cref="UserInfo"/>.</exception>
        public static async Task<UserInfo> GetUserAsync()
        {
            try
            {
                if (!isReady)
                    await shouldBeReadyTask.Task;
                lock (getUserLock)
                {
                    IPlatformUserModel platformUserModel = _platformUserModel ?? SetPlatformUserModel();
                    if (platformUserModel == null)
                    {
                        Logger.log.Error($"IPlatformUserModel not found, cannot update user info.");
                        return null;
                    }
                    if (getUserTask == null || getUserTask.Status == TaskStatus.Faulted)
                        getUserTask = InternalGetUserAsync();
                }
                return await getUserTask;
            }
            catch (Exception ex)
            {
                Logger.log.Error($"Error retrieving UserInfo: {ex.Message}.");
                Logger.log.Debug(ex);
                throw;
            }
        }

        private static async Task<UserInfo> InternalGetUserAsync()
        {
            UserInfo userInfo = await _platformUserModel.GetUserInfo();
            if (userInfo != null)
            {
                Logger.log.Debug($"UserInfo found: {userInfo.platformUserId}: {userInfo.userName}");
                userName = userInfo.userName;
                userID = userInfo.platformUserId;
                platform = userInfo.platform;
            }
            else
                throw new InvalidOperationException("UserInfo is null.");
            return userInfo;
        }

        public static UserInfo.Platform GetPlatformInfo()
        {
            return platform;
        }

        public static string GetUserName()
        {
            return userName;
        }

        public static string GetUserID()
        {
            return userID;
        }

    }
}
