﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Talabat.APIS.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Interfaces;

namespace Talabat.APIS.Controllers
{
    [Authorize]
    public class PaymentController : BaseApiController 
    {
        private readonly IPaymentService _paymentService;
        const string endpointSecret = "whsec_89f4ae497623fc2be00edb24431e5afafc20ebcf50bcd57431a597ccd0bd4c71";


        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }




        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if(basket == null)
            {
                return BadRequest(new ApiResponse(400, "There Is Problem With Your Basket"));
            }

            return Ok(basket);
        }




        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json,
                    Request.Headers["Stripe-Signature"], endpointSecret);


                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                // Handle the event
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                   await  _paymentService.UpdatePaymentIntentToSuccessedOrFailed(paymentIntent.Id , false);

                }
                else if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    await _paymentService.UpdatePaymentIntentToSuccessedOrFailed(paymentIntent.Id, true);

                }
                // ... handle other event types
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }





    }
}
