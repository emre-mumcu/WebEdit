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