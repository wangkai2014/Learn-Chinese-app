﻿using System;
using System.Collections.Generic;

namespace CourseDB
{
    /// <summary>
    /// A course database contains a taught language course, consisting of levels
    /// and course metadata.
    /// </summary>
    public class Course : ICourse
    {
        /// <summary>
        /// Construct a course database.
        /// </summary>
        public Course()
        {
            this.Levels = new List<Level>();
        }

        /// <summary>
        /// The course ID.
        /// </summary>
        public CourseId Id { get; set; }

        /// <summary>
        /// The name of the course.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the course.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The time the course was last updated.
        /// </summary>
        public DateTime Updated { get; set; }

        /// <summary>
        /// The path to the database file.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// The ordered list of levels in the course.
        /// </summary>
        public IList<Level> Levels { get; set; }
    }
}
