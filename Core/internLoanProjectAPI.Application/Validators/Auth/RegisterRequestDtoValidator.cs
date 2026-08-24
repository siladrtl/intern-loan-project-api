using FluentValidation;
using internLoanProjectAPI.Application.DTOs.Auth;
using System;
using System.Linq;

namespace internLoanProjectAPI.Application.Validators.Auth
{
    public class RegisterRequestDtoValidator
        : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestDtoValidator()
        {
            // ==========================================
            // AD
            // ==========================================

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Ad alanı zorunludur.")
                .MinimumLength(2)
                .WithMessage("Ad en az 2 karakter olmalıdır.")
                .MaximumLength(50)
                .WithMessage("Ad en fazla 50 karakter olabilir.");


            // ==========================================
            // SOYAD
            // ==========================================

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Soyad alanı zorunludur.")
                .MinimumLength(2)
                .WithMessage("Soyad en az 2 karakter olmalıdır.")
                .MaximumLength(50)
                .WithMessage("Soyad en fazla 50 karakter olabilir.");


            
            // E-POSTA
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("E-posta alanı zorunludur.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi giriniz.");


      
            // TC KİMLİK NUMARASI
            RuleFor(x => x.NationalId)
                .NotEmpty()
                .WithMessage("TC Kimlik Numarası zorunludur.")
                .Must(BeValidTurkishIdentityNumber)
                .WithMessage(
                    "Geçerli bir TC Kimlik Numarası giriniz."
                );


            // TELEFON
            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .WithMessage("Telefon numarası zorunludur.")
                .Matches(@"^5\d{9}$")
                .WithMessage(
                    "Telefon numarasını 5XXXXXXXXX formatında giriniz."
                );


            // DOĞUM TARİHİ
            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .WithMessage("Doğum tarihi zorunludur.")
                .Must(BeAtLeast18YearsOld)
                .WithMessage(
                    "Kayıt olabilmek için en az 18 yaşında olmalısınız."
                );


            // ŞEHİR
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("Şehir alanı zorunludur.")
                .MaximumLength(50)
                .WithMessage(
                    "Şehir en fazla 50 karakter olabilir."
                );

            // İLÇE
            RuleFor(x => x.District)
                .NotEmpty()
                .WithMessage("İlçe alanı zorunludur.")
                .MaximumLength(50)
                .WithMessage(
                    "İlçe en fazla 50 karakter olabilir."
                );


            // MÜŞTERİ TİPİ
            RuleFor(x => x.CustomerType)
                .IsInEnum()
                .WithMessage(
                    "Geçerli bir müşteri tipi seçiniz."
                );


            // ŞİFRE
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Şifre alanı zorunludur.")
                .MinimumLength(8)
                .WithMessage(
                    "Şifre en az 8 karakter olmalıdır."
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


        // ==========================================
        // 18 YAŞ KONTROLÜ
        // ==========================================

        private static bool BeAtLeast18YearsOld(
            DateTime birthDate)
        {
            if (birthDate == default)
                return false;

            return birthDate.Date <=
                   DateTime.Today.AddYears(-18);
        }


        // ==========================================
        // TC KİMLİK NUMARASI ALGORİTMA KONTROLÜ
        // ==========================================

        private static bool BeValidTurkishIdentityNumber(
            string nationalId)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                return false;


            // 11 karakter olmalı
            if (nationalId.Length != 11)
                return false;


            // Sadece rakam olmalı
            if (!nationalId.All(char.IsDigit))
                return false;


            // İlk rakam 0 olamaz
            if (nationalId[0] == '0')
                return false;


            int[] digits =
                nationalId
                    .Select(x => x - '0')
                    .ToArray();


            // 1, 3, 5, 7 ve 9. haneler
            int oddSum =
                digits[0] +
                digits[2] +
                digits[4] +
                digits[6] +
                digits[8];


            // 2, 4, 6 ve 8. haneler
            int evenSum =
                digits[1] +
                digits[3] +
                digits[5] +
                digits[7];


            // 10. hane kontrolü
            int tenthDigit =
                ((oddSum * 7) - evenSum) % 10;


            if (tenthDigit < 0)
            {
                tenthDigit += 10;
            }


            if (digits[9] != tenthDigit)
            {
                return false;
            }


            // İlk 10 hanenin toplamı
            int firstTenSum =
                digits
                    .Take(10)
                    .Sum();


            // 11. hane kontrolü
            if (
                digits[10] !=
                firstTenSum % 10
            )
            {
                return false;
            }


            return true;
        }
    }
}