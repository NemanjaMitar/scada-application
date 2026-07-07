using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace ScadaGUI.Utils
{
    // Lokalizacija preko .resx fajlova (Localization/Strings.resx = srpski, Strings.en.resx = engleski).
    // Takođe drži izabranu vremensku zonu i format datuma za prikaz vremena kroz aplikaciju.
    public static class LocalizationManager
    {
        private static readonly ResourceManager Rm =
            new ResourceManager("ScadaGUI.Localization.Strings", Assembly.GetExecutingAssembly());

        // Srpski = InvariantCulture (koristi neutralni Strings.resx), engleski = "en" satelit.
        public static CultureInfo CurrentCulture { get; private set; } = CultureInfo.InvariantCulture;

        public static string CurrentLanguage { get; private set; } = "sr";

        // Format za prikaz datuma/vremena.
        public static string DateFormat { get; set; } = "dd.MM.yyyy HH:mm:ss";

        // Pomeraj u satima u odnosu na UTC.
        public static double TimeZoneOffsetHours { get; set; } = 1;

        public static void SetLanguage(string languageCode)
        {
            CurrentLanguage = languageCode;
            CurrentCulture = languageCode == "en" ? new CultureInfo("en") : CultureInfo.InvariantCulture;
        }

        // Vraća prevedeni string za dati ključ; ako ključ ne postoji vraća sam ključ (radi lakšeg debagovanja).
        public static string Get(string key)
        {
            return Rm.GetString(key, CurrentCulture) ?? key;
        }

        // Prikaz vremena po izabranoj zoni i formatu.
        public static string FormatDateTime(DateTime localDateTime)
        {
            DateTime utc = DateTime.SpecifyKind(localDateTime, DateTimeKind.Local).ToUniversalTime();
            DateTime shifted = utc.AddHours(TimeZoneOffsetHours);
            return shifted.ToString(DateFormat, CultureInfo.InvariantCulture);
        }

        public static string NowFormatted()
        {
            return FormatDateTime(DateTime.Now);
        }
    }
}
