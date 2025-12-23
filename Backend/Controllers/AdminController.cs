using DigitalBankLite.API.DTOs;
using DigitalBankLite.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBankLite.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("customers/pending")]
        public IActionResult GetPendingApprovals()
        {
            var result = _adminService.GetPendingApprovals();
            return Ok(result);
        }

        [HttpPut("customers/{id}/approve")]
        public IActionResult ApproveCustomer(int id)
        {
            var result = _adminService.ApproveCustomer(id);
            if (!result.Success) return NotFound(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpPut("customers/{id}/reject")]
        public IActionResult RejectCustomer(int id)
        {
            var result = _adminService.RejectCustomer(id);
            if (!result.Success) return NotFound(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpGet("accounts")]
        public IActionResult GetAllAccounts()
        {
            var result = _adminService.GetAllAccounts();
            return Ok(result);
        }

        [HttpDelete("accounts/{id}")]
        public IActionResult DeleteAccount(int id)
        {
            var result = _adminService.DeleteAccount(id);
            if (!result.Success) return BadRequest(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpGet("transactions/highvalue")]
        public IActionResult GetHighValueTransactions([FromQuery] decimal threshold = 100000)
        {
            var result = _adminService.GetHighValueTransactions(threshold);
            return Ok(result);
        }

        [HttpGet("servicerequests")]
        public IActionResult GetAllServiceRequests()
        {
            var result = _adminService.GetAllServiceRequests();
            return Ok(result);
        }

        [HttpPut("servicerequests/{id}")]
        public IActionResult UpdateServiceRequestStatus(int id, [FromBody] UpdateServiceRequestStatusDto dto)
        {
            var result = _adminService.UpdateServiceRequestStatus(id, dto.Status);
            if (!result.Success) return NotFound(new { message = result.Message });
            return Ok(new { message = result.Message });
        }

        [HttpDelete("servicerequests/{id}")]
        public IActionResult DeleteServiceRequest(int id)
        {
            var result = _adminService.DeleteServiceRequest(id);
             if (!result.Success) 
             {
                 if (result.Message == "Request not found.") return NotFound(new { message = result.Message });
                 return BadRequest(new { message = result.Message });
             }
            return Ok(new { message = result.Message });
        }
    }
}
