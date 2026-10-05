using Microsoft.AspNetCore.Identity;

namespace GelatinCapsule.Infrastructure.Services;

public class PasswordHasher
{
    private readonly IPasswordHasher<string> _hasher;

    public PasswordHasher()
    {
        // PBKDF2 با 100000 iteration — استاندارد OWASP 2023
        _hasher = new PasswordHasher<string>(
            new Microsoft.Extensions.Options.OptionsWrapper<PasswordHasherOptions>(
                new PasswordHasherOptions
                {
                    IterationCount = 100_000,
                    CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3
                }));
    }

    public string Hash(string password)
    {
        return _hasher.HashPassword(string.Empty, password);
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(string.Empty, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success
            || result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}