using internLoanProject.Domain.Entities;
using internLoanProjectAPI.Application.Abstractions.Messaging;
using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Document = QuestPDF.Fluent.Document;

namespace internLoanProjectAPI.RabbitMQ.Email
{
    public class LoanPaymentPlanPdfService:ILoanPaymentPlanPdfService
    {
        public LoanPaymentPlanPdfService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

   
        public byte[] Create(LoanApplication application)
        {
            var culture = CultureInfo.GetCultureInfo("tr-TR");
            var applicationNumber = $"KRD-{application.Id:D6}";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // ==================================
                    // SAYFA AYARLARI
                    // ==================================
                    page.Size(PageSizes.A4);
                    page.Margin(25);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    // ==================================
                    // HEADER
                    // ==================================
                    page.Header().Column(column =>
                    {
                        column.Item()
                            .Text("KREDİ ÖDEME PLANI")
                            .FontSize(20)
                            .Bold();

                        column.Item()
                            .PaddingTop(5)
                            .Text(application.LoanProduct.Bank.Name)
                            .FontSize(15)
                            .SemiBold();

                        column.Item()
                            .Text(application.LoanProduct.Name)
                            .FontSize(11);

                        column.Item()
                            .PaddingTop(6)
                            .Text($"Başvuru No: {applicationNumber}")
                            .FontSize(10)
                            .Bold();
                    });

                    // ==================================
                    // CONTENT
                    // ==================================
                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            // ==========================
                            // BAŞVURU BİLGİLERİ
                            // ==========================
                            column.Item()
                                .Text("Başvuru Bilgileri")
                                .FontSize(14)
                                .Bold();

                            column.Item()
                                .PaddingTop(8)
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                    });

                                    AddInfoRow(table, "Müşteri", $"{application.Customer.FirstName} {application.Customer.LastName}");
                                    AddInfoRow(table, "Banka", application.LoanProduct.Bank.Name);
                                    AddInfoRow(table, "Kredi Ürünü", application.LoanProduct.Name);
                                    AddInfoRow(table, "Kredi Tutarı", $"{application.LoanCalculation.Amount.ToString("N2", culture)} TL");
                                    AddInfoRow(table, "Vade", $"{application.LoanCalculation.Term} Ay");
                                    AddInfoRow(table, "Aylık Faiz Oranı", $"%{application.LoanCalculation.InterestRate.ToString("N2", culture)}");
                                    AddInfoRow(table, "Aylık Taksit", $"{application.LoanCalculation.MonthlyInstallment.ToString("N2", culture)} TL");
                                    AddInfoRow(table, "Toplam Ödeme", $"{application.LoanCalculation.TotalPayment.ToString("N2", culture)} TL");
                                    AddInfoRow(table, "Toplam Faiz", $"{application.LoanCalculation.TotalInterest.ToString("N2", culture)} TL");
                                });

                            // ==========================
                            // TAKSİT DETAYLARI
                            // ==========================
                            column.Item()
                                .PaddingTop(25)
                                .Text("Taksit Detayları")
                                .FontSize(14)
                                .Bold();

                            column.Item()
                                .PaddingTop(10)
                                .Table(table =>
                                {
                                    // ==================================
                                    // SÜTUNLAR
                                    // ==================================
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.ConstantColumn(28);
                                        columns.RelativeColumn(1.25f);
                                        columns.RelativeColumn(1.15f);
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn();
                                        columns.RelativeColumn(1.1f);
                                        columns.RelativeColumn(1.25f);
                                    });

                                    // ==================================
                                    // TABLO BAŞLIKLARI
                                    // ==================================
                                    table.Header(header =>
                                    {
                                        AddHeaderCell(header, "No");
                                        AddHeaderCell(header, "Tarih");
                                        AddHeaderCell(header, "Taksit");
                                        AddHeaderCell(header, "Faiz");
                                        AddHeaderCell(header, "KKDF");
                                        AddHeaderCell(header, "BSMV");
                                        AddHeaderCell(header, "Anapara");
                                        AddHeaderCell(header, "Kalan");
                                    });

                                    // ==================================
                                    // TAKSİT SATIRLARI
                                    // ==================================
                                    foreach (var item in application.LoanCalculation.PaymentPlans.OrderBy(x => x.InstallmentNumber))
                                    {
                                        AddCell(table, item.InstallmentNumber.ToString());
                                        AddCell(table, item.DueDate.ToString("dd.MM.yyyy"));
                                        AddCell(table, item.InstallmentAmount.ToString("N2", culture));
                                        AddCell(table, item.InterestAmount.ToString("N2", culture));
                                        AddCell(table, item.KkdfAmount.ToString("N2", culture));
                                        AddCell(table, item.BsmvAmount.ToString("N2", culture));
                                        AddCell(table, item.PrincipalAmount.ToString("N2", culture));
                                        AddCell(table, item.RemainingPrincipal.ToString("N2", culture));
                                    }
                                });

                            // ==========================
                            // BİLGİLENDİRME
                            // ==========================
                            column.Item()
                                .PaddingTop(20)
                                .Text("Bu ödeme planı kredi başvurunuza ait bilgilendirme amacıyla oluşturulmuştur.")
                                .FontSize(8)
                                .FontColor(Colors.Grey.Darken1);
                        });

                    // ==================================
                    // FOOTER
                    // ==================================
                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken1));
                            text.Span("Kredi Uygulaması • Sayfa ");
                            text.CurrentPageNumber();
                            text.Span(" / ");
                            text.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
        }

        // ==========================================
        // BİLGİ SATIRI
        // ==========================================
        private static void AddInfoRow(TableDescriptor table, string title, string value)
        {
            table.Cell()
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(7)
                .Text(title)
                .SemiBold();

            table.Cell()
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2)
                .Padding(7)
                .Text(value);
        }

        // ==========================================
        // TABLO BAŞLIK HÜCRESİ
        // ==========================================
        private static void AddHeaderCell(TableCellDescriptor header, string text)
        {
            header.Cell()
                .Background(Colors.Grey.Lighten2)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten1)
                .PaddingVertical(6)
                .PaddingHorizontal(3)
                .Text(text)
                .FontSize(8)
                .SemiBold();
        }

        // ==========================================
        // TABLO NORMAL HÜCRESİ
        // ==========================================
        private static void AddCell(TableDescriptor table, string text)
        {
            table.Cell()
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten3)
                .PaddingVertical(5)
                .PaddingHorizontal(3)
                .Text(text)
                .FontSize(7.5f);
        }
    }
}

