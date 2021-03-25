using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lightaplusplus.BisLogic
{
    public class SectionAdder
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;
        public SectionAdder(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public void addSection(int CourseId, int InstructorId, string SectionLocation, DateTime StartTime, DateTime EndTime, string DaysTaught, int SectionCapacity)
        {
            Models.Sections newSection = new Models.Sections();

            newSection.CourseId = CourseId;
            newSection.InstructorId = InstructorId;
            newSection.SectionLocation = SectionLocation;
            newSection.SectionTimeStart = StartTime;
            newSection.SectionTimeEnd = EndTime;
            newSection.DaysTaught = DaysTaught;
            newSection.SectionCapacity = SectionCapacity;

            
            _context.Sections.Add(newSection);
            _context.SaveChanges();
            //var testc = checkCourse(CourseCode, CourseNumber);
            return;
        }

        public bool checkSection(int CourseID, int InstructorId, DateTime StartTime)
        {
            try
            {
                var sections = _context.Sections.Where(s => s.CourseId == CourseID).Where(s => s.InstructorId == InstructorId).Where(s => s.SectionTimeStart == StartTime).First();
                return sections != null;
            }
            catch (Exception e)
            {
                // log exception
                return false;
            }
        }

        public void removeSection(int CourseID, int InstructorId, DateTime StartTime)
        {
            while (checkSection(CourseID, InstructorId, StartTime))
            {
                _context.Sections.Remove(_context.Sections.Where(s => s.CourseId == CourseID).Where(s => s.InstructorId == InstructorId).Where(s => s.SectionTimeStart == StartTime).First());
                _context.SaveChanges();
            }

            return;
        }
    }
}
