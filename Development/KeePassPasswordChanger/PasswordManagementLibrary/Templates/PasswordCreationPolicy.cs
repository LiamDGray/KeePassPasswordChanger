using System;

namespace KeePassPasswordChanger.Templates
{
    [Serializable]
    public class PasswordCreationPolicy
    {
        public bool LowerCase = true;
        public bool UpperCase = true;
        public bool Digits = true;
        public bool Punctuation = true;
        public bool Brackets = true;
        public bool SpecialAscii = true;

        public bool ExcludeLookAlike = false;
        public int Length = 50;
        public bool NoRepeatingCharacters = false;

    }
}
