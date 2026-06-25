/*
var container = PasswordEncryption.Encrypt(
    File.ReadAllBytes("data.bin"),
    "123456");

PasswordEncryption.ChangePassword(
    container,
    "123456",
    "YeniSifre123!");

var data = PasswordEncryption.Decrypt(
    container,
    "YeniSifre123!");
*/

using System.Security.Cryptography;

public sealed class EncryptionContainer
{
	public byte[] EncryptedData { get; set; } = [];
	public byte[] WrappedDek { get; set; } = [];
	public byte[] Salt { get; set; } = [];
	public byte[] DataNonce { get; set; } = [];
	public byte[] DataTag { get; set; } = [];
	public byte[] DekNonce { get; set; } = [];
	public byte[] DekTag { get; set; } = [];
}

public static class PasswordEncryption
{
	private const int KeySize = 32; // 256 bit
	private const int SaltSize = 16;
	private const int NonceSize = 12;
	private const int TagSize = 16;
	private const int Iterations = 100_000;

	public static EncryptionContainer Encrypt(
		byte[] plainData,
		string password)
	{
		// 1. DEK üret
		byte[] dek = RandomNumberGenerator.GetBytes(KeySize);

		// 2. Veriyi DEK ile şifrele
		byte[] dataNonce = RandomNumberGenerator.GetBytes(NonceSize);
		byte[] encryptedData = new byte[plainData.Length];
		byte[] dataTag = new byte[TagSize];

		using (var aes = new AesGcm(dek, TagSize))
		{
			aes.Encrypt(
				dataNonce,
				plainData,
				encryptedData,
				dataTag);
		}

		// 3. Şifreden KEK üret
		byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

		byte[] kek = Rfc2898DeriveBytes.Pbkdf2(
			password,
			salt,
			Iterations,
			HashAlgorithmName.SHA256,
			KeySize);

		// 4. DEK'i KEK ile sar
		byte[] dekNonce = RandomNumberGenerator.GetBytes(NonceSize);
		byte[] wrappedDek = new byte[dek.Length];
		byte[] dekTag = new byte[TagSize];

		using (var aes = new AesGcm(kek, TagSize))
		{
			aes.Encrypt(
				dekNonce,
				dek,
				wrappedDek,
				dekTag);
		}

		CryptographicOperations.ZeroMemory(dek);
		CryptographicOperations.ZeroMemory(kek);

		return new EncryptionContainer
		{
			EncryptedData = encryptedData,
			WrappedDek = wrappedDek,
			Salt = salt,
			DataNonce = dataNonce,
			DataTag = dataTag,
			DekNonce = dekNonce,
			DekTag = dekTag
		};
	}

	public static byte[] Decrypt(
		EncryptionContainer container,
		string password)
	{
		byte[] kek = Rfc2898DeriveBytes.Pbkdf2(
			password,
			container.Salt,
			Iterations,
			HashAlgorithmName.SHA256,
			KeySize);

		byte[] dek = new byte[KeySize];

		using (var aes = new AesGcm(kek, TagSize))
		{
			aes.Decrypt(
				container.DekNonce,
				container.WrappedDek,
				container.DekTag,
				dek);
		}

		byte[] plainData = new byte[container.EncryptedData.Length];

		using (var aes = new AesGcm(dek, TagSize))
		{
			aes.Decrypt(
				container.DataNonce,
				container.EncryptedData,
				container.DataTag,
				plainData);
		}

		CryptographicOperations.ZeroMemory(dek);
		CryptographicOperations.ZeroMemory(kek);

		return plainData;
	}

	public static void ChangePassword(
		EncryptionContainer container,
		string oldPassword,
		string newPassword)
	{
		// Eski KEK
		byte[] oldKek = Rfc2898DeriveBytes.Pbkdf2(
			oldPassword,
			container.Salt,
			Iterations,
			HashAlgorithmName.SHA256,
			KeySize);

		// DEK'i aç
		byte[] dek = new byte[KeySize];

		using (var aes = new AesGcm(oldKek, TagSize))
		{
			aes.Decrypt(
				container.DekNonce,
				container.WrappedDek,
				container.DekTag,
				dek);
		}

		// Yeni salt
		byte[] newSalt = RandomNumberGenerator.GetBytes(SaltSize);

		// Yeni KEK
		byte[] newKek = Rfc2898DeriveBytes.Pbkdf2(
			newPassword,
			newSalt,
			Iterations,
			HashAlgorithmName.SHA256,
			KeySize);

		// DEK'i yeniden sar
		byte[] newNonce = RandomNumberGenerator.GetBytes(NonceSize);
		byte[] newWrappedDek = new byte[KeySize];
		byte[] newTag = new byte[TagSize];

		using (var aes = new AesGcm(newKek, TagSize))
		{
			aes.Encrypt(
				newNonce,
				dek,
				newWrappedDek,
				newTag);
		}

		container.Salt = newSalt;
		container.WrappedDek = newWrappedDek;
		container.DekNonce = newNonce;
		container.DekTag = newTag;

		CryptographicOperations.ZeroMemory(dek);
		CryptographicOperations.ZeroMemory(oldKek);
		CryptographicOperations.ZeroMemory(newKek);
	}
}