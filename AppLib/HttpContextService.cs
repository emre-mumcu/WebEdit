using System.Net;

namespace WebEdit.AppLib;

public static class HttpContextService
{
	private static readonly HashSet<string> _loopbacks = new(StringComparer.OrdinalIgnoreCase)
	{
		"::1", "127.0.0.1"
	};

	/// <summary>
	/// Client'ın gerçek IP adresini döndürür.
	/// Reverse proxy / load balancer arkasında çalışırken de doğru sonuç verir.
	/// </summary>
	public static string? GetClientIpAddress(this IHttpContextAccessor accessor) => accessor.HttpContext?.GetClientIpAddress();

	public static string? GetClientIpAddress(this HttpContext context)
	{
		// 1. X-Forwarded-For — en yaygın proxy header'ı (nginx, AWS ALB, vb.)
		//    Değer şöyle gelir: "clientIP, proxy1, proxy2"
		//    İlk geçerli public IP'yi alıyoruz.
		var xForwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();

		if (!string.IsNullOrWhiteSpace(xForwardedFor))
		{
			var ip = xForwardedFor
				.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Select(raw => raw.Trim())
				.FirstOrDefault(IsValidPublicIp);

			if (ip is not null) return ip;
		}

		// 2. CF-Connecting-IP — Cloudflare kullanıyorsan bu header daha güvenilir
		var cfConnectingIp = context.Request.Headers["CF-Connecting-IP"].ToString().Trim();
		
		if (IsValidPublicIp(cfConnectingIp)) return cfConnectingIp;

		// 3. X-Real-IP — nginx'in genellikle set ettiği header
		var xRealIp = context.Request.Headers["X-Real-IP"].ToString().Trim();

		if (IsValidPublicIp(xRealIp)) return xRealIp;

		// 4. Fallback — proxy yok, doğrudan bağlantı
		var remoteIp = context.Connection.RemoteIpAddress;

		if (remoteIp is null) return null;

		// IPv4-mapped IPv6 adresini normalize et: "::ffff:1.2.3.4" → "1.2.3.4"
		if (remoteIp.IsIPv4MappedToIPv6) remoteIp = remoteIp.MapToIPv4();

		return remoteIp.ToString();
	}

	// -----------------------------------------------------------------------

	private static bool IsValidPublicIp(string? raw)
	{
		if (string.IsNullOrWhiteSpace(raw)) return false;
		if (_loopbacks.Contains(raw)) return false;

		if (!IPAddress.TryParse(raw, out var ip)) return false;

		// IPv4-mapped IPv6 → IPv4 olarak değerlendir
		if (ip.IsIPv4MappedToIPv6) ip = ip.MapToIPv4();

		return ip.AddressFamily switch
		{
			// Loopback zaten yakalandı, private subnet'leri de dışla
			System.Net.Sockets.AddressFamily.InterNetwork => !IsPrivateIPv4(ip),
			System.Net.Sockets.AddressFamily.InterNetworkV6 => !IPAddress.IsLoopback(ip),
			_ => false
		};
	}

	private static bool IsPrivateIPv4(IPAddress ip)
	{
		var bytes = ip.GetAddressBytes();
		
		return bytes[0] == 10                                   // 10.0.0.0/8
			|| (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) // 172.16–31.x.x
			|| (bytes[0] == 192 && bytes[1] == 168)             // 192.168.x.x
			|| bytes[0] == 127;                                 // 127.x.x.x
	}
}
