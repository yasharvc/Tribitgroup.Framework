namespace Tribitgroup.Framework.Identity.Shared.Enums
{
    public enum UserActivityEnum : int
    {
        Login,
        Logout,
        WrongPasswordTrial,
        Locked,
        Unlocked,
        Registered,
        Confirmed,
        EmailConfirmed,
        PhoneConfirmed,
    }
}