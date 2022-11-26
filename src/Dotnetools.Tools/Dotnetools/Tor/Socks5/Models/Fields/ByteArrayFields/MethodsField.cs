using Dotnetools.Helpers;
using Dotnetools.Tor.Socks5.Models.Bases;
using Dotnetools.Tor.Socks5.Models.Fields.OctetFields;

namespace Dotnetools.Tor.Socks5.Models.Fields.ByteArrayFields;

public class MethodsField : ByteArraySerializableBase
{
	public MethodsField(byte[] bytes)
	{
		Guard.NotNullOrEmpty(nameof(bytes), bytes);

		foreach (var b in bytes)
		{
			if (b != MethodField.NoAuthenticationRequired && b != MethodField.UsernamePassword)
			{
				throw new FormatException($"Unrecognized authentication method: {ByteHelpers.ToHex(b)}.");
			}
		}

		Bytes = bytes;
	}

	public MethodsField(params MethodField[] methods)
	{
		Guard.NotNullOrEmpty(nameof(methods), methods);

		int count = methods.Length;
		Bytes = new byte[count];
		for (int i = 0; i < count; i++)
		{
			Bytes[i] = methods[i].ToByte();
		}
	}

	private byte[] Bytes { get; }

	public override byte[] ToBytes() => Bytes;
}