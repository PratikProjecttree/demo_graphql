// using demo_graphql.Filters;
// using demo_graphql.Models;
// using demo_graphql.Models.EmailModels;
// using Microsoft.AspNetCore.Mvc;

// namespace demo_graphql.Controllers
// {
//     [ApiController]
//     [Route("api/[controller]")]
//     public class EmailQueueController : ControllerBase
//     {
//         private readonly IEmailQueueService _emailQueueService;

//         public EmailQueueController(IEmailQueueService emailQueueService)
//         {
//             _emailQueueService = emailQueueService;
//         }
//         [HttpPost("")]
//         public async Task<IActionResult> AddOrUpdateEmailQueue(EmailQueueRequest model)
//         {
//             try
//             {
//                 return Ok(await _emailQueueService.AddOrUpdateEmailQueue(model));
//             }
//             catch (Exception ex)
//             {
//                 if (ex is BadHttpRequestException)
//                 {
//                     return BadRequest(new ListResponse<dynamic>
//                     {
//                         Message = ex.Message,
//                         Succeeded = false,
//                         StatusCode = 400
//                     });
//                 }
//                 throw;
//             }
//         }
//     }
// }
