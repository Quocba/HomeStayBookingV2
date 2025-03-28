using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Common;

namespace BusinessObject.Entities
{
    [Table("HomeStay")]
    public class HomeStay : BaseEntity<Guid>
    {
        [MaxLength(255)]
        public string MainImage {  get; set; }

        [MaxLength(255)]
        public string Name {  get; set; }

        public int OpenIn {  get; set; }

        [MaxLength(255)]
        public string Description {  get; set; }

        public int Standar {  get; set; }

        public bool isDeleted {  get; set; }

        [MaxLength(255)]
        public string? Address {  get; set; }

        [MaxLength(255)]
        public string City { get; set; }


        [MaxLength(255)]
        public string CheckInTime {  get; set; }

        [MaxLength(255)]
        public string CheckOutTime { get; set; }
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        public ICollection<HomestayAmenity> HomestayAmenities { get; set; }
        public ICollection<HomeStayImage> HomestayImages { get; set; }
        public ICollection<Calendar> Calendars { get; set; }

        public ICollection<FeedBack> FeedBacks { get; set; }

        public ICollection<HomeStayFacility> HomestayFacilities { get; set; }

        public ICollection<TTlockAccuont> TTlockAccuonts { get; set; }

    }
}
