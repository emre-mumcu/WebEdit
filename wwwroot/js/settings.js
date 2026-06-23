// Prism Theme Settings

function ChangePrismTheme(PrismThemeName) {

	localStorage.setItem('prism-theme', PrismThemeName);
	
	var link = `${document.baseURI}lib/prismjs/themes/${PrismThemeName.toLowerCase()}.css`;

	document.getElementById("prism-theme-css").href = link;


	document.querySelectorAll('.prism-dropdown .dropdown-item').forEach(el => {
		el.classList.remove('active');
	});

	var activeItem = document.getElementById('prism-theme-' + PrismThemeName);
	if (activeItem) activeItem.classList.add('active');
	
	// location.reload();
}

document.addEventListener("DOMContentLoaded", function (event) {
	var prism_theme = (localStorage.getItem('prism-theme') == null) ? ("prism") : (localStorage.getItem('prism-theme'));

	ChangePrismTheme(prism_theme);
});

//Bootswatch Theme Settings

function ChangeBootswatchTheme(BsThemeName) {

	localStorage.setItem('bw-theme', BsThemeName);

	var link = `${document.baseURI}lib/bootswatch/${BsThemeName.toLowerCase()}/bootstrap.min.css`;

	document.getElementById("bw-theme-css").href = link;


	document.querySelectorAll('.bw-dropdown .dropdown-item').forEach(el => {
		el.classList.remove('active');
	});

	var activeItem = document.getElementById('bw-theme-' + BsThemeName);
	if (activeItem) activeItem.classList.add('active');

	// location.reload();
}

document.addEventListener("DOMContentLoaded", function (event) {
	var bw_theme = (localStorage.getItem('bw-theme') == null) ? ("default") : (localStorage.getItem('bw-theme'));

	ChangeBootswatchTheme(bw_theme);
});

