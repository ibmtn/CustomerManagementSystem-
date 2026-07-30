namespace KcetasWeb.Models
{
    using System;

    public class Abone
    {
        [System.Text.Json.Serialization.JsonPropertyName("aboneId")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault)]
        public int abone_id { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("aboneNo")]
        public string abone_no { get; set; }
        [System.Text.Json.Serialization.JsonPropertyName("aboneTipi")]
        public KcetasWeb.Models.Enums.AboneTipi? abone_tipi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("ad")]
        public string Ad { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("soyad")]
        public string Soyad { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("unvan")]
        public string Unvan { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tckn")]
        public string tckn { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("vkn")]
        public string vkn { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("telefon")]
        public string telefon { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string EmailApi { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("ePosta")]
        public string EPostaApi { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("e_posta")]
        public string e_posta_raw { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("mail")]
        public string MailApi { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("emailAdresi")]
        public string EmailAdresiApi { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("epostaAdresi")]
        public string EPostaAdresiApi { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string e_posta 
        { 
            get 
            {
                if (!string.IsNullOrWhiteSpace(EPostaApi)) return EPostaApi;
                if (!string.IsNullOrWhiteSpace(EmailApi)) return EmailApi;
                if (!string.IsNullOrWhiteSpace(MailApi)) return MailApi;
                if (!string.IsNullOrWhiteSpace(EmailAdresiApi)) return EmailAdresiApi;
                if (!string.IsNullOrWhiteSpace(EPostaAdresiApi)) return EPostaAdresiApi;
                return e_posta_raw;
            }
            set 
            { 
                EPostaApi = value; 
                e_posta_raw = value;
                EmailApi = value;
                MailApi = value;
                EmailAdresiApi = value;
                EPostaAdresiApi = value;
            }
        }

        [System.Text.Json.Serialization.JsonPropertyName("adres")]
        public string AdresApi { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("tebligatAdresi")]
        public string TebligatAdresiApi { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public string Adres 
        {
            get 
            {
                if (!string.IsNullOrWhiteSpace(AdresApi)) return AdresApi;
                if (!string.IsNullOrWhiteSpace(TebligatAdresiApi)) return TebligatAdresiApi;
                return "Adres Bulunamadı";
            }
            set => AdresApi = value;
        }

        [System.Text.Json.Serialization.JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        

        [System.Text.Json.Serialization.JsonIgnore]
        public int AboneId { get => abone_id; set => abone_id = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string AboneNo { get => abone_no; set => abone_no = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public KcetasWeb.Models.Enums.AboneTipi? AboneTipi { get => abone_tipi; set => abone_tipi = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string TcKimlikNo { get => tckn; set => tckn = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string VergiNo { get => vkn; set => vkn = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string Telefon { get => telefon; set => telefon = value; }
        [System.Text.Json.Serialization.JsonIgnore]
        public string EPosta { get => e_posta; set => e_posta = value; }
        [System.Text.Json.Serialization.JsonPropertyName("status")]
        public string Durum { get; set; } = "AKTIF";
        
    }
}
