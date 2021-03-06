﻿
namespace LanguageModel
{
    /// <summary>
    /// Interface for Chinese characters.
    /// </summary>
    public interface IChineseCharacter
    {
        /// <summary>
        /// Mandarin form.
        /// </summary>
        IPinyinSyllable Mandarin { get; }

        /// <summary>
        /// Simplified character.
        /// </summary>
        char Simplified { get; }

        /// <summary>
        /// Traditional character.
        /// </summary>
        char Traditional { get; }
    }
}
