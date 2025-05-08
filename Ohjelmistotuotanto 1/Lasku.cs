using System;

namespace Ohjelmistotuotanto_1
{
    // Model class for invoice data
    public class Lasku
    {
        // Primary key
        public int LaskuId { get; set; }
        
        // Foreign key to the reservation
        public int VarausId { get; set; }
        
        // Financial information
        public double Summa { get; set; }
        public double Alv { get; set; }
        public bool Maksettu { get; set; }
        
        // Additional properties for display purposes
        public string LaskunTiedot => $"Lasku #{LaskuId} - Varaus #{VarausId} - {Summa:C} (ALV {Alv}€) - {(Maksettu ? "Maksettu" : "Maksamaton")}";
        
        // Extended properties to show more details when needed
        public DateTime LuontiPvm { get; set; }
        public string AsiakkaanNimi { get; set; }
        public string MokinNimi { get; set; }
        public DateTime AlkuPvm { get; set; }
        public DateTime LoppuPvm { get; set; }
        
        // Default constructor
        public Lasku()
        {
            LuontiPvm = DateTime.Now;
            Maksettu = false;
        }
    }
} 