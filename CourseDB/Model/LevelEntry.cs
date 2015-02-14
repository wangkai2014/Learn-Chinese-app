﻿
namespace CourseDB.Model
{
    /// <summary>
    /// An entry within a level.
    /// </summary>
    public class LevelEntry
    {
        /// <summary>
        /// The ID of the entry in the entry database.
        /// </summary>
        public string EntryId { get; set; }

        /// <summary>
        /// The selected translation to be used in the level.
        /// </summary>
        public string SelectedTranslation { get; set; }
    }
}
