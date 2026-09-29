using System.Security.Cryptography;
using SchoolManagement.Application.Common.Interfaces;

namespace SchoolManagement.Infrastructure.Identity;

/// <summary>
/// Generates random passwords that satisfy the Identity password policy configured in
/// DependencyInjection (upper + lower + digit + symbol), using visually-unambiguous characters.
/// </summary>
public sealed class SecurePasswordGenerator : IPasswordGenerator
{
    private const string Lower = "abcdefghijkmnopqrstuvwxy";   // no 'l'
    private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";   // no 'I', 'O'
    private const string Digits = "23456789";                  // no '0', '1'
    private const string Special = "!@#$%^&*?";
    private const string All = Lower + Upper + Digits + Special;

    public string Generate(int length = 12)
    {
        if (length < 8) length = 8;

        var chars = new char[length];
        chars[0] = PickFrom(Lower);
        chars[1] = PickFrom(Upper);
        chars[2] = PickFrom(Digits);
        chars[3] = PickFrom(Special);

        for (var i = 4; i < length; i++)
            chars[i] = PickFrom(All);

        Shuffle(chars);
        return new string(chars);
    }

    private static char PickFrom(string alphabet) => alphabet[RandomNumberGenerator.GetInt32(alphabet.Length)];

    private static void Shuffle(char[] chars)
    {
        for (var i = chars.Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
    }
}
