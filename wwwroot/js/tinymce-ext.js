const tinyBaseConfig = {
	selector: '#Content',
	height: '600px',
	content_css: 'document',
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
	promotion: false,
	branding: false,
	statusbar: false,
	
	contextmenu: 'link image table',
	contextmenu_never_use_native: true,
	paste_data_images: true,
	relative_urls: false,
	remove_script_host: false,
	convert_urls: true,
	block_unsupported_drop: true,
	browser_spellcheck: true,
};

const tinyMenuConfig = {
	// menubar: false,
	menubar: 'file edit view insert format tools table help',
};

const tinyPluginConfig = {
	plugins: 'advlist anchor autolink autosave charmap code codesample colorpicker contextmenu directionality emoticons fullscreen help hr image imagetools importcss insertdatetime legacyoutput link lists media nonbreaking noneditable pagebreak paste preview print save searchreplace spellchecker tabfocus table template textcolor textpattern toc visualblocks visualchars wordcount',

	save_enablewhendirty: true,
};

const tinyToolbarConfig = {
	toolbar1: `formatselect | fontselect fontsizeselect  | bold italic underline strikethrough forecolor backcolor removeformat | alignleft aligncenter alignright alignjustify alignnone | bullist numlist outdent indent lineheight`,

	toolbar2: `save cancel | CustomImageUpload gallery blocks | undo redo | codesample | fullscreen ltr rtl | selectall remove | link image media anchor | table | charmap emoticons hr | searchreplace visualblocks code  preview print | insertdatetime pagebreak nonbreaking toc | help`,

	toolbar_mode: 'wrap',
};

const tinyAutoImageUpload =
{
	// Kullanıcı: Resim sürükler - Ctrl+ V ile yapıştırır - Image dialog kullanır
	automatic_uploads: true,
	file_picker_types: 'image',
	// images_upload_url: '/Tiny5/UploadImage',
	images_upload_handler: function (blobInfo, success, failure) {
		const formData = new FormData();
		formData.append('file', blobInfo.blob());
		formData.append('__RequestVerificationToken', token);

		fetch('/Tiny5/UploadImage', {
			method: 'POST',
			body: formData
		})
			.then(res => res.json())
			.then(data => success(data.location))
			.catch(() => failure('Upload failed'));
	}
};


const tinyFilePicker = {

	// images_reuse_filename: true,

	// images_file_types: 'jpeg,jpg,png,gif,webp', // Default Value: 'jpeg,jpg,jpe,jfi,jif,jfif,png,gif,bmp,webp'

	// image_advtab: true,

	file_picker_types: 'image media file',


	file_picker_callback: function (callback, value, meta) {

		const input = document.createElement('input');
		input.type = 'file';

		// hangi tür dosya?
		if (meta.filetype === 'image') {
			input.accept = 'image/*';
		}

		if (meta.filetype === 'media') {
			input.accept = 'video/*';
		}

		input.click();

		input.onchange = function () {
			const file = input.files[0];

			const formData = new FormData();
			formData.append('file', file);

			fetch('/Tiny5/UploadImage', {
				method: 'POST',
				headers: {
					'RequestVerificationToken': token
				},
				body: formData
			})
				.then(r => r.json())
				.then(data => {
					// TinyMCE'ye URL döndür
					callback(data.location);
				});
		};
	}

};

const tinyFunctionSetup = {
	setup: function (editor) {
		EditorInit(editor);
		CustomUploadImageButton(editor);
		// CustomUploadImageGalleryButton(editor);
	},
	init_instance_callback: function (editor) {
		ExecCommand(editor);
	},
	/*
	images_upload_handler: function (blobInfo, success, failure) {
		UploadImage(blobInfo, success, failure);
	},*/
};


const tinyCodeSampleLanguages = {

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
function UploadImage(blobInfo, success, failure) {
	var formData = new FormData();
	formData.append('img', blobInfo.blob(), blobInfo.blob().name);
	formData.append('__RequestVerificationToken', GetRequestVerificationToken());

	var imageUploadUrl = '@Url.Content("~/tinymce/image")';

	$.ajax({
		url: imageUploadUrl,
		type: 'POST',
		data: formData,
		processData: false,
		contentType: false,
		success: function (data, textStatus, jqXHR) {
			success(data.location);
			console.log("image saved");
		},
		error: function (jqXHR, textStatus, errorThrown) {
			if (jqXHR.responseText) {
				errors = JSON.parse(jqXHR.responseText).errors;
				console.log(errors.join(", "));
				console.log(textStatus);
				console.log(errorThrown);
				editor.insertContent(errorThrown);
			}
		}
	});
}*/

function ExecCommand(editor) {
	editor.on('ExecCommand', function (e) {
		console.log(`${e.command} executed`);
		//editor.setContent('<p>Hazır!</p>');
	});
}

function CancelCallback() {
	Swal.fire({
		title: 'Warning',
		text: "If you continue, all unsaved data will be lost.",
		icon: 'warning',
		showCancelButton: true,
		confirmButtonText: 'OK',
		cancelButtonText: 'Cancel',
	}).then((result) => {
		if (result.isConfirmed) {
			tinymce.activeEditor.resetContent();
		}
	});
}


function GetRequestVerificationToken() {
	const form = document.querySelector('form');
	return form?.querySelector('input[name="__RequestVerificationToken"]')?.value;
}

function EditorInit(editor) {
	editor.on('init', function (e) {
		console.log('tinymce has been initialized');
	});
}
/*
function CustomUploadImageButton(editor) {

	var fileInput = $('<input id="tinymce-uploader" type="file" name="pic" accept="image/*" style="display:none">');

	$(editor.getElement()).parent().append(fileInput);

	editor.ui.registry.addButton('CustomImageUpload', {
		icon: 'image',
		tooltip: 'Image Upload',
		onAction: function (_) {
			fileInput.trigger('click');
		}
	});
	fileInput.on("change", function (e) {
		CustomUploadImage($(this), editor);
	});
}*/

function CustomUploadImageButton(editor) {

	const fileInput = document.createElement('input');
	fileInput.id = 'tinymce-uploader';
	fileInput.type = 'file';
	fileInput.name = 'pic';
	fileInput.accept = 'image/*';
	fileInput.style.display = 'none';

	// editor container'a ekle
	editor.getElement().parentNode.appendChild(fileInput);

	// button ekle
	editor.ui.registry.addButton('CustomImageUpload', {
		icon: 'image',
		tooltip: 'Image Upload',
		onAction: function () {
			fileInput.click();
		}
	});

	// change event
	fileInput.addEventListener('change', function () {
		CustomUploadImage(fileInput, editor);
	});
}


function CustomUploadImage(inp, editor) {
	const imageUploadUrl = '/Tiny5/UploadImage';

	const input = inp.files[0];

	const formData = new FormData();
	formData.append('file', input, input.name);
	formData.append('__RequestVerificationToken', token);

	fetch(imageUploadUrl, {
		method: 'POST',
		body: formData
	})
		.then(async response => {
			if (!response.ok) {
				throw new Error(await response.text());
			}
			return response.json();
		})
		.then(data => {
			editor.insertContent(`<img src='${data.location}' alt='' />`);

			// input temizleme
			inp.value = null;
		})
		.catch(error => {
			editor.insertContent(error.message);
		});
}

/*
function CustomUploadImageGalleryButton(editor) {
	editor.ui.registry.addButton('gallery', {
		text: 'Gallery',
		icon: 'gallery',
		tooltip: 'Add image from gallery',
		onAction: function (_) {
			$('#image').val('');
			tinyMCE.activeEditor.windowManager.openUrl({
				title: 'Image Gallery',
				url: '@Url.Content("~/ImageGallery/Index")',
				width: 800,
				height: 600,
				buttons: [
					{
						type: 'cancel',
						name: 'closeButton',
						text: 'Cancel'
					}
				],
				onmessage: function (dialogApi, details) {
					console.log(dialogApi);
					console.log(details);
				}
			});
			//tinyMCE.activeEditor.windowManager.open({
			//    title: 'My Gallery', 
			//    size: 'large',
			//    body: {
			//        type: 'panel', // The root body type - a Panel or TabPanel
			//        items: [ // A list of panel components
			//            {
			//                type: 'htmlpanel', // A HTML panel component
			//                html: '<div class="gallery-div"><input type="hidden" id="image"/>Your Gallery Items Html</div>'
			//            }
			//        ]
			//    },
			//    buttons: [
			//        {
			//            type: 'cancel',
			//            name: 'closeButton',
			//            text: 'Cancel'
			//        },
			//        {
			//            type: 'submit',
			//            name: 'submitButton',
			//            text: 'Insert To Editor',
			//            primary: true
			//        }
			//    ],
			//    onSubmit: function (api) {
			//        var data = api.getData();
			//        let source = $('#image').val();

			//        if (source != '') {
			//            editor.focus();
			//            editor.selection.setContent('<img src="' + source + '" />');
			//            api.close();
			//        } else {
			//            alert("Please select an image.");
			//        }
			//    }
			//});
		}
	});
}*/