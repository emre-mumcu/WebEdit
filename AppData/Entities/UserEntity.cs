using System;

namespace WebEdit.AppData.Entities;

public class UserEntity: BaseEntity
{
	public required string UserName { get; set; }
	public required string EncryptionKey { get; set; }
}
