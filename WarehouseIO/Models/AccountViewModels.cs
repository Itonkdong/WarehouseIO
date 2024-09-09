using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WarehouseIO.CustomAttributes;

namespace WarehouseIO.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [MustBeTrue(ErrorMessage = "You must accept Therms Of Service")]
        [Display(Name = "Accept Therms Of Service")]
        public bool AcceptThermsOfService { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class AccountInformationViewModel
    {
        public string Name { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        [Display(Name = "Profile Image")]
        public string? ProfileImage { get; set; }

        [Display(Name = "Date Joined")]
        public DateTime DateJoined { get; set; }

        [Display(Name = "Old Password")]
        public string? OldPassword { get; set; }

        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }
        public string? ErrorMessage { get; set; }

        public AccountInformationViewModel()
        {
            
        }

        private AccountInformationViewModel(ApplicationUser user)
        {
            this.Name = user.Name;
            this.Email = user.Email;
            this.ProfileImage = user.ProfileImage;
            this.DateJoined = user.DateJoined;
        }

        public static AccountInformationViewModel Create(ApplicationUser user)
        {
            return new AccountInformationViewModel(user);
        }

        public static bool operator ==(AccountInformationViewModel left, AccountInformationViewModel right)
        {
            if (left is null || right is null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(AccountInformationViewModel left, AccountInformationViewModel right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            AccountInformationViewModel that = (AccountInformationViewModel)obj;


            return this.Name == that.Name &&
                   this.Email == that.Email &&
                   this.ProfileImage == that.ProfileImage;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
