using Microsoft.AspNetCore.Identity;

namespace CarService.Services
{
    public class PolishIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
            => Create(nameof(DefaultError), "Wystąpił nieznany błąd.");

        public override IdentityError ConcurrencyFailure()
            => Create(nameof(ConcurrencyFailure), "Wystąpił błąd współbieżności.");

        public override IdentityError PasswordMismatch()
            => Create(nameof(PasswordMismatch), "Nieprawidłowe hasło.");

        public override IdentityError InvalidToken()
            => Create(nameof(InvalidToken), "Nieprawidłowy token.");

        public override IdentityError LoginAlreadyAssociated()
            => Create(nameof(LoginAlreadyAssociated), "To konto zewnętrzne jest już powiązane.");

        public override IdentityError InvalidUserName(string userName)
            => Create(nameof(InvalidUserName), $"Nazwa użytkownika '{userName}' jest nieprawidłowa.");

        public override IdentityError InvalidEmail(string email)
            => Create(nameof(InvalidEmail), $"Adres e-mail '{email}' jest nieprawidłowy.");

        public override IdentityError DuplicateUserName(string userName)
            => Create(nameof(DuplicateUserName), $"Nazwa użytkownika '{userName}' jest już zajęta.");

        public override IdentityError DuplicateEmail(string email)
            => Create(nameof(DuplicateEmail), $"Adres e-mail '{email}' jest już używany.");

        public override IdentityError DuplicateRoleName(string role)
            => Create(nameof(DuplicateRoleName), $"Nazwa roli '{role}' już istnieje.");

        public override IdentityError InvalidRoleName(string role)
            => Create(nameof(InvalidRoleName), $"Nazwa roli '{role}' jest nieprawidłowa.");

        public override IdentityError UserAlreadyHasPassword()
            => Create(nameof(UserAlreadyHasPassword), "Użytkownik ma już ustawione hasło.");

        public override IdentityError UserLockoutNotEnabled()
            => Create(nameof(UserLockoutNotEnabled), "Blokada użytkownika nie jest włączona.");

        public override IdentityError UserAlreadyInRole(string role)
            => Create(nameof(UserAlreadyInRole), $"Użytkownik jest już w roli '{role}'.");

        public override IdentityError UserNotInRole(string role)
            => Create(nameof(UserNotInRole), $"Użytkownik nie jest w roli '{role}'.");

        public override IdentityError PasswordTooShort(int length)
            => Create(nameof(PasswordTooShort), $"Hasło musi mieć co najmniej {length} znaków.");

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => Create(nameof(PasswordRequiresNonAlphanumeric), "Hasło musi zawierać znak specjalny.");

        public override IdentityError PasswordRequiresDigit()
            => Create(nameof(PasswordRequiresDigit), "Hasło musi zawierać cyfrę.");

        public override IdentityError PasswordRequiresLower()
            => Create(nameof(PasswordRequiresLower), "Hasło musi zawierać małą literę.");

        public override IdentityError PasswordRequiresUpper()
            => Create(nameof(PasswordRequiresUpper), "Hasło musi zawierać wielką literę.");

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
            => Create(nameof(PasswordRequiresUniqueChars), $"Hasło musi zawierać co najmniej {uniqueChars} unikalne znaki.");

        public override IdentityError RecoveryCodeRedemptionFailed()
            => Create(nameof(RecoveryCodeRedemptionFailed), "Nieprawidłowy kod odzyskiwania.");

        private static IdentityError Create(string code, string description)
        {
            return new IdentityError
            {
                Code = code,
                Description = description
            };
        }
    }
}
