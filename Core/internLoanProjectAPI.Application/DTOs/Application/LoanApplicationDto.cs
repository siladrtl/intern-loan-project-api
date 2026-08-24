using internLoanProject.Domain.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.Application.DTOs.Application
{
    public class LoanApplicationDto
    {
        public int Id { get; set; }

        public int CustomerId { get; set; }

        public int LoanProductId { get; set; }

        public int LoanCalculationId { get; set; }

        public LoanApplicationStatus Status { get; set; }

        public DateTime ApplicationDate { get; set; }

        public DateTime? DecisionDate { get; set; }

        public string? DecisionNote { get; set; }

    }
}



