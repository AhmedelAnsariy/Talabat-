using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIS.Errors;
using Talabat.Repository.Data;

namespace Talabat.APIS.Controllers
{
   
    public class BuggyController : BaseApiController
    {
        private readonly StoreDbContext _context;

        public BuggyController(StoreDbContext  context )
        {
            _context = context;
        }


        [HttpGet("NotFound")]
        public ActionResult GetNotFoundRequest()
        {
            var Product = _context.Products.Find(1000);
            if(Product is null)
            {
                return NotFound(new ApiResponse(404));
            }

            return Ok(Product);
        }


        [HttpGet("servererror")]
        public ActionResult GetServerError()
        {
            var Product = _context.Products.Find(1000);
            var result = Product.ToString();   // throw exception Null Reference Exception
            return Ok(result);

        }


        [HttpGet("badrequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }


        [HttpGet("badrequest/{id}")]
        public ActionResult GetBadRequest(int? id)
        {
            return Ok();
        }


        [HttpGet("Unauthorized")]
        public ActionResult GetUnauthorized()
        {
            return Unauthorized(new ApiResponse(401));
        }











    }
}
