using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models{
    public class User{

        public class PasswordCheck : ValidationAttribute{
            protected override ValidationResult IsValid(object value, ValidationContext validationContext){
                string pass = (string)value;
                if(value == null){
                    return new ValidationResult("Must enter a password");
                }
                if(pass.Length < 8){
                    return new ValidationResult("Password must be at least 8 characters");
                }
                return ValidationResult.Success;
            }
        }

        [Key]
        public int UserId {get; set;}

        [Required(ErrorMessage="You must enter a first name!")]
        [MinLength(2, ErrorMessage="First name must be at least 2 characters!")]
        [Display(Name="First Name: ")]
        public string FirstName{get; set;}

        [Required(ErrorMessage="You must enter a last name!")]
        [MinLength(2, ErrorMessage="Last name must be at least 2 characters!")]
        [Display(Name="Last Name: ")]
        public string LastName{get; set;}

        [Required(ErrorMessage="You must enter an email!")]
        [Display(Name="Email: ")]
        [DataType(DataType.EmailAddress)]
        public string Email{get; set;}

        [Required(ErrorMessage="You must enter a password!")]
        [PasswordCheck]
        [Display(Name="Password: ")]
        public string Password{get; set;}

        [Required(ErrorMessage=" ")]
        [NotMapped]
        [Display(Name="Confirm password: ")]
        public string PassConf{get; set;}


        public List<Guest> Attending{get; set;}
        public User(){
            Attending = new List<Guest>();
        }

        public DateTime CreatedAt{get; set;}
        public DateTime UpdatedAt{get; set;}
    }
}