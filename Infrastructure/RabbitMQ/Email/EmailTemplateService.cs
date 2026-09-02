using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace internLoanProjectAPI.RabbitMQ.Email
{
    public class EmailTemplateService: IEmailTemplateService
    {
        public string CreateApprovedLoanApplicationEmail(
         LoanApplication application)
        {
            var culture =
                CultureInfo.GetCultureInfo(
                    "tr-TR"
                );


            var applicationNumber =
                $"KRD-{application.Id:D6}";


            return $@"
<!DOCTYPE html>
<html>

<body style='
    margin:0;
    padding:25px;
    background:#f4f4f4;
    font-family:Arial, Helvetica, sans-serif;
'>

<div style='
    max-width:650px;
    margin:0 auto;
    background:#ffffff;
    border:1px solid #e5e5e5;
    border-radius:12px;
    overflow:hidden;
'>

    <div style='
        padding:22px 28px;
        background:#2c2f31;
        border-bottom:4px solid #ffcc00;
    '>

        <div style='
            color:#ffffff;
            font-size:20px;
            font-weight:700;
        '>
            Kredi Uygulaması
        </div>

    </div>


    <div style='padding:30px;'>

        <div style='
            color:#9a7600;
            font-size:12px;
            font-weight:bold;
            letter-spacing:1px;
        '>
            BAŞVURU SONUCU
        </div>


        <h2 style='
            margin:8px 0 20px;
            color:#222222;
        '>
            Kredi Başvurunuz Onaylandı
        </h2>


        <p style='
            color:#555555;
            font-size:15px;
        '>
            Merhaba
            <strong>
                {application.Customer.FirstName}
                {application.Customer.LastName}
            </strong>,
        </p>


        <p style='
            color:#555555;
            line-height:1.6;
        '>
            Kredi başvurunuz değerlendirilmiş ve
            <strong>onaylanmıştır.</strong>
        </p>


        <div style='
            margin:25px 0;
            padding:20px;
            background:#fafafa;
            border:1px solid #eeeeee;
            border-radius:10px;
        '>

            <p>
                <strong>Başvuru No:</strong>
                {applicationNumber}
            </p>

            <p>
                <strong>Banka:</strong>
                {application.LoanProduct.Bank.Name}
            </p>

            <p>
                <strong>Kredi Ürünü:</strong>
                {application.LoanProduct.Name}
            </p>

            <p>
                <strong>Kredi Tutarı:</strong>
                {application.LoanCalculation.Amount.ToString("N2", culture)}
                TL
            </p>

            <p>
                <strong>Vade:</strong>
                {application.LoanCalculation.Term}
                Ay
            </p>

            <p>
                <strong>Aylık Taksit:</strong>
                {application.LoanCalculation.MonthlyInstallment.ToString("N2", culture)}
                TL
            </p>

        </div>


        <div style='
            padding:18px;
            background:#fff9db;
            border:1px solid #ffcc00;
            border-radius:10px;
        '>

            <div style='
                color:#777777;
                font-size:11px;
                font-weight:bold;
            '>
                BAŞVURU NUMARANIZ
            </div>


            <div style='
                margin-top:6px;
                color:#222222;
                font-size:22px;
                font-weight:bold;
                letter-spacing:1px;
            '>
                {applicationNumber}
            </div>

        </div>


        <p style='
            margin-top:25px;
            color:#666666;
            line-height:1.6;
        '>
            Kredi ödeme planınız PDF olarak
            bu e-postaya eklenmiştir.
        </p>


        <p style='
            margin-top:30px;
            color:#999999;
            font-size:12px;
        '>
            Bu e-posta otomatik olarak gönderilmiştir.
        </p>

    </div>

</div>

</body>

</html>";
        }


        // ==========================================
        // RED MAILİ
        // ==========================================

        public string CreateRejectedLoanApplicationEmail(
            LoanApplication application)
        {
            var applicationNumber =
                $"KRD-{application.Id:D6}";


            var decisionNoteHtml =
                string.IsNullOrWhiteSpace(
                    application.DecisionNote
                )
                    ? string.Empty

                    : $@"
                    <div style='
                        margin:20px 0;
                        padding:18px;
                        background:#fff6f6;
                        border:1px solid #efc2c2;
                        border-radius:10px;
                        color:#8d3030;
                    '>

                        <strong>
                            Değerlendirme Notu
                        </strong>

                        <p style='
                            margin-bottom:0;
                        '>
                            {application.DecisionNote}
                        </p>

                    </div>";


            return $@"
<!DOCTYPE html>
<html>

<body style='
    margin:0;
    padding:25px;
    background:#f4f4f4;
    font-family:Arial, Helvetica, sans-serif;
'>

<div style='
    max-width:650px;
    margin:0 auto;
    background:#ffffff;
    border:1px solid #e5e5e5;
    border-radius:12px;
    overflow:hidden;
'>

    <div style='
        padding:22px 28px;
        background:#2c2f31;
        border-bottom:4px solid #ffcc00;
    '>

        <div style='
            color:#ffffff;
            font-size:20px;
            font-weight:700;
        '>
            Kredi Uygulaması
        </div>

    </div>


    <div style='padding:30px;'>

        <div style='
            color:#9a7600;
            font-size:12px;
            font-weight:bold;
            letter-spacing:1px;
        '>
            BAŞVURU SONUCU
        </div>


        <h2 style='
            margin:8px 0 20px;
            color:#222222;
        '>
            Kredi Başvurunuz Sonuçlandı
        </h2>


        <p style='
            color:#555555;
        '>
            Merhaba
            <strong>
                {application.Customer.FirstName}
                {application.Customer.LastName}
            </strong>,
        </p>


        <p style='
            color:#555555;
            line-height:1.6;
        '>
            <strong>
                {application.LoanProduct.Bank.Name}
            </strong>

            bünyesindeki

            <strong>
                {application.LoanProduct.Name}
            </strong>

            başvurunuz değerlendirilmiş ve
            reddedilmiştir.
        </p>


        <div style='
            margin:25px 0;
            padding:18px;
            background:#fafafa;
            border:1px solid #eeeeee;
            border-radius:10px;
        '>

            <strong>
                Başvuru No:
            </strong>

            {applicationNumber}

        </div>


        {decisionNoteHtml}


        <p style='
            margin-top:30px;
            color:#999999;
            font-size:12px;
        '>
            Bu e-posta otomatik olarak gönderilmiştir.
        </p>

    </div>

</div>

</body>

</html>";
        }
    }
}
