using FluentValidation;
using internLoanProjectAPI.Application.DTOs.Auth;
using System;
using System.Linq;

namespace internLoanProjectAPI.Application.Validators.Auth
{
    public class RegisterRequestDtoValidator: AbstractValidator<RegisterRequestDto>
    {
        private const string LetterPattern = @"^[a-zA-ZçÇğĞıİöÖşŞüÜ\s'-]+$";

        private const string EmailPattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";


        public RegisterRequestDtoValidator()
        {

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Ad alanı zorunludur.")

                .MinimumLength(2)
                .WithMessage("Ad en az 2 karakter olmalıdır.")

                .MaximumLength(50)
                .WithMessage("Ad en fazla 50 karakter olabilir.")

                .Matches(LetterPattern)
                .WithMessage(
                    "Ad yalnızca harflerden oluşmalıdır."
                );


            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Soyad alanı zorunludur.")

                .MinimumLength(2)
                .WithMessage("Soyad en az 2 karakter olmalıdır.")

                .MaximumLength(50)
                .WithMessage("Soyad en fazla 50 karakter olabilir.")

                .Matches(LetterPattern)
                .WithMessage(
                    "Soyad yalnızca harflerden oluşmalıdır."
                );


            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("E-posta alanı zorunludur.")

                .MaximumLength(150)
                .WithMessage(
                    "E-posta en fazla 150 karakter olabilir."
                )

                .EmailAddress()
                .WithMessage(
                    "Geçerli bir e-posta adresi giriniz."
                )

                .Matches(EmailPattern)
                .WithMessage(
                    "Geçerli bir e-posta adresi giriniz."
                );

            RuleFor(x => x.NationalId)
                .NotEmpty()
                .WithMessage(
                    "TC Kimlik Numarası zorunludur."
                )

                .Must(BeValidTurkishIdentityNumber)
                .WithMessage(
                    "Geçerli bir TC Kimlik Numarası giriniz."
                );


            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage(
                    "Telefon numarası zorunludur."
                )

                .Matches(@"^5\d{9}$")
                .WithMessage(
                    "Telefon numarasını 5XXXXXXXXX formatında giriniz."
                );


            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .WithMessage(
                    "Doğum tarihi zorunludur."
                )

                .Must(BeValidBirthDate)
                .WithMessage(
                    "Geçerli bir doğum tarihi giriniz."
                )

                .Must(BeAtLeast18YearsOld)
                .WithMessage(
                    "Kayıt olabilmek için en az 18 yaşında olmalısınız."
                );

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage(
                    "Şehir alanı zorunludur."
                )

                .MinimumLength(2)
                .WithMessage(
                    "Şehir en az 2 karakter olmalıdır."
                )

                .MaximumLength(50)
                .WithMessage(
                    "Şehir en fazla 50 karakter olabilir."
                )

                .Matches(LetterPattern)
                .WithMessage(
                    "Şehir yalnızca harflerden oluşmalıdır."
                );


            RuleFor(x => x.District)
                .NotEmpty()
                .WithMessage(
                    "İlçe alanı zorunludur."
                )

                .MinimumLength(2)
                .WithMessage(
                    "İlçe en az 2 karakter olmalıdır."
                )

                .MaximumLength(50)
                .WithMessage(
                    "İlçe en fazla 50 karakter olabilir."
                )

                .Matches(LetterPattern)
                .WithMessage(
                    "İlçe yalnızca harflerden oluşmalıdır."
                );

            RuleFor(x => x.CustomerType)
                .IsInEnum()
                .WithMessage(
                    "Geçerli bir müşteri tipi seçiniz."
                );

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(
                    "Şifre alanı zorunludur."
                )

                .MinimumLength(8)
                .WithMessage(
                    "Şifre en az 8 karakter olmalıdır."
                )

                .MaximumLength(64)
                .WithMessage(
                    "Şifre en fazla 64 karakter olabilir."
                )

                .Matches("[A-Z]")
                .WithMessage(
                    "Şifre en az bir büyük harf içermelidir."
                )

                .Matches("[a-z]")
                .WithMessage(
                    "Şifre en az bir küçük harf içermelidir."
                )

                .Matches("[0-9]")
                .WithMessage(
                    "Şifre en az bir rakam içermelidir."
                );
        }

        private static bool BeValidBirthDate(
            DateTime birthDate)
        {
            if (birthDate == default)
            {
                return false;
            }

            return birthDate.Date <=
                   DateTime.Today;
        }

        private static bool BeAtLeast18YearsOld(
            DateTime birthDate)
        {
            if (birthDate == default)
            {
                return false;
            }

            return birthDate.Date <=
                   DateTime.Today.AddYears(-18);
        }


        private static bool BeValidTurkishIdentityNumber(
            string nationalId)
        {
            if (
                string.IsNullOrWhiteSpace(
                    nationalId
                )
            )
            {
                return false;
            }


            if (
                nationalId.Length != 11
            )
            {
                return false;
            }


            if (
                !nationalId.All(
                    char.IsDigit
                )
            )
            {
                return false;
            }


            if (
                nationalId[0] == '0'
            )
            {
                return false;
            }


            int[] digits =
                nationalId
                    .Select(
                        x => x - '0'
                    )
                    .ToArray();


            int oddSum =
                digits[0] +
                digits[2] +
                digits[4] +
                digits[6] +
                digits[8];


            int evenSum =
                digits[1] +
                digits[3] +
                digits[5] +
                digits[7];


            int tenthDigit =
                ((oddSum * 7) - evenSum) %
                10;


            if (
                tenthDigit < 0
            )
            {
                tenthDigit += 10;
            }


            if (
                digits[9] != tenthDigit
            )
            {
                return false;
            }


            int firstTenSum =
                digits
                    .Take(10)
                    .Sum();


            return
                digits[10] ==
                firstTenSum % 10;
        }
    }
}