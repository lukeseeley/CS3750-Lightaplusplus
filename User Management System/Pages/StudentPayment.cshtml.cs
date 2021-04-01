using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lightaplusplus.BisLogic;
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
        [Required]
        [BindProperty]
        public string CardName { get; set; }
        [Required]
        [BindProperty]
        public string CardMonth { get; set; }
        [Required]
        [BindProperty]
        public string CardNumber { get; set; }
        [Required]
        [BindProperty]
        public string SecurityCode { get; set; }
        [Required]
        [BindProperty]
        public string CardYear { get; set; }
        [Required]
        [BindProperty]
        public double PaymentAmount { get; set; }

        public Sections[] SectionsArray { get; set; }

        public List<Payments> Payments { get; set; }

        public string ErrorCardNumber { get; set; }
        public string ErrorSecurityCode { get; set; }
        public string ErrorMonth { get; set; }
        public string ErrorYear { get; set; }
        public string ErrorPaymentAmount { get; set; }

        public string Message { get; set; }

        [BindProperty]
        public Notifications Notifications { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var id = Session.getUserId(HttpContext.Session);
            var userType = Session.getUserType(HttpContext.Session);
            ViewData["UserId"] = id;
            ViewData["UserType"] = userType;
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'S');
            if (path != "") return RedirectToPage(path);

            RemainingBalance = await GetRemainingBalacne();
            PaymentAmount = RemainingBalance;

            Payments = await _context.Payments.Where(p => p.UserId == (int)id).ToListAsync();

            Notifications = new Notifications(HttpContext.Session, _context);


            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var id = Session.getUserId(HttpContext.Session);
            var path = UserValidator.validateUser(_context, HttpContext.Session, 'S');
            if (path != "") return RedirectToPage(path);

            Payments = await _context.Payments.Where(p => p.UserId == (int)id).ToListAsync();

            RemainingBalance = await GetRemainingBalacne();

            if (!ModelState.IsValid)
            {
                RemainingBalance = await GetRemainingBalacne();
                PaymentAmount = RemainingBalance;
                return Page();
            }
            
            // validate cardnumber
            if (!Regex.IsMatch(CardNumber, @"\d{16}"))
            {
                ErrorCardNumber = "Card Number must be 16 digits.";
            }
            else
            {
                ErrorCardNumber = string.Empty;
            }

            // validate cvc
            if (!Regex.IsMatch(SecurityCode, @"\d{3}"))
            {
                ErrorSecurityCode = "Security code must be 3 digits.";
            }
            else
            {
                ErrorSecurityCode = string.Empty;
            }

            // Validate month
            try
            {
                int month = int.Parse(CardMonth);
            }
            catch
            {
                ErrorMonth = "Enter a number";
            }
            if(int.Parse(CardMonth) > 12 || int.Parse(CardMonth) < 1)
            {
                ErrorMonth = "Enter a valid month (1-12).";
            }
            else
            {
                ErrorMonth = string.Empty;
            }

            // Validate year
            try
            {
                int year = int.Parse(CardYear);
            }
            catch
            {
                ErrorYear = "Enter a number";
            }
            if (int.Parse(CardYear) < DateTime.Now.Year)
            {
                ErrorYear = "Enter a valid year.";
            }
            else
            {
                ErrorYear = string.Empty;
            }

            //Validate PaymentAmount
            if(PaymentAmount > RemainingBalance)
            {
                ErrorPaymentAmount = "Enter a payment amount that is less than the remaining balance.";
            }
            else if(PaymentAmount <= 0)
            {
                ErrorPaymentAmount = "Payment amount has to be greater than 0.";
            }
            else
            {
                ErrorPaymentAmount = string.Empty;
            }

            if (ErrorCardNumber == string.Empty && ErrorMonth == string.Empty && ErrorYear == string.Empty && ErrorSecurityCode == string.Empty && ErrorPaymentAmount == string.Empty)
            {
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
                    Payments payment = new Payments();
                    // set payment to correct type
                    payment.PaymentAmount = (int)PaymentAmount;
                    payment.PaymentDateTime = DateTime.Now;
                    payment.UserId = (int)id;

                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();

                    Message = "Payment Successful";
                    RemainingBalance = await GetRemainingBalacne();
                    PaymentAmount = RemainingBalance;
                    return Page();
                }
                else
                {
                    Message = "Payment Failed";
                    RemainingBalance = await GetRemainingBalacne();
                    PaymentAmount = RemainingBalance;
                    return Page();
                }
            }
            else
            {
                RemainingBalance = await GetRemainingBalacne();
                PaymentAmount = RemainingBalance;
                return Page();
            }
        }

        public async Task<int> GetRemainingBalacne()
        {
            var id = Session.getUserId(HttpContext.Session);

            // get all the sections the student is in
            var StudentSections = await _context.SectionStudents.Where(ss => ss.StudentId == id).ToListAsync();

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
            var paymentList = await _context.Payments.Where(p => p.UserId == id).ToListAsync();

            int? paymentTotal = 0;

            // get total payments
            foreach (var payment in paymentList)
            {
                paymentTotal += payment.PaymentAmount;
            }

            return (enrollmentTotal * 100) - (int)paymentTotal;
        } 
    }
}
