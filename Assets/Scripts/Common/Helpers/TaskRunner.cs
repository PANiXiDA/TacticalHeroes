using Assets.Scripts.Common.Constants;
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Assets.Scripts.Common.Helpers
{
    public static class TaskRunner
    {
        public static event Action<string> OnErrorOccurred;

        public static UniTask RunWithGlobalErrorHandling(Func<UniTask> task)
        {
            return Run(task);
        }

        private static async UniTask Run(Func<UniTask> task)
        {
            try
            {
                await task();
            }
            catch (ApplicationException ex)
            {
                OnErrorOccurred?.Invoke(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error: {ex.Message}");
                OnErrorOccurred?.Invoke(ErrorConstants.ServerUnavailable);
            }
        }
    }
}