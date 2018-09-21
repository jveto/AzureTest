using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models{
    public class Wedding{

        [Key]
        public int WeddingId {get; set;}

        [Required(ErrorMessage="You must enter Wedder One")]
        [Display(Name="Wedder One: ")]
        public string WedderOne{get; set;}

        [Required(ErrorMessage="You must enter Wedder Two")]
        [Display(Name="Wedder Two: ")]
        public string WedderTwo{get; set;}

        [Required(ErrorMessage="You must enter a date")]
        [Display(Name="Date: ")]
        public DateTime Date{get; set;}

        [Required(ErrorMessage="You must enter an address")]
        [Display(Name="Wedding Address: ")]
        public string Address{get; set;}

        public int UserId{get; set;}
        public User Creator{get; set;}
        
        public List<Guest> Guests{get; set;}

        public Wedding(){
            Guests = new List<Guest>();
        }

        public DateTime CreatedAt{get; set;}
        public DateTime UpdatedAt{get; set;}
    }
}