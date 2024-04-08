using System.Net;

namespace Test
{
	public static class IPAddressExtensions
	{
		public static IPAddress GetUpperBound(IPAddress lowerBound, int subnetMask)
		{
			if (subnetMask < 0 || subnetMask > 32)
				throw new ArgumentOutOfRangeException("Маска подсети должна быть в пределах от 0 до 32");

			byte[] lowerBytes = lowerBound.GetAddressBytes();

			uint binaryMask = ~0u << (32 - subnetMask);
			byte[] maskBytes = BitConverter.GetBytes(binaryMask);
			if (BitConverter.IsLittleEndian) Array.Reverse(maskBytes);

			byte[] upperBytes = new byte[4];
			for (int i = 0; i < upperBytes.Length; i++)
			{
				upperBytes[i] = (byte)(lowerBytes[i] | ~maskBytes[i]);
			}

			return new IPAddress(upperBytes);
		}

		public static bool IsInRange(IPAddress ipAddress, IPAddress lowerBound)
		{
			byte[] ipBytes = ipAddress.GetAddressBytes();
			byte[] lowerBytes = lowerBound.GetAddressBytes();

			for (int i = 0; i < ipBytes.Length; i++)
			{
				if (ipBytes[i] < lowerBytes[i])
					return false;
			}

			return true;
		}

		public static bool IsInRange(IPAddress ipAddress, IPAddress lowerBound, IPAddress upperBound)
		{
			byte[] ipBytes = ipAddress.GetAddressBytes();
			byte[] lowerBytes = lowerBound.GetAddressBytes();
			byte[] upperBytes = upperBound.GetAddressBytes();

			for (int i = 0; i < ipBytes.Length; i++)
			{
				if (ipBytes[i] < lowerBytes[i] || ipBytes[i] > upperBytes[i])
					return false;
			}

			return true;
		}

	}
}
