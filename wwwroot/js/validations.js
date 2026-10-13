document.addEventListener("DOMContentLoaded", function() {
    // Formlardaki tüm inputları yakala
    const inputs = document.querySelectorAll("input");

    inputs.forEach(input => {
        const name = (input.getAttribute("name") || "").toLowerCase();
        const id = (input.getAttribute("id") || "").toLowerCase();
        const type = (input.getAttribute("type") || "").toLowerCase();

        // 1. TC Kimlik No - Sadece Rakam ve Max 11 hane
        if (name === "tc" || name === "tcno" || name === "tc_no" || name === "tckn" || id === "tc" || id === "tcno" || id === "tckn") {
            input.setAttribute("maxlength", "11");
            input.addEventListener("input", function() {
                // Sadece rakamları tutar
                this.value = this.value.replace(/[^0-9]/g, '');
                // type="number" olan inputlarda maxlength HTML tarafında çalışmaz, o yüzden JS ile kesiyoruz:
                if (this.value.length > 11) {
                    this.value = this.value.slice(0, 11);
                }
            });
        }

        // 2. Vergi Kimlik No (VKN) - Sadece Rakam ve Max 10 hane
        if (name === "vkn" || id === "vkn") {
            input.setAttribute("maxlength", "10");
            input.addEventListener("input", function() {
                this.value = this.value.replace(/[^0-9]/g, '');
                if (this.value.length > 10) {
                    this.value = this.value.slice(0, 10);
                }
            });
        }

        // 3. Telefon Numarası - Sadece Rakam ve Max 11 hane (Örn: 05554443322)
        if (name === "telefon" || name === "tel" || id === "telefon" || id === "tel" || name.includes("telefon")) {
            input.setAttribute("maxlength", "11");
            input.addEventListener("input", function() {
                this.value = this.value.replace(/[^0-9]/g, '');
                if (this.value.length > 11) {
                    this.value = this.value.slice(0, 11);
                }
            });
        }

        // 4. İsim ve Soyisim - Sadece Harf ve Boşluk
        const isNameField = name === "ad" || name === "soyad" || name === "isim" || name === "unvan" || 
                            name === "musteri_ad" || name === "musteri_soyad" || name === "musteri_unvan" ||
                            name.includes("ad_soyad") || name === "adsoyad" || name === "adsoyadunvan" || id === "adsoyadunvan";

        if (isNameField) {
            input.addEventListener("input", function() {
                // Türkçe harfler (ğüşıöç), büyük-küçük harfler ve boşluklara izin ver, gerisini sil
                this.value = this.value.replace(/[^a-zA-ZğüşıöçĞÜŞİÖÇ\s]/g, '');
            });
        }

        // 5. Endeks Değerleri - Sadece Rakam ve Nokta/Virgül
        if (name.includes("endeks") || id.includes("endeks")) {
            input.addEventListener("input", function() {
                // Sadece rakam, nokta ve virgüle izin ver (Örn: 1453.50 veya 1453,50)
                this.value = this.value.replace(/[^0-9,.]/g, '');
            });
        }
    });
});
