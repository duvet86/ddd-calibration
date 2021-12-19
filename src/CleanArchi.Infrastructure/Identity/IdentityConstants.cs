namespace CleanArchi.Infrastructure.Identity;

public class IdentityConstants
{
    public const string COOCKIE_SCHEME = "Identity.Application";

    public const string AUTH_KEY = "AuthKeyOfDoomThatMustBeAMinimumNumberOfBytes";

    // TODO: Change this to an environment variable
    public const string JWT_SECRET_KEY = "SecretKeyOfDoomThatMustBeAMinimumNumberOfBytes";

    public const string ADMIN_ROLE = "Admin";
    public const string USER_ROLE = "User";
}
