using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lightaplusplus.Pages.Courses;
using Microsoft.EntityFrameworkCore;
using Lightaplusplus.Models;
using System.ComponentModel.DataAnnotations;

namespace UnitTestLMSProject
{
    [TestClass]
    public class APIUnitTests
    {
        [TestMethod]
        public void MakeValidPayment()
        {
            // Preparation or setup
            CreditCard myCard = new CreditCard();

            myCard.cardNumber = "4242424242424242";
            myCard.cvc = "111";
            myCard.exp_month = "12";
            myCard.exp_year = "2022";

            // Perform operations
            string isSuccessful = PaymentProcessor.processPayment(myCard, 20.00);

            // Analyze results
            Assert.IsTrue(isSuccessful == "succeeded");
        }

        [TestMethod]
        public void MakeInvalidPayment()
        {
            // Preparation or setup
            CreditCard myCard = new CreditCard();

            myCard.cardNumber = "4242424242424242";
            myCard.cvc = "111";
            myCard.exp_month = "12";
            myCard.exp_year = "2022";

            // Perform operations
            string isSuccessful = PaymentProcessor.processPayment(myCard, -20.00);

            // Analyze results
            Assert.IsFalse(isSuccessful == "succeeded");
        }
    }
}
