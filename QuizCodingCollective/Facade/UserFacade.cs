using PetaPoco;
using Quiz;
using QuizCodingCollective.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizCodingCollective.Facade
{
    public class UserFacade
    {
        public static LoginInfoSession Auth(LoginViewModel model, out List<string> errorMsg)
        {
            LoginInfoSession result = new LoginInfoSession();

            errorMsg = new List<string>();
            if (string.IsNullOrEmpty(model.Username))
            {
                errorMsg.Add("Username cannot be empty");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                errorMsg.Add("Password cannot be empty");
            }

            if (errorMsg.Count > 0)
                return null;

            Database db = new Database("Quiz");
            var account = db.FirstOrDefault<Account>("Select * From Account Where Username=@0", model.Username);
            PasswordHash hash = new PasswordHash(model.Password);
            if (account != null)
            {
                var bytes = Convert.FromBase64String(account.Password);
                hash = new PasswordHash(bytes);

                if (!hash.Verify(model.Password))
                {
                    errorMsg.Add("Invalid username or password");
                    return null;
                }

                result.UserID = account.Id.ToString();
                result.Username = account.Username;
                return result;
            }
            return null;
        }

        public LoginInfoSession RegisterVisitor(RegisterViewModel model, out List<string> errorMsg)
        {
            errorMsg = new List<string>();
            if (String.IsNullOrEmpty(model.Username))
            {
                errorMsg.Add("Fullname cannot be Empty");
            }
            if (CommonFacade.IsUsernameExists(model.Username))
            {
                errorMsg.Add("Username already exist");
            }
            if (String.IsNullOrEmpty(model.Password))
            {
                errorMsg.Add("Password cannot be Empty");
            }
            if (errorMsg.Count > 0)
                return null;

            PasswordHash hash = new PasswordHash(model.Password);
            byte[] hashBytes = hash.ToArray();
            var hashedPassword = Convert.ToBase64String(hashBytes);

            Account account = new Account()
            {
                Username = model.Username,
                Password = hashedPassword
            };

            Database db = new Database("Quiz");
            var inserted = db.Insert(account);

            LoginInfoSession loginInfoSession = new LoginInfoSession();
            loginInfoSession.Username = model.Username;
            loginInfoSession.UserID = inserted.ToString();
            return loginInfoSession;
        }
    }
}