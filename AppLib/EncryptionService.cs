using System.Security.Cryptography;
using System.Text;

namespace WebEdit.AppLib;

public interface IEncryptionService
{
	string Encrypt(string? plainText);
	string Decrypt(string? cipherText);
}

public class AesGcmEncryptionService : IEncryptionService
{
	private byte[]? _key;

	/*
	// AES Key 32 byte (256-bit) olmalı
	public AesGcmEncryptionService(IConfiguration configuration)
	{
		var keyBase64 = configuration["Encryption:Key"]
			?? throw new InvalidOperationException("Encryption key is not configured.");

		_key = Convert.FromBase64String(keyBase64);

		if (_key.Length != 32)
			throw new InvalidOperationException("Encryption key must be 256-bit (32 bytes).");
	}

	public AesGcmEncryptionService(string password)
	{
		if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Şifre boş olamaz.");

		var passwordBytes = Encoding.UTF8.GetBytes(password);

		// PBKDF2 ile key üretimi (salt ile)
		_key = Rfc2898DeriveBytes.Pbkdf2(
			password: Encoding.UTF8.GetBytes(password),
			// salt: ReadOnlySpan<byte>.Empty, // boş salt
			salt: SHA256.HashData(passwordBytes)[..16], // şifreden salt			
			iterations: 600_000,
			hashAlgorithm: HashAlgorithmName.SHA256,
			outputLength: 32
		);

		// PBKDF2 olmadan key üretimi (salt yok)
		_key = SHA256.HashData(
			Encoding.UTF8.GetBytes(password)
		);
	}
	*/

	public void Init(string password)
    {
		if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Şifre boş olamaz.");

		var passwordBytes = Encoding.UTF8.GetBytes(password);

		// PBKDF2 ile key üretimi (salt ile)
		/*
		_key = Rfc2898DeriveBytes.Pbkdf2(
			password: Encoding.UTF8.GetBytes(password),
			// salt: ReadOnlySpan<byte>.Empty, // boş salt
			salt: SHA256.HashData(passwordBytes)[..16], // şifreden salt			
			iterations: 600_000,
			hashAlgorithm: HashAlgorithmName.SHA256,
			outputLength: 32
		);
		*/

		// PBKDF2 olmadan key üretimi (salt yok)
		_key = SHA256.HashData(Encoding.UTF8.GetBytes(password));
	}

	public string Encrypt(string? plainText)
	{
		if (string.IsNullOrEmpty(plainText)) return string.Empty;

		// UTF-8 ile encode → tüm dilleri destekler
		var plainBytes = Encoding.UTF8.GetBytes(plainText);

		// Her şifrelemede yeni random nonce (12 byte = 96-bit, GCM standardı)
		var nonce = new byte[AesGcm.NonceByteSizes.MaxSize]; // 12 byte
		
		RandomNumberGenerator.Fill(nonce);

		var cipherBytes = new byte[plainBytes.Length];
		var tag = new byte[AesGcm.TagByteSizes.MaxSize]; // 16 byte

		using var aesGcm = new AesGcm(_key, AesGcm.TagByteSizes.MaxSize);
		aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

		// Format: nonce(12) + tag(16) + ciphertext → Base64
		var result = new byte[nonce.Length + tag.Length + cipherBytes.Length];		

		Buffer.BlockCopy(nonce, 0, result, 0, nonce.Length);
		Buffer.BlockCopy(tag, 0, result, nonce.Length, tag.Length);
		Buffer.BlockCopy(cipherBytes, 0, result, nonce.Length + tag.Length, cipherBytes.Length);

		return Convert.ToBase64String(result);
	}

	public string Decrypt(string? cipherText)
	{
		if (string.IsNullOrEmpty(cipherText)) return string.Empty;

		var fullBytes = Convert.FromBase64String(cipherText);

		const int nonceSize = 12;
		const int tagSize = 16;

		if (fullBytes.Length < nonceSize + tagSize)
			throw new CryptographicException("Invalid ciphertext.");

		var nonce = fullBytes[..nonceSize];
		var tag = fullBytes[nonceSize..(nonceSize + tagSize)];
		var cipherBytes = fullBytes[(nonceSize + tagSize)..];

		var plainBytes = new byte[cipherBytes.Length];

		using var aesGcm = new AesGcm(_key, AesGcm.TagByteSizes.MaxSize);
		aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

		return Encoding.UTF8.GetString(plainBytes);
	}
}

public class KeyService
{
	public void GenerateKey()
	{
		var key = new byte[32];
		RandomNumberGenerator.Fill(key);
		Console.WriteLine(Convert.ToBase64String(key));
	}
}

/*
// 1. DI Kaydı. Program.cs:
builder.Services.AddSingleton<IEncryptionService, AesGcmEncryptionService>();

// 2. Key Kaydı
// appsettings.Development.json (Dev-Test)
{
  "Encryption": {
    "Key": "BURAYA_ÜRETTIĞIN_32_BYTE_BASE64_KEY"
  }
}
# Ya da .NET User Secrets (Prod)
dotnet user-secrets init
dotnet user-secrets set "Encryption:Key" "ÜRETTIĞIN_KEY"

# Ya da environment variable (Docker, Azure vb.)
# ENCRYPTION__KEY=ÜRETTIĞIN_BASE64_KEY

*/
