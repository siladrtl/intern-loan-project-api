using internLoanProjectAPI.Application.Abstractions.Services;
using internLoanProjectAPI.Application.DTOs.Application;
using Microsoft.AspNetCore.Mvc;

namespace internLoanProjectAPI.API.Controllers
{
    public class AdminLoanApplicationController : Controller
    {
        private readonly IAdminLoanApplicationService _adminLoanApplicationService;

        public AdminLoanApplicationController(IAdminLoanApplicationService adminLoanApplicationService)
        {
            _adminLoanApplicationService = adminLoanApplicationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var applications = await _adminLoanApplicationService.GetAllAsync();


                return Ok(applications);
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        message =
                            ex.Message
                    }
                );
            }
        }

        [HttpPut("{applicationId:int}/approve")]
        public async Task<IActionResult> Approve(
            int applicationId,
            [FromBody] LoanApplicationDecisionDto? request)
        {
            try
            {
                var result =
                    await _adminLoanApplicationService
                        .ApproveAsync(
                            applicationId,
                            request?.Note
                        );


                return Ok(
                    result
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        message =
                            ex.Message
                    }
                );
            }
        }

        [HttpPut("{applicationId:int}/reject")]
        public async Task<IActionResult> Reject(
            int applicationId,
            [FromBody] LoanApplicationDecisionDto? request)
        {
            try
            {
                var result =
                    await _adminLoanApplicationService
                        .RejectAsync(
                            applicationId,
                            request?.Note
                        );


                return Ok(
                    result
                );
            }
            catch (Exception ex)
            {
                return BadRequest(
                    new
                    {
                        message =
                            ex.Message
                    }
                );
            }
        }
    }
}
