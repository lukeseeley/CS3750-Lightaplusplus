using Lightaplusplus.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Lightaplusplus.BisLogic
{
    public class StudentRegister
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;
        public StudentRegister(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }

        public List<SectionRegistrationData> GetSectionRegistration(int studentId, ISession sesh)
        {
            List<SectionRegistrationData> registration = new List<SectionRegistrationData>();

            /*var SectionsList = _context.Sections
                .Include(s => s.Instructor)
                .Include(s => s.Course)
                .AsNoTracking()
                .ToList();*/

            var SectionsList = Session.getAllSections(sesh);

            foreach (var section in SectionsList)
            {
                var sectionRegistry = _context.SectionStudents.Where(sr => sr.SectionId == section.SectionId).Count();
                var isEnrolled = _context.SectionStudents.FirstOrDefault(ss => ss.SectionId == section.SectionId && ss.StudentId == studentId); // can't take this out because we need to make sure the class isn't full
                char registrationStatus;
                if (isEnrolled != null) //Meaning this student is already registered in this section
                {
                    registrationStatus = 'R';
                }
                else if (sectionRegistry >= section.SectionCapacity) //Meaning the class is at full capacity
                {
                    registrationStatus = 'F';
                }
                else //Meaning the student is not enrolled in this class, and the class is not full
                {
                    registrationStatus = 'N';
                }
                registration.Add(new SectionRegistrationData(section, sectionRegistry, registrationStatus));
            }

            return registration;
        }

        /// <summary>
        /// A method for registering a student
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="sectionId"></param>
        /// <returns>non-zero means error. 1 => invalid user ; 2 => invalid section 3=> student already registered 4=> class at capacity</returns>
        public int RegisterStudent(int studentId, int sectionId)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == studentId);
            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);
            
            //Base checks
            if(user == null)
            {
                return 1;
            }
            if(section == null)
            {
                return 2;
            }

            //Specific checks
            if(checkRegistration(studentId, sectionId))
            {
                return 3;
            }
            if(!checkCapacity(sectionId))
            {
                return 4;
            }

            SectionStudents sectionStudents = new SectionStudents
            {
                StudentId = studentId,
                SectionId = sectionId
            };

            _context.SectionStudents.Add(sectionStudents);
            _context.SaveChanges();

            return 0;
        }

        /// <summary>
        /// A method for removing a student from a section.
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="sectionId"></param>
        /// <returns>non-zero means error. 1 => invalid user ; 2 => invalid section 3=> student is not registered</returns>
        public int DropStudent(int studentId, int sectionId)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == studentId);
            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);

            //Base checks
            if (user == null)
            {
                return 1;
            }
            if (section == null)
            {
                return 2;
            }

            //Specific checks
            if (!checkRegistration(studentId, sectionId))
            {
                return 3;
            }

            var registry = _context.SectionStudents.FirstOrDefault(ss => ss.StudentId == studentId && ss.SectionId == sectionId);
            _context.SectionStudents.Remove(registry);
            _context.SaveChanges();

            return 0;
        }

        /// <summary>
        /// A method for determining if a student is already registered in a class 
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="sectionId"></param>
        /// <returns>true if the student is registered for said course, false otherwise</returns>
        public bool checkRegistration (int studentId, int sectionId)
        {
            var isRegistered = _context.SectionStudents.FirstOrDefault(ss => ss.StudentId == studentId && ss.SectionId == sectionId);

            if (isRegistered == null) return false;
            else return true;
        }

        /// <summary>
        /// A method to check if a section is below capacity or not
        /// </summary>
        /// <param name="sectionId"></param>
        /// <returns>true if the section is not already at capacity, false otherwise</returns>
        public bool checkCapacity (int sectionId)
        {
            var sectionRegistry = _context.SectionStudents.Where(ss => ss.SectionId == sectionId).ToList();
            var section = _context.Sections.FirstOrDefault(s => s.SectionId == sectionId);

            if(section == null)
            {
                throw new Exception("Invalid Section Id");
            }

            if (section.SectionCapacity > sectionRegistry.Count) return true;
            else return false;
        }
    }
}
