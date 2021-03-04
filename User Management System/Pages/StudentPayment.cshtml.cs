using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lightaplusplus.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Lightaplusplus.Pages
{
    public class StudentPaymentModel : PageModel
    {
        private readonly Lightaplusplus.Data.Lightaplusplus_SystemContext _context;

        public StudentPaymentModel(Lightaplusplus.Data.Lightaplusplus_SystemContext context)
        {
            _context = context;
        }
        [BindProperty]
        public double RemainingBalance { get; set; }
        [BindProperty]
        public string CardName { get; set; }
        [BindProperty]
        public string CardMonth { get; set; }
        [BindProperty]
        public string CardNumber { get; set; }
        [BindProperty]
        public string SecurityCode { get; set; }
        [BindProperty]
        public string CardYear { get; set; }
        [BindProperty]
        public double PaymentAmount { get; set; }

        public Sections[] SectionsArray { get; set; }

        public Users Users { get; set; }
        public Payments Payment { get; set; }
        
        public string Message { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);

            // get all the sections the student is in
            var StudentSections = await _context.SectionStudents.Where(ss => ss.StudentId == Users.ID).ToListAsync();

            SectionsArray = new Sections[StudentSections.Count()];

            // put the sections in a list
            List<Sections> sectionsList = new List<Sections>();
            foreach (var section in StudentSections)
            {
                var sections = await _context.Sections.Include(s => s.Instructor).Where(s => s.SectionId == section.SectionId).FirstOrDefaultAsync();
                sectionsList.Add(sections);
            }

            // put the list into SectionsArray
            int i = 0;
            foreach (var section in sectionsList)
            {
                SectionsArray[i] = section;
                i++;
            }

            int enrollmentTotal = 0;

            // get the course information
            foreach (var studSection in SectionsArray)
            {
                var courses = _context.Courses.Where(c => c.CourseId == studSection.CourseId);
                foreach (var course in courses)
                {
                    studSection.Course = course;
                    enrollmentTotal += course.CourseCreditHours;
                }
            }

            // get all the payments the student has made
            var paymentList = await _context.Payments.Where(p => p.UserId == Users.ID).ToListAsync();

            int? paymentTotal = 0;

            // get total payments
            foreach (var payment in paymentList)
            {
                paymentTotal += payment.PaymentAmount;
            }

            RemainingBalance = (enrollmentTotal * 100) - (int)paymentTotal;
            PaymentAmount = RemainingBalance;
            SecurityCode = "123";
            CardMonth = DateTime.Now.Month.ToString();
            CardYear = DateTime.Now.Year.ToString();

            if (Users == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            Users = await _context.Users.FirstOrDefaultAsync(m => m.ID == id);
            // set card to values entered
            CreditCard card = new CreditCard();
            card.cardNumber = CardNumber;
            card.cvc = SecurityCode;
            card.exp_month = CardMonth;
            card.exp_year = CardYear;

            // try to process payment
            string result = PaymentProcessor.processPayment(card, PaymentAmount);

            // check the results
            if (result == "succeeded")
            {
                // set payment to correct type
                Payment.PaymentAmount = (int)PaymentAmount;
                Payment.PaymentDateTime = DateTime.Now;
                Payment.UserId = Users.ID;

                _context.Payments.Add(Payment);
                await _context.SaveChangesAsync();

                Message = "Payment Successful";
                return Page();
            }
            else
            {
                Message = "Payment Failed";
                return Page();
            }

            
        }
    }
}
