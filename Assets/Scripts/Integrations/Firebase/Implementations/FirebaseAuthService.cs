using Assets.Scripts.Integrations.Firebase.Interfaces;
using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using System;
using UnityEngine;

namespace Assets.Scripts.Integrations.Firebase.Implementations
{
    internal class FirebaseAuthService : IFirebaseAuthService
    {
        private FirebaseAuth auth;

        public FirebaseAuthService()
        {
            auth = FirebaseAuth.DefaultInstance;
        }

        public async UniTask<FirebaseUser> SignIn(string email, string password)
        {
            var signInTask = await auth.SignInWithEmailAndPasswordAsync(email, password);
            var user = signInTask.User;

            if (user != null && !user.IsEmailVerified)
            {
                throw new FirebaseException((int)AuthError.UnverifiedEmail, "Email has not been confirmed!");
            }

            return user;
        }

        public async UniTask RecoveryPassword(string email)
        {
            await auth.SendPasswordResetEmailAsync(email);
        }
    }
}
