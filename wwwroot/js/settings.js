/*
 localStorage 
 ------------
 localStorage.setItem("key", "value");
 localStorage.getItem("key");
 localStorage.removeItem("key");
 Object.keys(localStorage);

**/

class BootswatchManager {

	static defaultValue = "default";
	static localStorageKey = "bw-theme-css";	

	GetTheme() {
		return (localStorage.getItem(BootswatchManager.localStorageKey) == null) ? (BootswatchManager.defaultValue) : (localStorage.getItem(BootswatchManager.localStorageKey));
	}

	SetTheme(themeName) {
		localStorage.setItem(BootswatchManager.localStorageKey, themeName);
	}

	UpdateUI(themeName, setTheme = true) {

		if(setTheme) this.SetTheme(themeName);		

		document.getElementById(BootswatchManager.localStorageKey).href = `${document.baseURI}lib/bootswatch/${themeName.toLowerCase()}/bootstrap.min.css`;

		document.querySelectorAll('.bw-dropdown .dropdown-item').forEach(el => {
			el.classList.remove('active');
		});

		var activeItem = document.getElementById('bw-theme-' + themeName);

		if (activeItem) activeItem.classList.add('active');

		// location.reload();
	}
}

class PrismJSManager {

	static defaultValue = "prism";
	static localStorageKey = "prism-theme-css";	

	GetTheme() {
		return (localStorage.getItem(PrismJSManager.localStorageKey) == null) ? (PrismJSManager.defaultValue) : (localStorage.getItem(PrismJSManager.localStorageKey));
	}

	SetTheme(themeName) {
		localStorage.setItem(PrismJSManager.localStorageKey, themeName);
	}

	UpdateUI(themeName, setTheme = true) {

		if(setTheme) this.SetTheme(themeName);		

		document.getElementById(PrismJSManager.localStorageKey).href = `${document.baseURI}lib/prismjs/themes/${themeName.toLowerCase()}.css`;

		document.querySelectorAll('.prism-dropdown .dropdown-item').forEach(el => {
			el.classList.remove('active');
		});

		var activeItem = document.getElementById('prism-theme-' + themeName);
	
		if (activeItem) activeItem.classList.add('active');		

		// location.reload();
	}

}

window.bootswatchManager = new BootswatchManager();
window.prismJSManagerManager = new PrismJSManager();

document.addEventListener("DOMContentLoaded", function (event) {
	window.bootswatchManager.UpdateUI(window.bootswatchManager.GetTheme(), false);
	window.prismJSManagerManager.UpdateUI(window.prismJSManagerManager.GetTheme(), false);
});