using System.Globalization;

namespace ResumePDFAnalyzer.Services;

internal static class ResumeAgentHelpers
{
    private static readonly string[] DateFormats =
    [
        "yyyy-MM-dd",
        "dd/MM/yyyy",
        "d/M/yyyy",
        "dd-MM-yyyy",
        "d MMM yyyy",
        "dd MMM yyyy",
        "d MMMM yyyy",
        "dd MMMM yyyy",
        "MMMM d, yyyy",
        "MMMM dd, yyyy",
        "MMM d, yyyy",
        "MMM dd, yyyy",
    ];

    internal static readonly string[] InjectionPatterns =
    [
        "ignore previous", "ignore above", "ignore all", "ignore the above",
        "ignore your", "ignore prior", "ignore earlier",
        "forget previous", "forget your", "forget the above",
        "disregard", "override", "bypass",
        "you are now", "you are a", "act as", "act like",
        "pretend you", "pretend to be", "roleplay as", "play the role",
        "your new role", "your role is now", "switch to", "become a",
        "new instruction", "new task", "new prompt", "new context",
        "system prompt", "your instructions", "your rules", "your guidelines",
        "from now on", "starting now", "instead of",
        "jailbreak", "do anything now", "dan mode", "developer mode",
        "unrestricted mode", "no restrictions", "no limits", "without restrictions",
        "unlock", "enable all", "disable safety",
        "###", "```system", "[system]", "<system>", "</system>",
        "[instructions]", "</instructions>", "[prompt]",
        "human:", "assistant:", "user:", "ai:",
        "ลืมคำสั่ง", "เปลี่ยนบทบาท", "สมมติว่าคุณ", "แกล้งทำ", "ทำเป็นว่า",
        "ละเว้นคำสั่ง", "ไม่ต้องสนใจ", "คำสั่งใหม่", "บทบาทใหม่"
    ];

    internal static bool IsInjectionAttempt(string input)
    {
        var lower = input.ToLowerInvariant();
        return InjectionPatterns.Any(lower.Contains);
    }

    internal static string CalculateAge(string birthDateStr)
    {
        if (string.IsNullOrWhiteSpace(birthDateStr) || birthDateStr == "-")
            return "-";

        var culture = CultureInfo.InvariantCulture;

        if (!DateOnly.TryParseExact(birthDateStr, DateFormats, culture,
                DateTimeStyles.None, out var birthDate)
            && !DateOnly.TryParse(birthDateStr, culture,
                DateTimeStyles.None, out birthDate))
            return "-";

        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - birthDate.Year;
        if (today < birthDate.AddYears(age))
            age--;

        return age.ToString();
    }
}
