using PetaPoco;
using Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizCodingCollective.Facade
{
    public class CommonFacade
    {
        public static bool IsUsernameExists(string username)
        {
            Database db = new Database("Quiz");
            var result = db.FirstOrDefault<Account>("Select * From Account Where username=@0 and IsActive=1", username);
            return result != null;
        }
    }
}