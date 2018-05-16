using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UNMHockeySite.Models;
using UNMHockeySite.Services;

namespace UNMHockeySite.Controllers
{
    public class FormController : Controller
    {
        [HttpGet]
        public ActionResult SeasonTickets()
        {
            //This method is called when you arrive on the season tickets page
            EmailFormModel model = new EmailFormModel();
            return View(model);
        }

        public ActionResult OrderSuccess()
        {
            return View();
        }

        [HttpPost]
        [ActionName("SeasonTickets")]
        public async Task<ActionResult> SendEmailOrder(EmailFormModel model)
        {
            //This method gets called when the submit button is pressed

            //Get the captcha resposne
            var response = Request.Form["g-recaptcha-response"];

            //Call the verify user method to see if the respose is good
            bool verifiedUser = await CaptchaService.VerifyUser(response);

            //Check if the message is a link
            bool isLink = (model.Message.Contains(".com") ||
                            model.Message.Contains("www.") ||
                            model.Message.Contains(".edu") ||
                            model.Message.Contains(".net") ||
                            model.Message.Contains(".org"));
                //if the form is filled out, and the user is verified
            if (ModelState.IsValid && verifiedUser && !isLink)
            {
                //send the email for the order
                EmailService.SendOrderEmail(model);
                //send them to the order success page
                return RedirectToAction("OrderSuccess");
            }
            //if they didn't fill out the form or they aren't verified
            //send them back to the page
            return View(model);
        }
    }
}