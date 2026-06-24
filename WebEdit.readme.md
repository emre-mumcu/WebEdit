dotnet list package --include-transitive
dotnet list package --vulnerable

```cs
/*

// builder.Services.AddSingleton<IEncryptionService, AesGcmEncryptionService>();

builder.Services.AddSingleton<IEncryptionService>(sp => new AesGcmEncryptionService());
builder.Services.AddSingleton<IEncryptionService>(sp =>
{
	return new AesGcmEncryptionService("BenimGizliParolam");
});

builder.Services.AddSingleton<IEncryptionService>(sp =>
{
	var config = sp.GetRequiredService<IConfiguration>();

	var password = config["Encryption:Password"]
		?? throw new InvalidOperationException();

	return new AesGcmEncryptionService(password);
});


Options Pattern

public class EncryptionOptions
{
    public string Password { get; set; } = string.Empty;
}

builder.Services.Configure<EncryptionOptions>(builder.Configuration.GetSection("Encryption"));

public AesGcmEncryptionService(IOptions<EncryptionOptions> options)
{
    var password = options.Value.Password;
    ...
}

builder.Services.AddSingleton<IEncryptionService, AesGcmEncryptionService>();

*/

```

# MVCController'daki UploadImage

```cs
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UploadImage([FromServices] IWebHostEnvironment env, IFormFile file)
{
	if (file == null || file.Length == 0) return BadRequest();

	var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");

	if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

	var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

	var filePath = Path.Combine(uploadsFolder, fileName);

	using (var stream = new FileStream(filePath, FileMode.Create))
	{
		await file.CopyToAsync(stream);
	}

	return Json(new { location = "/uploads/" + fileName });
}

```

```
Get-Volume -DriveLetter A
fsutil fsinfo drivetype A:
Get-Item "A:\AppStore\Dev.Apps\WebEdit\bin\Debug\net10.0\WebEdit.exe" -Stream *
Get-WinEvent -LogName "Microsoft-Windows-CodeIntegrity/Operational" -MaxEvents 20 |
Where-Object {$_.Id -in 3118,3077,3033} |
Select-Object TimeCreated, Id, Message |
Format-List
Get-WinEvent -LogName "Microsoft-Windows-CodeIntegrity/Operational" -MaxEvents 5
# admin
Get-MpPreference | Select-Object EnableControlledFolderAccess

reg query "HKLM\SYSTEM\CurrentControlSet\Control\CI\Policy" /v VerifiedAndReputablePolicyState
```


# Code Signing

```powershell
# Self-signed certificate oluştur
New-SelfSignedCertificate `
 -Type CodeSigningCert `
 -Subject "CN=WebEdit Dev Cert" `
 -CertStoreLocation "Cert:\CurrentUser\My"

# PFX export et
$cert = Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -like "*WebEdit*" }

Export-PfxCertificate `
 -Cert $cert `
 -FilePath "A:\AppStore\Dev.Apps\WebEdit\webedit.pfx" `
 -Password (ConvertTo-SecureString "1234" -AsPlainText -Force)

# EXE imzala
signtool sign /f C:\Dev\webedit.pfx /p 1234 /fd SHA256 WebEdit.exe

# Self-signed cert’i “Trusted Root” yapma
# Run certmgr.msc
# 	“Trusted Root Certification Authorities”
# 	Import certificate

```

## Build Otomasyonu

WebEdit.csproj dosyana şunu ekle:

```xml
<PropertyGroup Condition="'$(Configuration)'=='Debug'">
  <SignEnabled>true</SignEnabled>
</PropertyGroup>

<Target Name="SignExe" AfterTargets="Build">
  <Exec Command="&quot;C:\Program Files (x86)\Windows Kits\10\bin\x64\signtool.exe&quot; sign /f A:\AppStore\Dev.Apps\WebEdit\webedit.pfx /p 1234 /fd SHA256 /tr http://timestamp.digicert.com /td SHA256 $(TargetPath)" />
</Target>
```

Artık <dotnet watch run> çalışınca:

1. Build olur
2. EXE oluşur
3. Otomatik imzalanır
4. Windows daha az “unknown publisher” uyarısı verir

# Create Project

.editorconfig eklemek için EXPLORER paneline sağ tık ve Generate .editorconfig seçin.

```powershell
# Projenin oluşturulması
dotnet new sln
dotnet new web
dotnet sln add .
dotnet new gitignore
```

Add the following to the .gitignore file:

```text
# SQLite Database
*.db
*.db-shm
*.db-wal

# Uploads Folder
wwwroot/uploads/
wwwroot/uploads/**
```

# launchSettings.json

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "browser": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:5001;http://localhost:5000",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
```

# EFCore

```powershell
dotnet ef --version
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef

dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools

dotnet ef migrations add InitialCreate -o AppData/Migrations
dotnet ef database update
```

# Git

```powershell
# Git Repository
git init
git add .
git commit -m "First Commit"
git branch -M main
git remote add origin https://github.com/emre-mumcu/net.mumcu.TinyMCE.git
git push -u origin main
```

# TinyMCE

```powershell
# CDN
<script src="https://cdn.tiny.cloud/1/no-api-key/tinymce/8/tinymce.min.js"></script>


# node ve npm sürümleri
node -v
npm -v

# Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
# or 
# Run PowerShell as Administrator and execute
# Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
# or
# use cmd
npm init -y
npm install tinymce
```

# Certificate Error

dotnet dev-certs https --check
dotnet dev-certs https --check --verbose
dotnet dev-certs https --clean
dotnet dev-certs https --trust

Sertifikayı dışa aktar:
dotnet dev-certs https -ep "%USERPROFILE%\Desktop\aspnetcore.pfx" -p 123456

# Packages

dotnet add package Mapster



dotnet watch run --no-hot-reload






dotnet add package net.mumcu.MVC.Extensions


dotnet nuget locals all --clear



https://www.tiny.cloud/docs/tinymce/5/



/*
 * Raw SQL queries for unmapped types
 * 
 var summaries =
    await context.Database.SqlQuery<PostSummary>(
            @$"SELECT b.Name AS BlogName, p.Title AS PostTitle, p.PublishedOn
            FROM Posts AS p
            INNER JOIN Blogs AS b ON p.BlogId = b.Id")
        .ToListAsync();
 */


wwwroot/uploads/ klasörünü ve içindeki her şeyi .gitignore dosyasına şöyle ekleyebilirsin:
wwwroot/uploads/
Trailing slash (/) klasör olduğunu belirtir ve içindeki tüm dosya/alt klasörleri kapsar.

Eğer klasörün kendisini de tamamen gizlemek istiyorsan (zaten aynı anlama gelir, ama bazen ikisi birden yazılır):
wwwroot/uploads/
wwwroot/uploads/**

Klasör zaten git'e eklenmiş ve tracked durumundaysa, önce önbellekten çıkarman gerekir:
bashgit rm -r --cached wwwroot/uploads/
Ardından commit atabilirsin. Aksi halde .gitignore etkisiz kalır.