using FluentValidation;
using internLoanProjectAPI.API.Models;


namespace internLoanProjectAPI.API.Validators
{
    public class RegisterFormRequestValidator: AbstractValidator<RegisterFormRequest>
    {
        private static readonly string[] AllowedExtensions =
        {
            ".pdf",
            ".jpg",
            ".jpeg",
            ".png"
        };

        private const long MaxFileSize = 5 * 1024 * 1024;

        public RegisterFormRequestValidator()
        {
            // Belge zorunlu
            RuleFor(x => x.VerificationDocument)
                .NotNull()
                .WithMessage(
                    "Müşteri tipinizi doğrulamak için belge yüklemelisiniz."
                );

            // Belge boş olmamalı
            RuleFor(x => x.VerificationDocument)
                .Must(file => file != null && file.Length > 0)
                .WithMessage(
                    "Yüklenen belge boş olamaz."
                )
                .When(x => x.VerificationDocument != null);

            // Dosya uzantısı kontrolü
            RuleFor(x => x.VerificationDocument)
                .Must(file =>
                {
                    if (file == null)
                        return false;

                    var extension = Path
                        .GetExtension(file.FileName)
                        .ToLowerInvariant();

                    return AllowedExtensions.Contains(extension);
                })
                .WithMessage(
                    "Sadece PDF, JPG, JPEG veya PNG formatında belge yükleyebilirsiniz."
                )
                .When(x => x.VerificationDocument != null);

            // Maksimum dosya boyutu kontrolü
            RuleFor(x => x.VerificationDocument)
                .Must(file =>
                    file != null &&
                    file.Length <= MaxFileSize
                )
                .WithMessage(
                    "Belge boyutu en fazla 5 MB olabilir."
                )
                .When(x => x.VerificationDocument != null);
        }
    }
}