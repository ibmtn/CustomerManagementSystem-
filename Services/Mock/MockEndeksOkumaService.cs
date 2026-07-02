using KcetasWeb.Models;
using KcetasWeb.Models.entities;
using KcetasWeb.Models.enums;
using KcetasWeb.Services.Interfaces;

namespace KcetasWeb.Services.Mock;

/// <summary>
/// Endeks okuma servisi mock implementasyonu.
/// 35 adet gerçekçi sayaç okuma verisi ile çalışır. ~40% Manuel, ~60% OSOS dağılımı.
/// </summary>
public class MockEndeksOkumaService : IEndeksOkumaService
{
    private static readonly List<EndeksOkuma> _okumalar = new()
    {
        // Manuel okumalar (14 adet - ~40%)
        new EndeksOkuma { OkumaId = 1, TuketimNoktasiId = 1001, SayacId = 5001, SozlesmeId = 3001, SayacSeriNo = "SN-38-001234", TuketimNoktasiKodu = "TK-2026-001", OkumaTarihi = new DateTime(2026, 4, 5), OncekiEndeks = 15200, GuncelEndeks = 15520, TuketimMiktari = 320, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Basarili, OkuyanKullaniciId = 101, OkuyanKullaniciAdi = "Ahmet Yılmaz", DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 4, 5) },
        new EndeksOkuma { OkumaId = 2, TuketimNoktasiId = 1002, SayacId = 5002, SozlesmeId = 3002, SayacSeriNo = "SN-38-002345", TuketimNoktasiKodu = "TK-2026-002", OkumaTarihi = new DateTime(2026, 4, 6), OncekiEndeks = 18400, GuncelEndeks = 18750, TuketimMiktari = 350, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Basarili, OkuyanKullaniciId = 102, OkuyanKullaniciAdi = "Mehmet Demir", DogrulamaDurumu = true, TarifeGrubu = "Ticarethane", CreatedAt = new DateTime(2026, 4, 6) },
        new EndeksOkuma { OkumaId = 3, TuketimNoktasiId = 1003, SayacId = 5003, SozlesmeId = 3003, SayacSeriNo = "SN-38-003456", TuketimNoktasiKodu = "TK-2026-003", OkumaTarihi = new DateTime(2026, 4, 7), OncekiEndeks = 22100, GuncelEndeks = 22550, TuketimMiktari = 450, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Basarili, OkuyanKullaniciId = 103, OkuyanKullaniciAdi = "Ayşe Kaya", DogrulamaDurumu = true, TarifeGrubu = "Sanayi", CreatedAt = new DateTime(2026, 4, 7) },
        new EndeksOkuma { OkumaId = 4, TuketimNoktasiId = 1004, SayacId = 5004, SozlesmeId = 3004, SayacSeriNo = "SN-38-004567", TuketimNoktasiKodu = "TK-2026-004", OkumaTarihi = new DateTime(2026, 4, 8), OncekiEndeks = 16800, GuncelEndeks = 16800, TuketimMiktari = 0, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.SifirTuketim, OkuyanKullaniciId = 101, OkuyanKullaniciAdi = "Ahmet Yılmaz", DogrulamaDurumu = false, AnomaliAciklamasi = "Sıfır tüketim tespit edildi, sayaç kontrolü gerekli", TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 4, 8) },
        new EndeksOkuma { OkumaId = 5, TuketimNoktasiId = 1005, SayacId = 5005, SozlesmeId = 3005, SayacSeriNo = "SN-38-005678", TuketimNoktasiKodu = "TK-2026-005", OkumaTarihi = new DateTime(2026, 4, 10), OncekiEndeks = 19500, GuncelEndeks = 19780, TuketimMiktari = 280, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Basarili, OkuyanKullaniciId = 104, OkuyanKullaniciAdi = "Fatma Çelik", DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 4, 10) },
        new EndeksOkuma { OkumaId = 6, TuketimNoktasiId = 1006, SayacId = 5006, SozlesmeId = 3006, SayacSeriNo = "SN-38-006789", TuketimNoktasiKodu = "TK-2026-006", OkumaTarihi = new DateTime(2026, 4, 12), OncekiEndeks = 20100, GuncelEndeks = 22650, TuketimMiktari = 2550, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Anormal, OkuyanKullaniciId = 102, OkuyanKullaniciAdi = "Mehmet Demir", DogrulamaDurumu = false, AnomaliAciklamasi = "Anormal yüksek tüketim: 2550 kWh. Kaçak kullanım şüphesi", TarifeGrubu = "Ticarethane", CreatedAt = new DateTime(2026, 4, 12) },
        new EndeksOkuma { OkumaId = 7, TuketimNoktasiId = 1007, SayacId = 5007, SozlesmeId = 3007, SayacSeriNo = "SN-38-007890", TuketimNoktasiKodu = "TK-2026-007", OkumaTarihi = new DateTime(2026, 5, 3), OncekiEndeks = 17300, GuncelEndeks = 17680, TuketimMiktari = 380, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Basarili, OkuyanKullaniciId = 105, OkuyanKullaniciAdi = "Mustafa Özkan", DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 5, 3) },
        new EndeksOkuma { OkumaId = 8, TuketimNoktasiId = 1008, SayacId = 5008, SozlesmeId = 3008, SayacSeriNo = "SN-38-008901", TuketimNoktasiKodu = "TK-2026-008", OkumaTarihi = new DateTime(2026, 5, 5), OncekiEndeks = 21000, GuncelEndeks = 21180, TuketimMiktari = 180, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Basarili, OkuyanKullaniciId = 103, OkuyanKullaniciAdi = "Ayşe Kaya", DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 5, 5) },
        new EndeksOkuma { OkumaId = 9, TuketimNoktasiId = 1009, SayacId = 5009, SozlesmeId = 3009, SayacSeriNo = "SN-38-009012", TuketimNoktasiKodu = "TK-2026-009", OkumaTarihi = new DateTime(2026, 5, 7), OncekiEndeks = 24500, GuncelEndeks = 25300, TuketimMiktari = 800, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Basarili, OkuyanKullaniciId = 101, OkuyanKullaniciAdi = "Ahmet Yılmaz", DogrulamaDurumu = true, TarifeGrubu = "Sanayi", CreatedAt = new DateTime(2026, 5, 7) },
        new EndeksOkuma { OkumaId = 10, TuketimNoktasiId = 1010, SayacId = 5010, SozlesmeId = 3010, SayacSeriNo = "SN-38-010123", TuketimNoktasiKodu = "TK-2026-010", OkumaTarihi = new DateTime(2026, 5, 10), OncekiEndeks = 15800, GuncelEndeks = 15920, TuketimMiktari = 120, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Basarili, OkuyanKullaniciId = 104, OkuyanKullaniciAdi = "Fatma Çelik", DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 5, 10) },
        new EndeksOkuma { OkumaId = 11, TuketimNoktasiId = 1011, SayacId = 5011, SozlesmeId = 3011, SayacSeriNo = "SN-38-011234", TuketimNoktasiKodu = "TK-2026-011", OkumaTarihi = new DateTime(2026, 5, 12), OncekiEndeks = 19200, GuncelEndeks = 19200, TuketimMiktari = 0, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.SifirTuketim, OkuyanKullaniciId = 105, OkuyanKullaniciAdi = "Mustafa Özkan", DogrulamaDurumu = false, AnomaliAciklamasi = "Sıfır tüketim, abone taşınmış olabilir", TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 5, 12) },
        new EndeksOkuma { OkumaId = 12, TuketimNoktasiId = 1012, SayacId = 5012, SozlesmeId = 3012, SayacSeriNo = "SN-38-012345", TuketimNoktasiKodu = "TK-2026-012", OkumaTarihi = new DateTime(2026, 6, 2), OncekiEndeks = 17600, GuncelEndeks = 17850, TuketimMiktari = 250, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Basarili, OkuyanKullaniciId = 102, OkuyanKullaniciAdi = "Mehmet Demir", DogrulamaDurumu = true, TarifeGrubu = "Ticarethane", CreatedAt = new DateTime(2026, 6, 2) },
        new EndeksOkuma { OkumaId = 13, TuketimNoktasiId = 1013, SayacId = 5013, SozlesmeId = 3013, SayacSeriNo = "SN-38-013456", TuketimNoktasiKodu = "TK-2026-013", OkumaTarihi = new DateTime(2026, 6, 5), OncekiEndeks = 16400, GuncelEndeks = 16750, TuketimMiktari = 350, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Basarili, OkuyanKullaniciId = 103, OkuyanKullaniciAdi = "Ayşe Kaya", DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 6, 5) },
        new EndeksOkuma { OkumaId = 14, TuketimNoktasiId = 1014, SayacId = 5014, SozlesmeId = 3014, SayacSeriNo = "SN-38-014567", TuketimNoktasiKodu = "TK-2026-014", OkumaTarihi = new DateTime(2026, 6, 8), OncekiEndeks = 23100, GuncelEndeks = 23500, TuketimMiktari = 400, OkumaKaynagi = OkumaKaynagi.Manuel, OkumaDurumu = OkumaDurumu.Hatali, OkuyanKullaniciId = 101, OkuyanKullaniciAdi = "Ahmet Yılmaz", DogrulamaDurumu = false, AnomaliAciklamasi = "Sayaç ekranında hata kodu görüntülendi", TarifeGrubu = "Sanayi", CreatedAt = new DateTime(2026, 6, 8) },

        // OSOS okumaları (21 adet - ~60%)
        new EndeksOkuma { OkumaId = 15, TuketimNoktasiId = 1015, SayacId = 5015, SozlesmeId = 3015, SayacSeriNo = "SN-38-015678", TuketimNoktasiKodu = "TK-2026-015", OkumaTarihi = new DateTime(2026, 4, 5), OncekiEndeks = 18900, GuncelEndeks = 19250, TuketimMiktari = 350, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 4, 5) },
        new EndeksOkuma { OkumaId = 16, TuketimNoktasiId = 1016, SayacId = 5016, SozlesmeId = 3016, SayacSeriNo = "SN-38-016789", TuketimNoktasiKodu = "TK-2026-016", OkumaTarihi = new DateTime(2026, 4, 5), OncekiEndeks = 21500, GuncelEndeks = 21920, TuketimMiktari = 420, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Ticarethane", CreatedAt = new DateTime(2026, 4, 5) },
        new EndeksOkuma { OkumaId = 17, TuketimNoktasiId = 1017, SayacId = 5017, SozlesmeId = 3017, SayacSeriNo = "SN-38-017890", TuketimNoktasiKodu = "TK-2026-017", OkumaTarihi = new DateTime(2026, 4, 6), OncekiEndeks = 24800, GuncelEndeks = 25450, TuketimMiktari = 650, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Sanayi", CreatedAt = new DateTime(2026, 4, 6) },
        new EndeksOkuma { OkumaId = 18, TuketimNoktasiId = 1018, SayacId = 5018, SozlesmeId = 3018, SayacSeriNo = "SN-38-018901", TuketimNoktasiKodu = "TK-2026-018", OkumaTarihi = new DateTime(2026, 4, 8), OncekiEndeks = 16200, GuncelEndeks = 16500, TuketimMiktari = 300, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 4, 8) },
        new EndeksOkuma { OkumaId = 19, TuketimNoktasiId = 1019, SayacId = 5019, SozlesmeId = 3019, SayacSeriNo = "SN-38-019012", TuketimNoktasiKodu = "TK-2026-019", OkumaTarihi = new DateTime(2026, 4, 10), OncekiEndeks = 20400, GuncelEndeks = 20780, TuketimMiktari = 380, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Ticarethane", CreatedAt = new DateTime(2026, 4, 10) },
        new EndeksOkuma { OkumaId = 20, TuketimNoktasiId = 1020, SayacId = 5020, SozlesmeId = 3020, SayacSeriNo = "SN-38-020123", TuketimNoktasiKodu = "TK-2026-020", OkumaTarihi = new DateTime(2026, 4, 12), OncekiEndeks = 15600, GuncelEndeks = 18200, TuketimMiktari = 2600, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Anormal, DogrulamaDurumu = false, AnomaliAciklamasi = "Anormal yüksek tüketim: 2600 kWh. OSOS veri doğrulaması başarısız", TarifeGrubu = "Sanayi", CreatedAt = new DateTime(2026, 4, 12) },
        new EndeksOkuma { OkumaId = 21, TuketimNoktasiId = 1021, SayacId = 5021, SozlesmeId = 3021, SayacSeriNo = "SN-38-021234", TuketimNoktasiKodu = "TK-2026-021", OkumaTarihi = new DateTime(2026, 5, 3), OncekiEndeks = 17800, GuncelEndeks = 18150, TuketimMiktari = 350, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 5, 3) },
        new EndeksOkuma { OkumaId = 22, TuketimNoktasiId = 1022, SayacId = 5022, SozlesmeId = 3022, SayacSeriNo = "SN-38-022345", TuketimNoktasiKodu = "TK-2026-022", OkumaTarihi = new DateTime(2026, 5, 5), OncekiEndeks = 22700, GuncelEndeks = 23100, TuketimMiktari = 400, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Ticarethane", CreatedAt = new DateTime(2026, 5, 5) },
        new EndeksOkuma { OkumaId = 23, TuketimNoktasiId = 1023, SayacId = 5023, SozlesmeId = 3023, SayacSeriNo = "SN-38-023456", TuketimNoktasiKodu = "TK-2026-023", OkumaTarihi = new DateTime(2026, 5, 7), OncekiEndeks = 19900, GuncelEndeks = 20200, TuketimMiktari = 300, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 5, 7) },
        new EndeksOkuma { OkumaId = 24, TuketimNoktasiId = 1024, SayacId = 5024, SozlesmeId = 3024, SayacSeriNo = "SN-38-024567", TuketimNoktasiKodu = "TK-2026-024", OkumaTarihi = new DateTime(2026, 5, 10), OncekiEndeks = 16900, GuncelEndeks = 16900, TuketimMiktari = 0, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.SifirTuketim, DogrulamaDurumu = false, AnomaliAciklamasi = "OSOS sıfır tüketim bildirdi, iletişim kesintisi olabilir", TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 5, 10) },
        new EndeksOkuma { OkumaId = 25, TuketimNoktasiId = 1025, SayacId = 5025, SozlesmeId = 3025, SayacSeriNo = "SN-38-025678", TuketimNoktasiKodu = "TK-2026-025", OkumaTarihi = new DateTime(2026, 5, 12), OncekiEndeks = 21300, GuncelEndeks = 21750, TuketimMiktari = 450, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Sanayi", CreatedAt = new DateTime(2026, 5, 12) },
        new EndeksOkuma { OkumaId = 26, TuketimNoktasiId = 1026, SayacId = 5026, SozlesmeId = 3026, SayacSeriNo = "SN-38-026789", TuketimNoktasiKodu = "TK-2026-026", OkumaTarihi = new DateTime(2026, 6, 2), OncekiEndeks = 18100, GuncelEndeks = 18350, TuketimMiktari = 250, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 6, 2) },
        new EndeksOkuma { OkumaId = 27, TuketimNoktasiId = 1027, SayacId = 5027, SozlesmeId = 3027, SayacSeriNo = "SN-38-027890", TuketimNoktasiKodu = "TK-2026-027", OkumaTarihi = new DateTime(2026, 6, 5), OncekiEndeks = 23400, GuncelEndeks = 23850, TuketimMiktari = 450, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Ticarethane", CreatedAt = new DateTime(2026, 6, 5) },
        new EndeksOkuma { OkumaId = 28, TuketimNoktasiId = 1028, SayacId = 5028, SozlesmeId = 3028, SayacSeriNo = "SN-38-028901", TuketimNoktasiKodu = "TK-2026-028", OkumaTarihi = new DateTime(2026, 6, 8), OncekiEndeks = 15400, GuncelEndeks = 15580, TuketimMiktari = 180, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 6, 8) },
        new EndeksOkuma { OkumaId = 29, TuketimNoktasiId = 1029, SayacId = 5029, SozlesmeId = 3029, SayacSeriNo = "SN-38-029012", TuketimNoktasiKodu = "TK-2026-029", OkumaTarihi = new DateTime(2026, 6, 10), OncekiEndeks = 20600, GuncelEndeks = 23100, TuketimMiktari = 2500, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Anormal, DogrulamaDurumu = false, AnomaliAciklamasi = "Anormal yüksek tüketim: 2500 kWh. Sayaç arızası olabilir", TarifeGrubu = "Sanayi", CreatedAt = new DateTime(2026, 6, 10) },
        new EndeksOkuma { OkumaId = 30, TuketimNoktasiId = 1030, SayacId = 5030, SozlesmeId = 3030, SayacSeriNo = "SN-38-030123", TuketimNoktasiKodu = "TK-2026-030", OkumaTarihi = new DateTime(2026, 6, 12), OncekiEndeks = 17200, GuncelEndeks = 17550, TuketimMiktari = 350, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 6, 12) },
        new EndeksOkuma { OkumaId = 31, TuketimNoktasiId = 1031, SayacId = 5031, SozlesmeId = 3031, SayacSeriNo = "SN-38-031234", TuketimNoktasiKodu = "TK-2026-031", OkumaTarihi = new DateTime(2026, 6, 15), OncekiEndeks = 22400, GuncelEndeks = 22750, TuketimMiktari = 350, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Ticarethane", CreatedAt = new DateTime(2026, 6, 15) },
        new EndeksOkuma { OkumaId = 32, TuketimNoktasiId = 1032, SayacId = 5032, SozlesmeId = 3032, SayacSeriNo = "SN-38-032345", TuketimNoktasiKodu = "TK-2026-032", OkumaTarihi = new DateTime(2026, 6, 18), OncekiEndeks = 19600, GuncelEndeks = 19850, TuketimMiktari = 250, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 6, 18) },
        new EndeksOkuma { OkumaId = 33, TuketimNoktasiId = 1033, SayacId = 5033, SozlesmeId = 3033, SayacSeriNo = "SN-38-033456", TuketimNoktasiKodu = "TK-2026-033", OkumaTarihi = new DateTime(2026, 6, 20), OncekiEndeks = 16500, GuncelEndeks = 16680, TuketimMiktari = 180, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 6, 20) },
        new EndeksOkuma { OkumaId = 34, TuketimNoktasiId = 1034, SayacId = 5034, SozlesmeId = 3034, SayacSeriNo = "SN-38-034567", TuketimNoktasiKodu = "TK-2026-034", OkumaTarihi = new DateTime(2026, 6, 25), OncekiEndeks = 24100, GuncelEndeks = 24600, TuketimMiktari = 500, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Sanayi", CreatedAt = new DateTime(2026, 6, 25) },
        new EndeksOkuma { OkumaId = 35, TuketimNoktasiId = 1035, SayacId = 5035, SozlesmeId = 3035, SayacSeriNo = "SN-38-035678", TuketimNoktasiKodu = "TK-2026-035", OkumaTarihi = new DateTime(2026, 6, 28), OncekiEndeks = 18700, GuncelEndeks = 18980, TuketimMiktari = 280, OkumaKaynagi = OkumaKaynagi.OSOS, OkumaDurumu = OkumaDurumu.Basarili, DogrulamaDurumu = true, TarifeGrubu = "Mesken", CreatedAt = new DateTime(2026, 6, 28) }
    };

    /// <inheritdoc />
    public List<EndeksOkuma> GetAll()
    {
        return _okumalar.OrderByDescending(x => x.OkumaTarihi).ToList();
    }

    /// <inheritdoc />
    public EndeksOkuma? GetById(int id)
    {
        return _okumalar.FirstOrDefault(x => x.OkumaId == id);
    }

    /// <inheritdoc />
    public List<EndeksOkuma> Filtrele(OkumaKaynagi? kaynak, OkumaDurumu? durum, DateTime? baslangic, DateTime? bitis, string? arama)
    {
        var query = _okumalar.AsEnumerable();

        if (kaynak.HasValue)
            query = query.Where(x => x.OkumaKaynagi == kaynak.Value);

        if (durum.HasValue)
            query = query.Where(x => x.OkumaDurumu == durum.Value);

        if (baslangic.HasValue)
            query = query.Where(x => x.OkumaTarihi >= baslangic.Value);

        if (bitis.HasValue)
            query = query.Where(x => x.OkumaTarihi <= bitis.Value);

        if (!string.IsNullOrWhiteSpace(arama))
        {
            var aramaLower = arama.ToLower();
            query = query.Where(x =>
                (x.TuketimNoktasiKodu != null && x.TuketimNoktasiKodu.ToLower().Contains(aramaLower)) ||
                (x.SayacSeriNo != null && x.SayacSeriNo.ToLower().Contains(aramaLower)) ||
                (x.OkuyanKullaniciAdi != null && x.OkuyanKullaniciAdi.ToLower().Contains(aramaLower)) ||
                (x.AnomaliAciklamasi != null && x.AnomaliAciklamasi.ToLower().Contains(aramaLower)) ||
                (x.TarifeGrubu != null && x.TarifeGrubu.ToLower().Contains(aramaLower))
            );
        }

        return query.OrderByDescending(x => x.OkumaTarihi).ToList();
    }

    /// <inheritdoc />
    public (int Toplam, int Manuel, int OSOS, int Anomali, decimal OrtalamaTuketim) GetIstatistikler()
    {
        int toplam = _okumalar.Count;
        int manuel = _okumalar.Count(x => x.OkumaKaynagi == OkumaKaynagi.Manuel);
        int osos = _okumalar.Count(x => x.OkumaKaynagi == OkumaKaynagi.OSOS);
        int anomali = _okumalar.Count(x => x.OkumaDurumu == OkumaDurumu.Anormal);
        decimal ortalamaTuketim = toplam > 0
            ? Math.Round(_okumalar.Average(x => x.TuketimMiktari), 2)
            : 0;

        return (toplam, manuel, osos, anomali, ortalamaTuketim);
    }
}
