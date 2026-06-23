// IFE (Immediately Invoked Function Expression)
// JavaScript’te normalde fonksiyonlar tek başına çalıştırılamaz, ama parantez içine alınca “değer gibi” olur.
// Güzel, çünkü IIFE artık çoğu modern projede “geçici çözüm” gibi kalıyor. Yerini büyük ölçüde ES Modules (ESM) aldı.

// Arrow function IIFE
(() => {
	// console.log(this); // this lexical (dış scope’tan gelir), kendi this’i yoktur
	// console.log(arguments); // yok, hata verir
})();

// Klasik IIFE 
(function () {
    // console.log(this); // this dinamik çalışır, global veya çağrıldığı bağlama göre değişebilir
	// console.log(arguments); // var
})();