const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

const darkMode = window.matchMedia('(prefers-color-scheme: dark)').matches;

const tinyBaseConfig = {
	// ID'si Content olan HTML elementini TinyMCE editörüne dönüştürür
	selector: '#Content',

	skin: darkMode ? 'oxide-dark' : 'oxide',

	content_css: darkMode ? 'dark' : 'default',

	height: '600px',

	// Editör içerisindeki içeriğe TinyMCE'nin hazır "document" stilini uygular. Bu stil Word benzeri bir görünüm sağlar. Alternatifleri: 'default' / 'dark' / false
	content_css: 'document',

	// Bu CSS sadece editörün içindeki iframe'e uygulanır
	content_style: `
		body {
			max-width: none !important;
			width: 95%;
			min-height: 297mm;
			margin: 30px auto;
			padding: 10mm 10mm;
			box-sizing: border-box;
		}
	`,

	// TinyMCE'nin "Upgrade" veya promosyon mesajlarını gizler
	promotion: false,

	// Alt kısımdaki Powered by Tiny yazısını kaldırır
	branding: false,

	// Alttaki bilgi çubuğunu gizler. Normalde burada: kelime sayısı, element yolu, resize tutamacı görünür
	statusbar: false,

	// Sağ tıklayınca yalnızca listedeli işlemler çıkar
	contextmenu: 'undo redo | cut copy paste | link image table',

	// true ise tarayıcının sağ tık menüsü yerine her zaman TinyMCE'nin menüsü açılır
	contextmenu_never_use_native: true,

	// Clipboard'dan resim yapıştırılmasına izin verir: <img src="data:image/png;base64,..." />
	paste_data_images: true,

	// TinyMCE'nin yapıştırılan veya sürüklenen resimler için images_upload_handler'ı ya da images_upload_url'ı otomatik çağırıp çağırmayacağını belirler. images_upload_handler ve images_upload_url aynı anda varsa TinyMCE’de öncelik images_upload_handler’dadır.
	automatic_uploads: true,

	// built-in (otomatik upload sistemi)
	// Ctrl + V (paste image), Drag & drop, Editor içine direkt resim bırakma
	images_upload_url: '/Upload',

	// custom upload sistemi. Kullanıcı upload işlemini kendisi yönetiyor, ben karışmıyorum
	images_upload_handler: function (blobInfo, success, failure) {
		uploadImage(blobInfo)
			.then(success)
			.catch(err => failure(err.message))
		;
	},

	// false ise linkleri göreceli değil tam yol olarak üretir: true: <img src="/uploads/a.jpg"> false: <img src="https://site.com/uploads/a.jpg">
	relative_urls: false,

	// false Host kısmını kaldırmaz: https://site.com/resim.jpg, true: /resim.jpg
	remove_script_host: false,

	// TinyMCE URL'leri normalize eder: https://site.com//resim.jpg gibi bozuk URL'leri düzeltir.
	convert_urls: true,

	// true: Desteklenmeyen dosyaların editöre sürüklenmesini engeller
	block_unsupported_drop: true,

	// true: Chrome/Edge'in kendi yazım denetimini açar
	browser_spellcheck: true,
};

const tinyMenuConfig = {
	// requires related plugins
	// menubar: false,
	menubar: 'file edit view insert format tools table help',
};

const tinyPluginConfig = {
	plugins: 'advlist anchor autolink autosave charmap code codesample colorpicker contextmenu directionality emoticons fullscreen help hr image imagetools importcss insertdatetime legacyoutput link lists media nonbreaking noneditable pagebreak paste preview print save searchreplace spellchecker tabfocus table template textcolor textpattern toc visualblocks visualchars wordcount',

	save_enablewhendirty: true,
};


const tinyToolbarConfig = {
	toolbar1: `fontselect fontsizeselect | formatselect bold italic underline strikethrough forecolor backcolor removeformat | alignleft aligncenter alignright alignjustify alignnone | bullist numlist outdent indent lineheight`,

	toolbar2: `save cancel | codesample customImageButton | undo redo | fullscreen  | selectall remove | table | charmap emoticons hr | searchreplace visualblocks code  preview print | insertdatetime pagebreak nonbreaking toc | help `,

	toolbar_mode: 'wrap',

	// ltr rtl link image media anchor
};

const tinyFunctionSetup = {
	setup: function (editor) {
		EditorInit(editor);
		tinyMCECustomImageUpload(editor);
	},
	init_instance_callback: function (editor) {
		ExecCommand(editor);
	},
};


async function uploadImage(blobInfo) {
	const imageUploadMode = localStorage.getItem('imageUploadMode') || 'server';
	
	if (imageUploadMode === 'server') return await uploadToServer(blobInfo);
	else if (imageUploadMode === 'base64') return await toBase64(blobInfo);
	else alert("Unknown upload mode");
}


function toBase64(blobInfo) {

	// kopyalanan ya da sürüklenen resmin tarayıcı cahe inde blob olarak saklanması yerine editör içeriğinde Base64 olarak saklanması için	
	// <img src="blob:https://localhost:5001/c1948805-ccea-4fa3-9d89-08aea7866f80... yerine
	// <img src="data:image/png;base64,iVBOR... olarak

	return new Promise((resolve, reject) => {

		const reader = new FileReader();

		reader.onload = () => resolve(reader.result);

		reader.onerror = reject;

		reader.readAsDataURL(blobInfo.blob());
	});
}


async function uploadToServer(blobInfo) {

	// const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

	const formData = new FormData();

	formData.append('file', blobInfo.blob(), blobInfo.filename());

	const response = await fetch('/api/services/uploadimage', {
		method: 'POST',
		body: formData,
		headers: {
			'RequestVerificationToken': token
		}
	});

	if (!response.ok) {
		throw new Error('Upload failed');
	}

	const data = await response.json();

	return data.location;
}


async function uploadToServer2(blobInfo) {

	// const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

	const formData = new FormData();

	formData.append('file', blobInfo.blob(), blobInfo.filename());

	return fetch('/api/services/uploadimage', {
		method: 'POST',
		body: formData,
		headers: {
			'RequestVerificationToken': token
		}
	})
		.then(r => {
			if (!r.ok)
				throw new Error('Upload failed');

			return r.json();
		})
		.then(data => data.location);
}


function EditorInit(editor) {
	editor.on('init', function (e) {
		console.log('tinymce has been initialized');
	});
}


function ExecCommand(editor) {
	editor.on('ExecCommand', function (e) {
		console.log(`${e.command} executed`);
		//editor.setContent('<p>Hazır!</p>');
	});
}


function tinyMCECustomImageUpload(editor) {

	editor.ui.registry.addButton('customImageButton', {
		text: '',
		icon: 'gallery',
		onAction: function () {

			const input = document.createElement('input');
			input.type = 'file';
			input.accept = 'image/*';

			input.click();

			input.onchange = async function () {
				const file = this.files[0];

				const blobInfo = {
					blob: () => file,
					filename: () => file.name
				};

				const url = await uploadImage(blobInfo);

				// editor.insertContent kullanıldığında TinyMCE'nin images_upload_handler ÇALIŞMAZ
				editor.insertContent(`<img src="${url}" />`);
			};
		}
	});
}


const tinyFilePicker = {

	// Toolbar'dan "Insert Image" butonu tıklandığında ya da Link/Media/Image dialog kullanıldığında

	// true: Upload edilirken orijinal dosya adı korunur
	// false: TinyMCE genelde benzersiz (unique) isim üretir
	images_reuse_filename: false,

	// TinyMCE şunları filtreler: file picker, drag & drop, paste image, upload pipeline
	// Default Value: 'jpeg,jpg,jpe,jfi,jif,jfif,png,gif,bmp,webp'
	// images_file_types: 'jpeg,jpg,png,gif,webp', 

	// TinyMCE’de image(resim) dialog’una “Advanced / Gelişmiş” sekmesini ekleyip eklemeyeceğini belirler.
	image_advtab: true,

	file_picker_types: 'image', //'image media file'


	file_picker_callback: function (callback, value, meta) {

		const input = document.createElement('input');

		input.type = 'file';

		if (meta.filetype === 'image') input.accept = 'image/*';

		if (meta.filetype === 'media') input.accept = 'video/*';		

		input.click();

		input.onchange = function () {

			const file = input.files[0];

			const formData = new FormData();
			
			formData.append('file', file);

			fetch('/api/services/uploadimage', {
				method: 'POST',
				headers: {
					'RequestVerificationToken': token
				},
				body: formData
			})
				.then(r => r.json())
				.then(data => {
					callback(data.location);
				});
		};
	}
};


const tinyCodeSampleLanguagesCommon = {

	codesample_languages: [
		{ text: 'HTML/XML', value: 'markup' },
		{ text: "XML", value: "xml" },
		{ text: "HTML", value: "html" },
		{ text: "SVG", value: "svg" },
		{ text: "CSS", value: "css" },
		{ text: "Javascript", value: "javascript" },
		{ text: "ActionScript", value: "actionscript" },
		{ text: "asciidoc", value: "asciidoc" },
		{ text: "aspnet", value: "aspnet" },
		{ text: "bash", value: "bash" },
		{ text: "basic", value: "basic" },
		{ text: "batch", value: "batch" },
		{ text: "c", value: "c" },
		{ text: "C#", value: "csharp" },
		{ text: "C++", value: "cpp" },
		{ text: "ruby", value: "ruby" },
		{ text: "dart", value: "dart" },
		{ text: "docker", value: "docker" },
		{ text: "git", value: "git" },
		{ text: "http", value: "http" },
		{ text: "java", value: "java" },
		{ text: "JSON", value: "json" },
		{ text: "jsonp", value: "jsonp" },
		{ text: "markdown", value: "markdown" },
		{ text: "nginx", value: "nginx" },
		{ text: "objectivec", value: "objectivec" },
		{ text: "PHP", value: "php" },
		{ text: "python", value: "python" },
		{ text: "jsx", value: "jsx" },
		{ text: "rest", value: "rest" },
		{ text: "scss", value: "scss" },
		{ text: "SQL", value: "sql" },
		{ text: "swift", value: "swift" },
		{ text: "textile", value: "textile" },
		{ text: "TypeScript", value: "typescript" },
		{ text: "YAML", value: "yaml" }
	],
	// This configuration option allows a global Prism.js version to be used when highlighting code sample blocks, instead of using the Prism.js version bundled inside the codesample plugin. This allows for a custom version of Prism.js, including additional languages, to be used.
	codesample_global_prismjs: true,

	//codesample_dialog_height: 900,
	//codesample_dialog_width: 600,
};


const tinyCodeSampleLanguagesFull = {

	codesample_languages: [
		{ text: 'HTML/XML', value: 'markup' },
		{ text: "XML", value: "xml" },
		{ text: "HTML", value: "html" },
		{ text: "mathml", value: "mathml" },
		{ text: "SVG", value: "svg" },
		{ text: "CSS", value: "css" },
		{ text: "Clike", value: "clike" },
		{ text: "Javascript", value: "javascript" },
		{ text: "ActionScript", value: "actionscript" },
		{ text: "apacheconf", value: "apacheconf" },
		{ text: "apl", value: "apl" },
		{ text: "applescript", value: "applescript" },
		{ text: "asciidoc", value: "asciidoc" },
		{ text: "aspnet", value: "aspnet" },
		{ text: "autoit", value: "autoit" },
		{ text: "autohotkey", value: "autohotkey" },
		{ text: "bash", value: "bash" },
		{ text: "basic", value: "basic" },
		{ text: "batch", value: "batch" },
		{ text: "c", value: "c" },
		{ text: "brainfuck", value: "brainfuck" },
		{ text: "bro", value: "bro" },
		{ text: "bison", value: "bison" },
		{ text: "C#", value: "csharp" },
		{ text: "C++", value: "cpp" },
		{ text: "CoffeeScript", value: "coffeescript" },
		{ text: "ruby", value: "ruby" },
		{ text: "d", value: "d" },
		{ text: "dart", value: "dart" },
		{ text: "diff", value: "diff" },
		{ text: "docker", value: "docker" },
		{ text: "eiffel", value: "eiffel" },
		{ text: "elixir", value: "elixir" },
		{ text: "erlang", value: "erlang" },
		{ text: "fsharp", value: "fsharp" },
		{ text: "fortran", value: "fortran" },
		{ text: "git", value: "git" },
		{ text: "glsl", value: "glsl" },
		{ text: "go", value: "go" },
		{ text: "groovy", value: "groovy" },
		{ text: "haml", value: "haml" },
		{ text: "handlebars", value: "handlebars" },
		{ text: "haskell", value: "haskell" },
		{ text: "haxe", value: "haxe" },
		{ text: "http", value: "http" },
		{ text: "icon", value: "icon" },
		{ text: "inform7", value: "inform7" },
		{ text: "ini", value: "ini" },
		{ text: "j", value: "j" },
		{ text: "jade", value: "jade" },
		{ text: "java", value: "java" },
		{ text: "JSON", value: "json" },
		{ text: "jsonp", value: "jsonp" },
		{ text: "julia", value: "julia" },
		{ text: "keyman", value: "keyman" },
		{ text: "kotlin", value: "kotlin" },
		{ text: "latex", value: "latex" },
		{ text: "less", value: "less" },
		{ text: "lolcode", value: "lolcode" },
		{ text: "lua", value: "lua" },
		{ text: "makefile", value: "makefile" },
		{ text: "markdown", value: "markdown" },
		{ text: "matlab", value: "matlab" },
		{ text: "mel", value: "mel" },
		{ text: "mizar", value: "mizar" },
		{ text: "monkey", value: "monkey" },
		{ text: "nasm", value: "nasm" },
		{ text: "nginx", value: "nginx" },
		{ text: "nim", value: "nim" },
		{ text: "nix", value: "nix" },
		{ text: "nsis", value: "nsis" },
		{ text: "objectivec", value: "objectivec" },
		{ text: "ocaml", value: "ocaml" },
		{ text: "oz", value: "oz" },
		{ text: "parigp", value: "parigp" },
		{ text: "parser", value: "parser" },
		{ text: "pascal", value: "pascal" },
		{ text: "perl", value: "perl" },
		{ text: "PHP", value: "php" },
		{ text: "processing", value: "processing" },
		{ text: "prolog", value: "prolog" },
		{ text: "protobuf", value: "protobuf" },
		{ text: "puppet", value: "puppet" },
		{ text: "pure", value: "pure" },
		{ text: "python", value: "python" },
		{ text: "q", value: "q" },
		{ text: "qore", value: "qore" },
		{ text: "r", value: "r" },
		{ text: "jsx", value: "jsx" },
		{ text: "rest", value: "rest" },
		{ text: "rip", value: "rip" },
		{ text: "roboconf", value: "roboconf" },
		{ text: "crystal", value: "crystal" },
		{ text: "rust", value: "rust" },
		{ text: "sas", value: "sas" },
		{ text: "sass", value: "sass" },
		{ text: "scss", value: "scss" },
		{ text: "scala", value: "scala" },
		{ text: "scheme", value: "scheme" },
		{ text: "smalltalk", value: "smalltalk" },
		{ text: "smarty", value: "smarty" },
		{ text: "SQL", value: "sql" },
		{ text: "stylus", value: "stylus" },
		{ text: "swift", value: "swift" },
		{ text: "tcl", value: "tcl" },
		{ text: "textile", value: "textile" },
		{ text: "twig", value: "twig" },
		{ text: "TypeScript", value: "typescript" },
		{ text: "verilog", value: "verilog" },
		{ text: "vhdl", value: "vhdl" },
		{ text: "wiki", value: "wiki" },
		{ text: "YAML", value: "yaml" }
	],
	// This configuration option allows a global Prism.js version to be used when highlighting code sample blocks, instead of using the Prism.js version bundled inside the codesample plugin. This allows for a custom version of Prism.js, including additional languages, to be used.
	codesample_global_prismjs: true,

	//codesample_dialog_height: 900,
	//codesample_dialog_width: 600,
};

/*
Mevcut tüm öğeleri görmek için Editörü başlattıktan sonra konsolda:
tinymce.activeEditor.ui.registry.getAll()

Kayıtlı tüm öğeleri görmek için Editörü başlattıktan sonra konsolda:
console.log(tinymce.activeEditor.ui.registry.getAll().menuItems);

TinyMCE 5'te yüklü pluginleri görmek için:
console.log(Object.keys(tinymce.activeEditor.plugins));

Kendi menü öğeni eklemek:
setup: function(editor) {
	editor.ui.registry.addMenuItem('merhaba', {
		text: 'Merhaba',
		onAction: function() {
			alert('Merhaba');
		}
	});
}
contextmenu: 'merhaba link image table'
*/