/*
string encrypted = StringVault.Encrypt(
    "Merhaba Dünya",
    "EskiŞifre");

string plain = StringVault.Decrypt(
    encrypted,
    "EskiŞifre");

string reEncrypted = StringVault.ChangePassword(
    encrypted,
    "EskiŞifre",
    "YeniŞifre");
*/

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

public static class StringVault
{
	private const int KeySize = 32;
	private const int SaltSize = 16;
	private const int NonceSize = 12;
	private const int TagSize = 16;
	private const int Iterations = 100_000;

	private sealed class Payload
	{
		public string Salt { get; set; } = "";
		public string WrappedDek { get; set; } = "";
		public string DekNonce { get; set; } = "";
		public string DekTag { get; set; } = "";

		public string Data { get; set; } = "";
		public string DataNonce { get; set; } = "";
		public string DataTag { get; set; } = "";
	}

	public static string Encrypt(string plainText, string password)
	{
		byte[] dek = RandomNumberGenerator.GetBytes(KeySize);

		byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

		byte[] dataNonce = RandomNumberGenerator.GetBytes(NonceSize);
		byte[] encryptedData = new byte[plainBytes.Length];
		byte[] dataTag = new byte[TagSize];

		using (var aes = new AesGcm(dek, TagSize))
		{
			aes.Encrypt(
				dataNonce,
				plainBytes,
				encryptedData,
				dataTag);
		}

		byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

		byte[] kek = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			Iterations,
			HashAlgorithmName.SHA256,
			KeySize);

		byte[] dekNonce = RandomNumberGenerator.GetBytes(NonceSize);
		byte[] wrappedDek = new byte[KeySize];
		byte[] dekTag = new byte[TagSize];

		using (var aes = new AesGcm(kek, TagSize))
		{
			aes.Encrypt(
				dekNonce,
				dek,
				wrappedDek,
				dekTag);
		}

		var payload = new Payload
		{
			Salt = Convert.ToBase64String(salt),
			WrappedDek = Convert.ToBase64String(wrappedDek),
			DekNonce = Convert.ToBase64String(dekNonce),
			DekTag = Convert.ToBase64String(dekTag),

			Data = Convert.ToBase64String(encryptedData),
			DataNonce = Convert.ToBase64String(dataNonce),
			DataTag = Convert.ToBase64String(dataTag)
		};

		CryptographicOperations.ZeroMemory(dek);
		CryptographicOperations.ZeroMemory(kek);

		return Convert.ToBase64String(
			Encoding.UTF8.GetBytes(
				JsonSerializer.Serialize(payload)));
	}

	public static string Decrypt(string encryptedText, string password)
	{
		var payload = JsonSerializer.Deserialize<Payload>(
			Encoding.UTF8.GetString(
				Convert.FromBase64String(encryptedText)))
			?? throw new InvalidOperationException();

		byte[] salt = Convert.FromBase64String(payload.Salt);

		byte[] kek = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			Iterations,
			HashAlgorithmName.SHA256,
			KeySize);

		byte[] dek = new byte[KeySize];

		using (var aes = new AesGcm(kek, TagSize))
		{
			aes.Decrypt(
				Convert.FromBase64String(payload.DekNonce),
				Convert.FromBase64String(payload.WrappedDek),
				Convert.FromBase64String(payload.DekTag),
				dek);
		}

		byte[] plainBytes =
			new byte[Convert.FromBase64String(payload.Data).Length];

		using (var aes = new AesGcm(dek, TagSize))
		{
			aes.Decrypt(
				Convert.FromBase64String(payload.DataNonce),
				Convert.FromBase64String(payload.Data),
				Convert.FromBase64String(payload.DataTag),
				plainBytes);
		}

		CryptographicOperations.ZeroMemory(dek);
		CryptographicOperations.ZeroMemory(kek);

		return Encoding.UTF8.GetString(plainBytes);
	}

	public static string ChangePassword(
		string encryptedText,
		string oldPassword,
		string newPassword)
	{
		string plainText = Decrypt(encryptedText, oldPassword);

		return Encrypt(plainText, newPassword);
	}
}