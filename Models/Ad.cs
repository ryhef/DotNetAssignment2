using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Assignment2.Models
{
    public class Ad
    {   
        public int AdId { get; set; }
        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; }
        [Required]
        [Display(Name = "Question")]
        public string Url { get; set; }
        public string brokerageId { get; set; }
    }
}
