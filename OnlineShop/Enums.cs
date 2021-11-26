using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineShop
{
   public enum AuthenticationLogType:byte
    {
        WrongCaptcha=1,
        WrongUsername,
        WrongEasyCode,
        WrongPassword,
        InactiveUser,
        UserLocked,
        EmptyInputs,
        SimultaneousSession,
        PasswordMustChange,
        NotAllowedIpAddress,
        SuccessfullLogin,
        ChangedHostName,
        LoginTimeOut,
        UnknownUser,
        UserAccessDenied,
    }
}